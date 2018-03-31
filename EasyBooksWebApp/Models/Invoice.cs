using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyBooksWebApp.Models
{
    public class Invoice
    {
        [Key]
        public long InvoiceID { get; set; }                                     // PRIMARY Key

        [Required]
        [ForeignKey("FK_Invoice_Customer_CustomerID")]
        [Column("CustomerID")]
        public long CustomerID { get; set; }                                    // FOREIGN Key to -> Customer:CustomerID

        [Column("InvoiceNo")]
        [Display(Name = "Invoice No")]
        [StringLength(60)]
        public string InvoiceNo { get; set; }

        [Required]
        [Column("InvoiceDate")]
        [Display(Name = "Invoice Date")]
        [DataType(DataType.Date)]
        public DateTime InvoiceDate { get; set; }

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

        public Customer Customer { get; set; }
    }
}
