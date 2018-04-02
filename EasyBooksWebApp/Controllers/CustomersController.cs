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

namespace EasyBooksWebApp.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _customerImagePath = "\\uploads\\customers\\images";

        public CustomersController(ApplicationDbContext context, IHostingEnvironment appEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _userManager = userManager;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserQueryable()
                    .Include(u => u.Customers)
                        .ThenInclude(c => c.State)
                    .Include(u => u.Customers)
                        .ThenInclude(c => c.Invoices)
                    .Include(u => u.Customers)
                        .ThenInclude(c => c.ReceivePayments)
                    .SingleOrDefaultAsync();
                
                return View(user.Customers);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await GetUserQueryable()
                    .Include(u => u.Customers)
                        .ThenInclude(c => c.State)
                    .Include(u => u.Customers)
                        .ThenInclude(c => c.Invoices)
                    .Include(u => u.Customers)
                        .ThenInclude(c => c.ReceivePayments)
                    .SingleOrDefaultAsync();

                var customer = user.Customers.SingleOrDefault(c => c.CustomerID == id);
                if (customer == null)
                {
                    return NotFound();
                }

                ViewData["TotalDue"] = customer.Invoices.Select(i => i.TotalAmount).Sum() - customer.ReceivePayments.Select(rP => rP.AmountReceived).Sum();
                return View(customer);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: Customers/Create
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

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerID,UserId,FirstMidName,LastName,Phone,Email,Street,City,StateID,Pin,Notes,ProfileImageName")] Customer customer)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    var file = HttpContext.Request.Form.Files.Any() ? HttpContext.Request.Form.Files.Last() : null;
                    if (file != null && file.Length > 0)
                    {
                        var imageUploadPath = Path.Combine(_appEnvironment.WebRootPath + _customerImagePath);
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLower();
                        try
                        {
                            using (var fileStream = new FileStream(Path.Combine(imageUploadPath, fileName), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                customer.ProfileImageName = fileName;
                            }
                        }
                        catch (Exception ex) { }
                    }

                    _context.Add(customer);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["StateID"] = new SelectList(_context.State.OrderBy(s => s.Name), "StateID", "Name", customer.StateID);
                ViewData["UserId"] = _userManager.GetUserId(User);
                return View(customer);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await GetUserQueryable()
                    .Include(u => u.Customers)
                    .SingleOrDefaultAsync();

                var customer = user.Customers.SingleOrDefault(c => c.CustomerID == id);
                if (customer == null)
                {
                    return NotFound();
                }
                ViewData["StateID"] = new SelectList(_context.State.OrderBy(s => s.Name), "StateID", "Name", customer.StateID);
                ViewData["UserId"] = _userManager.GetUserId(User);
                return View(customer);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("CustomerID,UserId,FirstMidName,LastName,Phone,Email,Street,City,StateID,Pin,Notes,ProfileImageName")] Customer customer)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id != customer.CustomerID)
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
                            var imageUploadPath = Path.Combine(_appEnvironment.WebRootPath + _customerImagePath);
                            // Delete existing image file (if any)
                            if (!string.IsNullOrEmpty(customer.ProfileImageName))
                            {
                                try
                                {
                                    System.IO.File.Delete(Path.Combine(imageUploadPath, customer.ProfileImageName));
                                }
                                catch (Exception ex) { }
                            }
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLower();
                            try
                            {
                                using (var fileStream = new FileStream(Path.Combine(imageUploadPath, fileName), FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                    customer.ProfileImageName = fileName;
                                }
                            }
                            catch (Exception ex) { }
                        }

                        _context.Update(customer);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CustomerExists(customer.CustomerID))
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
                ViewData["StateID"] = new SelectList(_context.State.OrderBy(s => s.Name), "StateID", "Name", customer.StateID);
                ViewData["UserId"] = _userManager.GetUserId(User);
                return View(customer);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await GetUserQueryable()
                    .Include(u => u.Customers)
                        .ThenInclude(c => c.State)
                    .SingleOrDefaultAsync();

                var customer = user.Customers.SingleOrDefault(c => c.CustomerID == id);
                if (customer == null)
                {
                    return NotFound();
                }

                return View(customer);
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserQueryable()
                    .Include(u => u.Customers)
                        .ThenInclude(c => c.State)
                    .SingleOrDefaultAsync();

                var customer = user.Customers.SingleOrDefault(c => c.CustomerID == id);
                if (customer == null)
                {
                    return NotFound();
                }

                var imageUploadPath = Path.Combine(_appEnvironment.WebRootPath + _customerImagePath);
                // Delete existing image file (if any)
                if (!string.IsNullOrEmpty(customer.ProfileImageName))
                {
                    try
                    {
                        System.IO.File.Delete(Path.Combine(imageUploadPath, customer.ProfileImageName));
                    }
                    catch (Exception ex) { }
                }

                _context.Customer.Remove(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        #region Helpers

        private bool CustomerExists(long id)
        {
            return _context.Customer.Any(e => e.CustomerID == id);
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
