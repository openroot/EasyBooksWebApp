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
    public class InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserQueryable().
                    Include(u => u.Customers).
                        ThenInclude(c => c.Invoices)
                    .SingleOrDefaultAsync();
                
                var invoices = user.Customers.SelectMany(c => c.Invoices).ToList();
                return View(invoices);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: Invoices/Details/5
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
                            ThenInclude(c => c.Invoices).
                        Include(u => u.Customers).
                            ThenInclude(c => c.State)
                        .SingleOrDefaultAsync();
                
                var invoice = user.Customers.SelectMany(c => c.Invoices).SingleOrDefault(i => i.InvoiceID == id);
                if (invoice == null)
                {
                    return NotFound();
                }

                return View(invoice);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: Invoices/Create
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
                return View();
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InvoiceID,CustomerID,InvoiceNo,InvoiceDate,DueDate,Memo,TotalAmount")] Invoice invoice)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(invoice);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                var user = await GetUserQueryable().
                        Include(u => u.Customers).
                            ThenInclude(c => c.State)
                        .SingleOrDefaultAsync();

                var customerList = user.Customers.Select(c => new { CustomerID = c.CustomerID, Description = c.FirstMidName + " " + c.LastName + ", " + c.State.Name });
                ViewData["CustomerID"] = new SelectList(_context.Customer, "CustomerID", "Description", invoice.CustomerID);
                return View(invoice);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: Invoices/Edit/5
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
                            ThenInclude(c => c.Invoices).
                        Include(u => u.Customers).
                            ThenInclude(c => c.State)
                        .SingleOrDefaultAsync();

                var invoice = user.Customers.SelectMany(c => c.Invoices).SingleOrDefault(i => i.InvoiceID == id);
                if (invoice == null)
                {
                    return NotFound();
                }
                var customerList = user.Customers.Select(c => new { CustomerID = c.CustomerID, Description = c.FirstMidName + " " + c.LastName + ", " + c.State.Name });
                ViewData["CustomerID"] = new SelectList(customerList, "CustomerID", "Description");
                return View(invoice);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: Invoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("InvoiceID,CustomerID,InvoiceNo,InvoiceDate,DueDate,Memo,TotalAmount")] Invoice invoice)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id != invoice.InvoiceID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(invoice);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!InvoiceExists(invoice.InvoiceID))
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
                ViewData["CustomerID"] = new SelectList(customerList, "CustomerID", "Description", invoice.CustomerID);
                return View(invoice);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: Invoices/Delete/5
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
                            ThenInclude(c => c.Invoices).
                        Include(u => u.Customers).
                            ThenInclude(c => c.State)
                        .SingleOrDefaultAsync();
                
                var invoice = user.Customers.SelectMany(c => c.Invoices).SingleOrDefault(i => i.InvoiceID == id);
                if (invoice == null)
                {
                    return NotFound();
                }

                return View(invoice);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserQueryable().
                        Include(u => u.Customers).
                            ThenInclude(c => c.Invoices)
                        .SingleOrDefaultAsync();

                var invoice = user.Customers.SelectMany(c => c.Invoices).SingleOrDefault(i => i.InvoiceID == id);
                if (invoice == null)
                {
                    return NotFound();
                }

                _context.Invoice.Remove(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        #region Helpers

        private bool InvoiceExists(long id)
        {
            return _context.Invoice.Any(e => e.InvoiceID == id);
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
