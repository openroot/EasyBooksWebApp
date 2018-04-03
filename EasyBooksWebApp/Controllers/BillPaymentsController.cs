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
    public class BillPaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BillPaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BillPayments
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserQueryable().
                    Include(u => u.Suppliers).
                        ThenInclude(s => s.BillPayments).
                            ThenInclude(bP => bP.PaymentMethod)
                    .SingleOrDefaultAsync();

                var billPayments = user.Suppliers.SelectMany(s => s.BillPayments).ToList();
                return View(billPayments);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: BillPayments/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await GetUserQueryable().
                    Include(u => u.Suppliers).
                        ThenInclude(s => s.BillPayments).
                            ThenInclude(bP => bP.PaymentMethod).
                    Include(u => u.Suppliers).
                        ThenInclude(s => s.State)
                    .SingleOrDefaultAsync();

                var billPayment = user.Suppliers.SelectMany(s => s.BillPayments).SingleOrDefault(bP => bP.BillPaymentID == id);
                if (billPayment == null)
                {
                    return NotFound();
                }

                return View(billPayment);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: BillPayments/Create
        public async Task<IActionResult> Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserQueryable().
                    Include(u => u.Suppliers).
                        ThenInclude(s => s.State)
                    .SingleOrDefaultAsync();

                var supplierList = user.Suppliers.Select(s => new { SupplierID = s.SupplierID, Description = s.FirstMidName + " " + s.LastName + ", " + s.State.Name });
                ViewData["SupplierID"] = new SelectList(supplierList, "SupplierID", "Description");
                ViewData["PaymentMethodID"] = new SelectList(_context.PaymentMethod.OrderBy(pM => pM.Name), "PaymentMethodID", "Name");
                return View();
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: BillPayments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BillPaymentID,SupplierID,PaymentDate,PaymentMethodID,ReferenceNo,AmountPaid")] BillPayment billPayment)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(billPayment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                var user = await GetUserQueryable().
                    Include(u => u.Suppliers).
                        ThenInclude(s => s.State)
                    .SingleOrDefaultAsync();

                var supplierList = user.Suppliers.Select(s => new { SupplierID = s.SupplierID, Description = s.FirstMidName + " " + s.LastName + ", " + s.State.Name });
                ViewData["SupplierID"] = new SelectList(supplierList, "SupplierID", "Description", billPayment.SupplierID);
                ViewData["PaymentMethodID"] = new SelectList(_context.PaymentMethod.OrderBy(pM => pM.Name), "PaymentMethodID", "Name", billPayment.PaymentMethodID);
                return View(billPayment);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: BillPayments/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await GetUserQueryable().
                    Include(u => u.Suppliers).
                        ThenInclude(s => s.BillPayments).
                    Include(u => u.Suppliers).
                        ThenInclude(s => s.State)
                    .SingleOrDefaultAsync();

                var billPayment = user.Suppliers.SelectMany(s => s.BillPayments).SingleOrDefault(bP => bP.BillPaymentID == id);
                if (billPayment == null)
                {
                    return NotFound();
                }
                var supplierList = user.Suppliers.Select(s => new { SupplierID = s.SupplierID, Description = s.FirstMidName + " " + s.LastName + ", " + s.State.Name });
                ViewData["SupplierID"] = new SelectList(supplierList, "SupplierID", "Description", billPayment.SupplierID);
                ViewData["PaymentMethodID"] = new SelectList(_context.PaymentMethod.OrderBy(pM => pM.Name), "PaymentMethodID", "Name", billPayment.PaymentMethodID);
                return View(billPayment);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: BillPayments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("BillPaymentID,SupplierID,PaymentDate,PaymentMethodID,ReferenceNo,AmountPaid")] BillPayment billPayment)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id != billPayment.BillPaymentID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(billPayment);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BillPaymentExists(billPayment.BillPaymentID))
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
                    Include(u => u.Suppliers).
                        ThenInclude(s => s.State)
                    .SingleOrDefaultAsync();

                var supplierList = user.Suppliers.Select(s => new { SupplierID = s.SupplierID, Description = s.FirstMidName + " " + s.LastName + ", " + s.State.Name });
                ViewData["SupplierID"] = new SelectList(supplierList, "SupplierID", "Description", billPayment.SupplierID);
                ViewData["PaymentMethodID"] = new SelectList(_context.PaymentMethod.OrderBy(pM => pM.Name), "PaymentMethodID", "Name", billPayment.PaymentMethodID);
                return View(billPayment);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: BillPayments/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await GetUserQueryable().
                    Include(u => u.Suppliers).
                        ThenInclude(s => s.BillPayments).
                            ThenInclude(bP => bP.PaymentMethod).
                    Include(u => u.Suppliers).
                        ThenInclude(s => s.State)
                    .SingleOrDefaultAsync();

                var billPayment = user.Suppliers.SelectMany(s => s.BillPayments).SingleOrDefault(bP => bP.BillPaymentID == id);
                if (billPayment == null)
                {
                    return NotFound();
                }

                return View(billPayment);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: BillPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserQueryable().
                     Include(u => u.Suppliers).
                         ThenInclude(s => s.BillPayments)
                     .SingleOrDefaultAsync();

                var billPayment = user.Suppliers.SelectMany(s => s.BillPayments).SingleOrDefault(bP => bP.BillPaymentID == id);
                if (billPayment == null)
                {
                    return NotFound();
                }

                _context.BillPayment.Remove(billPayment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        #region Helpers

        private bool BillPaymentExists(long id)
        {
            return _context.BillPayment.Any(e => e.BillPaymentID == id);
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
