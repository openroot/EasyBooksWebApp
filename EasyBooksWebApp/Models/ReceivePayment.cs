using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyBooksWebApp.Models
{
    public class ReceivePayment
    {
        [Key]
        public long ReceivePaymentID { get; set; }                              // PRIMARY Key

        [Required]
        [ForeignKey("FK_ReceivePayment_Customer_CustomerID")]
        [Column("CustomerID")]
        public long CustomerID { get; set; }                                    // FOREIGN Key to -> Customer:CustomerID

        [Required]
        [Column("PaymentDate")]
        [Display(Name = "Payment Date")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        [Required]
        [ForeignKey("FK_ReceivePayment_PaymentMethod_PaymentMethodID")]
        [Column("PaymentMethodID")]
        [Display(Name = "Payment Method")]
        public int PaymentMethodID { get; set; }                                // FOREIGN Key to -> PaymentMethod:PaymentMethodID

        [Column("ReferenceNo")]
        [Display(Name = "Reference No")]
        [StringLength(60)]
        public string ReferenceNo { get; set; }

        [Required]
        [Column("AmountReceived")]
        [Display(Name = "Amount Received")]
        [Range(1, 100000000)]
        public decimal AmountReceived { get; set; }


        // Navigation Properties

        public Customer Customer { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
