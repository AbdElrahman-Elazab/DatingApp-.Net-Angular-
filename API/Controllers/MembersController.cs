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
public class MembersController(IMemberRepository memberRepository) : BaseController
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
}