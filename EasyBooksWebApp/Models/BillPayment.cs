using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyBooksWebApp.Models
{
    public class BillPayment
    {
        [Key]
        public long BillPaymentID { get; set; }                                 // PRIMARY Key

        [Required]
        [ForeignKey("FK_BillPayment_Supplier_SupplierID")]
        [Column("SupplierID")]
        public long SupplierID { get; set; }                                    // FOREIGN Key to -> Supplier:SupplierID

        [Required]
        [Column("PaymentDate")]
        [Display(Name = "Payment Date")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        [Required]
        [ForeignKey("FK_BillPayment_PaymentMethod_PaymentMethodID")]
        [Column("PaymentMethodID")]
        [Display(Name = "Payment Method")]
        public int PaymentMethodID { get; set; }                                // FOREIGN Key to -> PaymentMethod:PaymentMethodID

        [Column("ReferenceNo")]
        [Display(Name = "Reference No")]
        [StringLength(60)]
        public string ReferenceNo { get; set; }

        [Required]
        [Column("AmountPaid")]
        [Display(Name = "Amount Paid")]
        [Range(1, 100000000)]
        public decimal AmountPaid { get; set; }


        // Navigation Properties

        public Supplier Supplier { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
