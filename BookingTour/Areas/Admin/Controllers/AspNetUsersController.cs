using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookingTour.Data;
using BookingTour.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookingTour.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = CD.Role_Admin)]
    public class AspNetUsersController : Controller
    {
        private readonly YourExistingDbContextName _context;

        public AspNetUsersController(YourExistingDbContextName context)
        {
            _context = context;
        }

        // GET: Admin/AspNetUsers
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10; // Số bản ghi mỗi trang

            // Lấy danh sách người dùng
            var query = _context.AspNetUsers.OrderBy(u => u.UserName);

            // Tính tổng số bản ghi
            var totalUsers = await query.CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            // Lấy các bản ghi cho trang hiện tại
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Gửi thông tin đến view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            return View(await _context.AspNetUsers.ToListAsync());
        }

        // GET: Admin/AspNetUsers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aspNetUser = await _context.AspNetUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aspNetUser == null)
            {
                return NotFound();
            }

            return View(aspNetUser);
        }

        // GET: Admin/AspNetUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AspNetUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {

                _context.Add(aspNetUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aspNetUser);
        }

        // GET: Admin/AspNetUsers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aspNetUser = await _context.AspNetUsers.FindAsync(id);
            if (aspNetUser == null)
            {
                return NotFound();
            }
            return View(aspNetUser);
        }

        // POST: Admin/AspNetUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,fullname,Address,Age")] AspNetUser aspNetUser)
        {
            if (id != aspNetUser.Id)
            {
                return NotFound();
            }
            Console.WriteLine($"Fullname: {aspNetUser.fullname}");
            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm người dùng trong cơ sở dữ liệu
                    var userToUpdate = await _context.AspNetUsers.FindAsync(id);
                    if (userToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các thuộc tính cần thiết
                    userToUpdate.UserName = aspNetUser.UserName;
                    userToUpdate.NormalizedUserName = aspNetUser.NormalizedUserName;
                    userToUpdate.Email = aspNetUser.Email;
                    userToUpdate.NormalizedEmail = aspNetUser.NormalizedEmail;
                    userToUpdate.EmailConfirmed = aspNetUser.EmailConfirmed;
                    userToUpdate.PhoneNumber = aspNetUser.PhoneNumber;
                    userToUpdate.PhoneNumberConfirmed = aspNetUser.PhoneNumberConfirmed;
                    userToUpdate.fullname = aspNetUser.fullname; // Cập nhật fullname
                    userToUpdate.Address = aspNetUser.Address; // Cập nhật Address
                    userToUpdate.Age = aspNetUser.Age; // Cập nhật Age

                    // Cập nhật người dùng
                    _context.Update(userToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AspNetUserExists(aspNetUser.Id))
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
            return View(aspNetUser);
        }

        // GET: Admin/AspNetUsers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aspNetUser = await _context.AspNetUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aspNetUser == null)
            {
                return NotFound();
            }

            return View(aspNetUser);
        }

        // POST: Admin/AspNetUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var aspNetUser = await _context.AspNetUsers.FindAsync(id);
            if (aspNetUser != null)
            {
                _context.AspNetUsers.Remove(aspNetUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AspNetUserExists(string id)
        {
            return _context.AspNetUsers.Any(e => e.Id == id);
        }
        public async Task<IActionResult> LockAccount(string id)
        {
            var user = await _context.AspNetUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.LockoutEnd = DateTimeOffset.MaxValue; // Khóa tài khoản
            user.LockoutEnabled = true;

            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UnlockAccount(string id)
        {
            var user = await _context.AspNetUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.LockoutEnd = null; // Mở khóa tài khoản
            user.LockoutEnabled = false;

            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
