using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyBooksWebApp.Models
{
    public class Supplier
    {
        [Key]
        public long SupplierID { get; set; }                                    // PRIMARY Key

        [Required]
        [ForeignKey("FK_Supplier_AspNetUsers_UserId")]
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

        [Column("Company")]
        [StringLength(60, MinimumLength = 1)]
        public string Company { get; set; }

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

        [ForeignKey("FK_Supplier_State_StateID")]
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
        public List<Bill> Bills { get; set; }
        public List<BillPayment> BillPayments { get; set; }
        public State State { get; set; }
    }
}
