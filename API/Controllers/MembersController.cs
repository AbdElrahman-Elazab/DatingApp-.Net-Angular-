using API.Data;
using API.Entities;
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
}