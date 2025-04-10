using Microsoft.AspNetCore.Identity;

namespace MinimalAPITemplate.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Handle { get; set; }
    }
}
