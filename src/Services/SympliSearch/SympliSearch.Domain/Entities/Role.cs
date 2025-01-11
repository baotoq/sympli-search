using Microsoft.AspNetCore.Identity;

namespace SympliSearch.Domain.Entities;

public class Role : IdentityRole<Guid>
{
    public override Guid Id { get; set; } = Guid.CreateVersion7();
}
