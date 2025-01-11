using Microsoft.AspNetCore.Identity;

namespace SearchService.Domain.Entities;

public class Role : IdentityRole<Guid>
{
    public override Guid Id { get; set; } = Guid.CreateVersion7();
}
