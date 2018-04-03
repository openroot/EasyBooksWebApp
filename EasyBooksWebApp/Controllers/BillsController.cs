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
    public class BillsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bills
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserQueryable().
                    Include(u => u.Suppliers).
                        ThenInclude(s => s.Bills)
                    .SingleOrDefaultAsync();

                var bills = user.Suppliers.SelectMany(c => c.Bills).ToList();
                return View(bills);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: Bills/Details/5
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
                        ThenInclude(s => s.Bills).
                    Include(u => u.Suppliers).
                        ThenInclude(s => s.State)
                    .SingleOrDefaultAsync();

                var bill = user.Suppliers.SelectMany(c => c.Bills).SingleOrDefault(b => b.BillID == id);
                if (bill == null)
                {
                    return NotFound();
                }

                return View(bill);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: Bills/Create
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
                return View();
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: Bills/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BillID,SupplierID,BillNo,BillDate,DueDate,Memo,TotalAmount")] Bill bill)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(bill);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                var user = await GetUserQueryable().
                    Include(u => u.Suppliers).
                        ThenInclude(s => s.State)
                    .SingleOrDefaultAsync();

                var supplierList = user.Suppliers.Select(s => new { SupplierID = s.SupplierID, Description = s.FirstMidName + " " + s.LastName + ", " + s.State.Name });
                ViewData["SupplierID"] = new SelectList(supplierList, "SupplierID", "Description", bill.SupplierID);
                return View(bill);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: Bills/Edit/5
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
                        ThenInclude(s => s.Bills).
                    Include(u => u.Suppliers).
                        ThenInclude(s => s.State)
                    .SingleOrDefaultAsync();

                var bill = user.Suppliers.SelectMany(c => c.Bills).SingleOrDefault(b => b.BillID == id);
                if (bill == null)
                {
                    return NotFound();
                }
                var supplierList = user.Suppliers.Select(s => new { SupplierID = s.SupplierID, Description = s.FirstMidName + " " + s.LastName + ", " + s.State.Name });
                ViewData["SupplierID"] = new SelectList(supplierList, "SupplierID", "Description", bill.SupplierID);
                return View(bill);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: Bills/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("BillID,SupplierID,BillNo,BillDate,DueDate,Memo,TotalAmount")] Bill bill)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id != bill.BillID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(bill);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BillExists(bill.BillID))
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
                ViewData["SupplierID"] = new SelectList(supplierList, "SupplierID", "Description", bill.SupplierID);
                return View(bill);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: Bills/Delete/5
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
                        ThenInclude(s => s.Bills).
                    Include(u => u.Suppliers).
                        ThenInclude(s => s.State)
                    .SingleOrDefaultAsync();

                var bill = user.Suppliers.SelectMany(c => c.Bills).SingleOrDefault(b => b.BillID == id);
                if (bill == null)
                {
                    return NotFound();
                }

                return View(bill);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserQueryable().
                    Include(u => u.Suppliers).
                        ThenInclude(s => s.Bills)
                    .SingleOrDefaultAsync();

                var bill = user.Suppliers.SelectMany(c => c.Bills).SingleOrDefault(b => b.BillID == id);
                if (bill == null)
                {
                    return NotFound();
                }

                _context.Bill.Remove(bill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        #region Helpers

        private bool BillExists(long id)
        {
            return _context.Bill.Any(e => e.BillID == id);
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
