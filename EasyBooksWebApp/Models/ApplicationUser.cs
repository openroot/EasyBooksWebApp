using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace EasyBooksWebApp.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        // Navigation Properties

        public List<Customer> Customers { get; set; }
        public List<Supplier> Suppliers { get; set; }
    }
}
