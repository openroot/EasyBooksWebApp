using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EasyBooksWebApp.Data;
using EasyBooksWebApp.Models;

namespace EasyBooksWebApp.Controllers
{
    public class SeedContentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SeedContentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SeedContent
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["PaymentMethodSeedStatus"] = SeedPaymentMethodTable() ? "Table 'PaymentMethod' seeded" : "Table 'PaymentMethod' already seeded";
                ViewData["StateSeedStatus"] = SeedStateTable() ? "Table 'State' seeded" : "Table 'State' already seeded";
                
                return View();
            }
            return Redirect("Account/Login");
        }

        private bool SeedPaymentMethodTable()
        {
            var retValue = false;
            // Look for any payment methods already seeded
            if (!_context.PaymentMethod.Any())
            {
                _context.PaymentMethod.AddRange(
                    new PaymentMethod { Name = "Bank Transfer" },
                    new PaymentMethod { Name = "Cash" },
                    new PaymentMethod { Name = "Cheque" },
                    new PaymentMethod { Name = "Credit Card" },
                    new PaymentMethod { Name = "Debit Card" }
                );
                _context.SaveChanges();
                retValue = true;
            }
            return retValue;
        }

        private bool SeedStateTable()
        {
            var retValue = false;
            // Look for any states already seeded
            if (!_context.State.Any())
            {
                _context.State.AddRange(
                    new State { Name = "Andaman and Nicobar Islands" },
                    new State { Name = "Andhra Pradesh" },
                    new State { Name = "Arunachal Pradesh" },
                    new State { Name = "Assam" },
                    new State { Name = "Bihar" },
                    new State { Name = "Chandigarh" },
                    new State { Name = "Chhattisgarh" },
                    new State { Name = "Dadra and Nagar Haveli" },
                    new State { Name = "Daman and Diu" },
                    new State { Name = "Delhi" },
                    new State { Name = "Goa" },
                    new State { Name = "Gujarat" },
                    new State { Name = "Haryana" },
                    new State { Name = "Himachal Pradesh" },
                    new State { Name = "Jammu and Kashmir" },
                    new State { Name = "Jharkhand" },
                    new State { Name = "Karnataka" },
                    new State { Name = "Kerala" },
                    new State { Name = "Lakshadweep" },
                    new State { Name = "Madhya Pradesh" },
                    new State { Name = "Maharashtra" },
                    new State { Name = "Manipur" },
                    new State { Name = "Meghalaya" },
                    new State { Name = "Mizoram" },
                    new State { Name = "Nagaland" },
                    new State { Name = "Odisha" },
                    new State { Name = "Puducherry" },
                    new State { Name = "Punjab" },
                    new State { Name = "Rajasthan" },
                    new State { Name = "Sikkim" },
                    new State { Name = "Tamil Nadu" },
                    new State { Name = "Telangana" },
                    new State { Name = "Tripura" },
                    new State { Name = "Uttar Pradesh" },
                    new State { Name = "Uttarakhand" },
                    new State { Name = "West Bengal" }
                );
                _context.SaveChanges();
                retValue = true;
            }
            return retValue;
        }
    }
}