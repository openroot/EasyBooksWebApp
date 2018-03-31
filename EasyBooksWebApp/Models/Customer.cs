using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyBooksWebApp.Models
{
    public class Customer
    {
        [Key]
        public long CustomerID { get; set; }                                    // PRIMARY Key

        [Required]
        [ForeignKey("FK_Customer_AspNetUsers_UserId")]
        [Column("UserId")]
        public string UserId { get; set; }                                      // FOREIGN Key to -> AspNetUsers:UserId

        [Required]
        [Column("FirstMidName")]
        [Display(Name = "First Name")]
        [StringLength(60, MinimumLength = 1)]
        public string FirstMidName { get; set; }

        [Required]
        [Column("LastName")]
        [Display(Name = "Last Name")]
        [StringLength(60, MinimumLength = 1)]
        public string LastName { get; set; }

        [Column("Phone")]
        [Display(Name = "Phone/Mobile")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Column("Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Column("Street")]
        [Display(Name = "Street")]
        [DataType(DataType.MultilineText)]
        public string Street { get; set; }

        [Column("City")]
        [Display(Name = "City/Town")]
        [StringLength(60)]
        public string City { get; set; }

        [ForeignKey("FK_Customer_State_StateID")]
        [Column("StateID")]
        [Display(Name = "State")]
        public int? StateID { get; set; }                                       // FOREIGN Key to -> State:StateID

        [Column("Pin")]
        [DataType(DataType.PostalCode)]
        public string Pin { get; set; }

        [Column("Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Column("ProfileImageName")]
        [Display(Name = "Profile Image")]
        public string ProfileImageName { get; set; }


        // Navigation Properties

        public ApplicationUser User { get; set; }
        public List<Invoice> Invoices { get; set; }
        public List<ReceivePayment> ReceivePayments { get; set; }
        public State State { get; set; }
    }
}
