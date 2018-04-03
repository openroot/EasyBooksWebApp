using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyBooksWebApp.Data;
using EasyBooksWebApp.Models;

namespace EasyBooksWebApp.Controllers
{
    public class ReceivePaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReceivePaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ReceivePayments
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserQueryable().
                    Include(u => u.Customers).
                        ThenInclude(c => c.ReceivePayments).
                            ThenInclude(rP => rP.PaymentMethod)                            
                    .SingleOrDefaultAsync();
                
                var receivePayments = user.Customers.SelectMany(c => c.ReceivePayments).ToList();
                return View(receivePayments);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: ReceivePayments/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await GetUserQueryable().
                        Include(u => u.Customers).
                            ThenInclude(c => c.ReceivePayments).
                                ThenInclude(rP => rP.PaymentMethod).
                        Include(u => u.Customers).
                            ThenInclude(c => c.State)
                        .SingleOrDefaultAsync();

                var receivePayment = user.Customers.SelectMany(c => c.ReceivePayments).SingleOrDefault(i => i.ReceivePaymentID == id);
                if (receivePayment == null)
                {
                    return NotFound();
                }

                return View(receivePayment);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: ReceivePayments/Create
        public async Task<IActionResult> Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserQueryable().
                        Include(u => u.Customers).
                            ThenInclude(c => c.State)
                        .SingleOrDefaultAsync();

                var customerList = user.Customers.Select(c => new { CustomerID = c.CustomerID, Description = c.FirstMidName + " " + c.LastName + ", " + c.State.Name });
                ViewData["CustomerID"] = new SelectList(customerList, "CustomerID", "Description");
                ViewData["PaymentMethodID"] = new SelectList(_context.PaymentMethod.OrderBy(pM => pM.Name), "PaymentMethodID", "Name");
                return View();
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: ReceivePayments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReceivePaymentID,CustomerID,PaymentDate,PaymentMethodID,ReferenceNo,AmountReceived")] ReceivePayment receivePayment)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(receivePayment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                var user = await GetUserQueryable().
                        Include(u => u.Customers).
                            ThenInclude(c => c.State)
                        .SingleOrDefaultAsync();

                var customerList = user.Customers.Select(c => new { CustomerID = c.CustomerID, Description = c.FirstMidName + " " + c.LastName + ", " + c.State.Name });
                ViewData["CustomerID"] = new SelectList(customerList, "CustomerID", "Description", receivePayment.CustomerID);
                ViewData["PaymentMethodID"] = new SelectList(_context.PaymentMethod.OrderBy(pM => pM.Name), "PaymentMethodID", "Name", receivePayment.PaymentMethodID);
                return View(receivePayment);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: ReceivePayments/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await GetUserQueryable().
                       Include(u => u.Customers).
                           ThenInclude(c => c.ReceivePayments).
                       Include(u => u.Customers).
                           ThenInclude(c => c.State)
                       .SingleOrDefaultAsync();

                var receivePayment = user.Customers.SelectMany(c => c.ReceivePayments).SingleOrDefault(i => i.ReceivePaymentID == id);
                if (receivePayment == null)
                {
                    return NotFound();
                }
                var customerList = user.Customers.Select(c => new { CustomerID = c.CustomerID, Description = c.FirstMidName + " " + c.LastName + ", " + c.State.Name });
                ViewData["CustomerID"] = new SelectList(customerList, "CustomerID", "Description", receivePayment.CustomerID);
                ViewData["PaymentMethodID"] = new SelectList(_context.PaymentMethod.OrderBy(pM => pM.Name), "PaymentMethodID", "Name", receivePayment.PaymentMethodID);
                return View(receivePayment);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: ReceivePayments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ReceivePaymentID,CustomerID,PaymentDate,PaymentMethodID,ReferenceNo,AmountReceived")] ReceivePayment receivePayment)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id != receivePayment.ReceivePaymentID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(receivePayment);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ReceivePaymentExists(receivePayment.ReceivePaymentID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }

                var user = await GetUserQueryable().
                       Include(u => u.Customers).
                           ThenInclude(c => c.State)
                       .SingleOrDefaultAsync();

                var customerList = user.Customers.Select(c => new { CustomerID = c.CustomerID, Description = c.FirstMidName + " " + c.LastName + ", " + c.State.Name });
                ViewData["CustomerID"] = new SelectList(customerList, "CustomerID", "Description", receivePayment.CustomerID);
                ViewData["PaymentMethodID"] = new SelectList(_context.PaymentMethod.OrderBy(pM => pM.Name), "PaymentMethodID", "Name", receivePayment.PaymentMethodID);
                return View(receivePayment);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: ReceivePayments/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await GetUserQueryable().
                       Include(u => u.Customers).
                           ThenInclude(c => c.ReceivePayments).
                                ThenInclude(rP => rP.PaymentMethod).
                       Include(u => u.Customers).
                           ThenInclude(c => c.State)
                       .SingleOrDefaultAsync();

                var receivePayment = user.Customers.SelectMany(c => c.ReceivePayments).SingleOrDefault(i => i.ReceivePaymentID == id);
                if (receivePayment == null)
                {
                    return NotFound();
                }

                return View(receivePayment);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: ReceivePayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserQueryable().
                       Include(u => u.Customers).
                           ThenInclude(c => c.ReceivePayments)
                       .SingleOrDefaultAsync();

                var receivePayment = user.Customers.SelectMany(c => c.ReceivePayments).SingleOrDefault(i => i.ReceivePaymentID == id);
                if (receivePayment == null)
                {
                    return NotFound();
                }

                _context.ReceivePayment.Remove(receivePayment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        #region Helpers

        private bool ReceivePaymentExists(long id)
        {
            return _context.ReceivePayment.Any(e => e.ReceivePaymentID == id);
        }

        private IQueryable<ApplicationUser> GetUserQueryable()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return _context.Users.Where(u => u.UserName.Equals(User.Identity.Name));
            }
            return null;
        }

        #endregion
    }
}
