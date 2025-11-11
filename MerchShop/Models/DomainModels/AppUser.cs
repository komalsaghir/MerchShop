using Microsoft.AspNetCore.Identity;

namespace MerchShop.Models.DomainModels
{
    public class AppUser : IdentityUser
    {
        // Additional properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties for relationships
        // For example, if you have a one-to-many relationship with another entity
        // public ICollection<Order> Orders { get; set; }

        // Custom methods
        // You can add custom methods to perform user-specific operations
        // For example, methods to update user profile, change password, etc.

        // Constructor
        // You may want to define a constructor to initialize properties
        // public AppUser()
        // {
        //     IsActive = true;
        // }
    }
}
