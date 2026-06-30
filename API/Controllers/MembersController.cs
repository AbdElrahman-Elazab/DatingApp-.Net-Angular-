using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


public class MembersController(AppDbContext _context) : BaseController
{
    [HttpGet]
    public async Task< ActionResult<IReadOnlyList<AppUser>>> GetMembers()
    {
        var members=await _context.Users.ToListAsync();

        return members;

    }
    [Authorize]
    [HttpGet("{id}")]
      public async Task<ActionResult<AppUser>> GetMember(string id)
    {
        var member=await _context.Users.FindAsync(id);

        if(member is null)
        return NotFound();
        return member;

    }
}