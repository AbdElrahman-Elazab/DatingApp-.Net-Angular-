using System.Security.Claims;
using API.Data;
using API.Dtos;
using API.Entities;
using API.Extentions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class MembersController(IMemberRepository memberRepository,IPhotoService photoService) : BaseController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
    {
        var members=await memberRepository.GetMembersAsync();

        return Ok(members);

    }
    [Authorize]
    [HttpGet("{id}")]
      public async Task<ActionResult<AppUser>> GetMember(string id)
    {
        var member=await memberRepository.GetMemberByIdAsync(id);

        if(member is null)
        return NotFound();

        return Ok(member);

    }

    [HttpGet("{id}/photos")]
    public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhoto(string id)
    {
        var photos=await memberRepository.GetPhotosForMemberAsync(id);
        return Ok(photos);
        
    }

     [HttpPut]
    public async Task<ActionResult<IReadOnlyList<Photo>>> UpdateMember(MemberUpdateDto  memberUpdateDto)
    {
        var memberId=User.GetMemberId();
        if (memberId is null) BadRequest("Oops - no id in token");
        
        var member=await memberRepository.GetMemberForUpdate(memberId);
        if(member is null) BadRequest("Could not get member");

        member?.DisplayName=memberUpdateDto.DisplayName ?? member.DisplayName;
        member?.Description=memberUpdateDto.Description ?? member.Description;
        member?.Country=memberUpdateDto.Country ?? member.Country;
        member?.City=memberUpdateDto.DisplayName ?? member.City;

        member?.User.DisplayName=memberUpdateDto.DisplayName??member.User.DisplayName;

        memberRepository.Update(member); //optionl

        if(await memberRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update member");

    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<Photo>> AddPhoto([FromForm]IFormFile file)
    {
        var member= await memberRepository.GetMemberForUpdate(User.GetMemberId());
        if(member is null) return BadRequest("Cannot Update Member");

        var result= await photoService.UploadPhotoAsync(file);
        if(result.Error != null) return BadRequest(result.Error.Message);

        var photo =new Photo
        {
            Url=result.SecureUrl.AbsoluteUri,
            MemberId=User.GetMemberId(),
            PublicId=result.PublicId
        };

        if(member.ImageUrl is null){
            member.ImageUrl=photo.Url;
            member.User.ImageUrl=photo.Url;
        };
        
        member.Photoes.Add(photo);

        if(await memberRepository.SaveAllAsync())return photo;

        return BadRequest("Problem adding photo");

        
    }
    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var member= await memberRepository.GetMemberForUpdate(User.GetMemberId());
        if(member is null ) return BadRequest("Can not get member");

        var photo=member.Photoes.FirstOrDefault(p=>p.Id==photoId);

        if(member.ImageUrl == photo?.Url ||photo is null)
         return BadRequest("Cannot set this to main photo");

         member.ImageUrl=photo.Url;
         member.User.ImageUrl=photo.Url;

         if(await memberRepository.SaveAllAsync())return NoContent();

         return BadRequest("Problem set to main photo");

    }


    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult<Photo>> DeletePhoto(int photoId)
    {
        var member= await memberRepository.GetMemberForUpdate(User.GetMemberId());
        if(member is null) return BadRequest("Cannot get  Member");

        var photo =member.Photoes.SingleOrDefault(p=>p.Id==photoId);
        if(photo is null || photo.Url == member.ImageUrl)
        {
            return BadRequest("Cannot Delete this photo");
        }
        if(photo.PublicId is not null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if(result.Error != null) return BadRequest(result.Error.Message);

        }
        member.Photoes.Remove(photo);

        if(await memberRepository.SaveAllAsync())return NoContent();

        return BadRequest("Problem deleteing photo");

        
    }
   
}