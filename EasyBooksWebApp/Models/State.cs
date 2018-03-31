using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyBooksWebApp.Models
{
    public class State
    {
        [Key]
        public int StateID { get; set; }                    // PRIMARY Key

        [Required]
        [Column("Name")]
        [Display(Name = "State")]
        [StringLength(60, MinimumLength = 2)]
        public string Name { get; set; }
    }
}
