using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MemberRepository: IMemberRepository
{
   
    private readonly AppDbContext _context;

    public MemberRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Member>> GetMembersAsync()
    {
        return await _context.Members
            .Include(m => m.Photoes)
            .ToListAsync();
    }

    // جلب عضو محدد بواسطة Id مع صورهم
    public async Task<Member?> GetMemberByIdAsync(string id)
    {
        return await _context.Members
            .Include(m => m.Photoes)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    // جلب صور عضو معين
    public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
    {
        return await _context.Members
            .Where(p => p.Id == memberId)
            .SelectMany(p=>p.Photoes)
            .ToListAsync();
    }
     public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void Update(Member member)
    {
        _context.Entry(member).State = EntityState.Modified;
    }

}
