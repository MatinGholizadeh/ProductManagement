using Microsoft.AspNetCore.Identity;

namespace ProdManagement.Domain.Entities.User;

public class ApplicationUser : IdentityUser
{
    #region Default fields
    //public string Id { get; set; }
    //public string UserName { get; set; }
    //public string PasswordHash { get; set; }
    //public string SecurityStamp { get; set; }
    //public string Email { get; set; }
    //public bool EmailConfirmed { get; set; }
    //public string PhoneNumber { get; set; }
    //public bool PhoneNumberConfirmed { get; set; }
    //public bool TwoFactorEnabled { get; set; }
    //public DateTime? LockoutEndDateUtc { get; set; }
    //public bool LockoutEnabled { get; set; }
    //public int AccessFailedCount { get; set; }
    #endregion Default fields - End

    public string LastName { get; set; }
    public string FirstName { get; set; }
    public bool IsAdmin { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

