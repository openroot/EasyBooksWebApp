using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyBooksWebApp.Models
{
    public class PaymentMethod
    {
        [Key]
        public int PaymentMethodID { get; set; }            // PRIMARY Key

        [Required]
        [Column("Name")]
        [Display(Name = "Payment Method")]
        [StringLength(60, MinimumLength = 2)]
        public string Name { get; set; }
    }
}
