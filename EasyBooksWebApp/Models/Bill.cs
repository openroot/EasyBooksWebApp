using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyBooksWebApp.Models
{
    public class Bill
    {
        [Key]
        public long BillID { get; set; }                                        // PRIMARY Key

        [Required]
        [ForeignKey("FK_Bill_Supplier_SupplierID")]
        [Column("SupplierID")]
        public long SupplierID { get; set; }                                    // FOREIGN Key to -> Supplier:SupplierID

        [Column("BillNo")]
        [Display(Name = "Bill No")]
        [StringLength(60)]
        public string BillNo { get; set; }

        [Required]
        [Column("BillDate")]
        [Display(Name = "Bill Date")]
        [DataType(DataType.Date)]
        public DateTime BillDate { get; set; }

        [Required]
        [Column("DueDate")]
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Column("Memo")]
        [DataType(DataType.MultilineText)]
        public string Memo { get; set; }

        [Required]
        [Column("TotalAmount")]
        [Display(Name = "Total Amount")]
        [Range(1, 100000000)]
        public decimal TotalAmount { get; set; }


        // Navigation Properties

        public Supplier Supplier { get; set; }
    }
}
