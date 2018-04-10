using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyBooksWebApp.Data;
using EasyBooksWebApp.Models;
using Serilog;

namespace EasyBooksWebApp.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _supplierImagePath = "\\uploads\\suppliers\\images";

        public SuppliersController(ApplicationDbContext context, IHostingEnvironment appEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _userManager = userManager;
        }

        // GET: Suppliers
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserQueryable()
                    .Include(u => u.Suppliers)
                        .ThenInclude(s => s.State)
                    .Include(u => u.Suppliers)
                        .ThenInclude(s => s.Bills)
                    .Include(u => u.Suppliers)
                        .ThenInclude(s => s.BillPayments)
                    .SingleOrDefaultAsync();

                return View(user.Suppliers);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await GetUserQueryable()
                    .Include(u => u.Suppliers)
                        .ThenInclude(s => s.State)
                    .Include(u => u.Suppliers)
                        .ThenInclude(s => s.Bills)
                    .Include(u => u.Suppliers)
                        .ThenInclude(s => s.BillPayments)
                    .SingleOrDefaultAsync();

                var supplier = user.Suppliers.SingleOrDefault(s => s.SupplierID == id);
                if (supplier == null)
                {
                    return NotFound();
                }

                ViewData["TotalPayable"] = supplier.Bills.Select(b => b.TotalAmount).Sum() - supplier.BillPayments.Select(bP => bP.AmountPaid).Sum();
                return View(supplier);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["StateID"] = new SelectList(_context.State.OrderBy(s => s.Name), "StateID", "Name");
                ViewData["UserId"] = _userManager.GetUserId(User);
                return View();
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SupplierID,UserId,FirstMidName,LastName,Company,Phone,Email,Street,City,StateID,Pin,Notes,ProfileImageName")] Supplier supplier)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    var file = HttpContext.Request.Form.Files.Any() ? HttpContext.Request.Form.Files.Last() : null;
                    if (file != null && file.Length > 0)
                    {
                        var imageUploadPath = Path.Combine(_appEnvironment.WebRootPath + _supplierImagePath);
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLower();
                        try
                        {
                            using (var fileStream = new FileStream(Path.Combine(imageUploadPath, fileName), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                supplier.ProfileImageName = fileName;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Supplier profile image upload error at Create().");
                        }
                    }

                    _context.Add(supplier);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["StateID"] = new SelectList(_context.State.OrderBy(s => s.Name), "StateID", "Name", supplier.StateID);
                ViewData["UserId"] = _userManager.GetUserId(User);
                return View(supplier);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await GetUserQueryable()
                    .Include(u => u.Suppliers)
                    .SingleOrDefaultAsync();

                var supplier = user.Suppliers.SingleOrDefault(s => s.SupplierID == id);
                if (supplier == null)
                {
                    return NotFound();
                }
                ViewData["StateID"] = new SelectList(_context.State, "StateID", "Name", supplier.StateID);
                ViewData["UserId"] = _userManager.GetUserId(User);
                return View(supplier);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("SupplierID,UserId,FirstMidName,LastName,Company,Phone,Email,Street,City,StateID,Pin,Notes,ProfileImageName")] Supplier supplier)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id != supplier.SupplierID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var file = HttpContext.Request.Form.Files.Any() ? HttpContext.Request.Form.Files.Last() : null;
                        if (file != null && file?.Length > 0)
                        {
                            var imageUploadPath = Path.Combine(_appEnvironment.WebRootPath + _supplierImagePath);
                            // Delete existing image file (if any)
                            if (!string.IsNullOrEmpty(supplier.ProfileImageName))
                            {
                                try
                                {
                                    System.IO.File.Delete(Path.Combine(imageUploadPath, supplier.ProfileImageName));
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex, "Supplier profile image delete error at Edit().");
                                }
                            }
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLower();
                            try
                            {
                                using (var fileStream = new FileStream(Path.Combine(imageUploadPath, fileName), FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                    supplier.ProfileImageName = fileName;
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "Supplier profile image upload error at Edit().");
                            }
                        }

                        _context.Update(supplier);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!SupplierExists(supplier.SupplierID))
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
                ViewData["StateID"] = new SelectList(_context.State, "StateID", "Name", supplier.StateID);
                ViewData["UserId"] = _userManager.GetUserId(User);
                return View(supplier);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await GetUserQueryable()
                    .Include(u => u.Suppliers)
                        .ThenInclude(c => c.State)
                    .SingleOrDefaultAsync();

                var supplier = user.Suppliers.SingleOrDefault(s => s.SupplierID == id);
                if (supplier == null)
                {
                    return NotFound();
                }

                return View(supplier);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserQueryable()
                    .Include(u => u.Suppliers)
                        .ThenInclude(c => c.State)
                    .SingleOrDefaultAsync();

                var supplier = user.Suppliers.SingleOrDefault(s => s.SupplierID == id);
                if (supplier == null)
                {
                    return NotFound();
                }

                var imageUploadPath = Path.Combine(_appEnvironment.WebRootPath + _supplierImagePath);
                // Delete existing image file (if any)
                if (!string.IsNullOrEmpty(supplier.ProfileImageName))
                {
                    try
                    {
                        System.IO.File.Delete(Path.Combine(imageUploadPath, supplier.ProfileImageName));
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Supplier profile image delete error at DeleteConfirmed().");
                    }
                }

                _context.Supplier.Remove(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        #region Helpers

        private bool SupplierExists(long id)
        {
            return _context.Supplier.Any(e => e.SupplierID == id);
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
