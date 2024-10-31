using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookingTour.Data;
using BookingTour.Models;
using X.PagedList;
using X.PagedList.Extensions;

namespace BookingTour.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly YourExistingDbContextName _context;

        public CategoryController(YourExistingDbContextName context)
        {
            _context = context;
        }

        // GET: Admin/Category
        public async Task<IActionResult> Index(int page = 1)
        {
            if (!User.IsInRole("ADMIN"))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            const int pageSize = 10; // Số bản ghi mỗi trang

            // Lấy danh sách tour có trạng thái "Chờ Duyệt"
            var query = _context.Tours
                .Include(t => t.IdHotelNavigation)
                .Include(t => t.IdTransNavigation)
                .Include(t => t.IdTypeNavigation);

            // Tính tổng số bản ghi
            var totalTours = await query.CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalTours / (double)pageSize);

            // Lấy các bản ghi cho trang hiện tại
            var tours = await query
                .OrderBy(t => t.IdTour) // Thay đổi theo thuộc tính bạn muốn sắp xếp
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Gửi thông tin đến view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize; // Gửi pageSize đến view

            return View(tours);
        }
        // GET: Admin/Category/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .Include(t => t.IdHotelNavigation)
                .Include(t => t.IdTransNavigation)
                .Include(t => t.IdTypeNavigation)
                .FirstOrDefaultAsync(m => m.IdTour == id);
            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        // GET: Admin/Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] TypeOfTour typeOfTour)
        {
            if (ModelState.IsValid)
            {
                var newtypeoftour = new TypeOfTour { 
                    Name = typeOfTour.Name,
                    Description = typeOfTour.Description,
                };
                newtypeoftour.IdType = _context.TypeOfTours.Max(p => p.IdType) + 1;
                _context.Add(newtypeoftour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(typeOfTour);
        }

        // GET: Admin/Category/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tourType = await _context.TypeOfTours.FindAsync(id);
            if (tourType == null)
            {
                return NotFound();
            }

            return View(tourType);
        }

        // POST: Admin/Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("IdType,Name,Description")] TypeOfTour tourType)
        {
            if (id != tourType.IdType)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm loại tour hiện tại
                    var existingTourType = await _context.TypeOfTours.FindAsync(id);
                    if (existingTourType == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin loại tour
                    existingTourType.Name = tourType.Name;
                    existingTourType.Description = tourType.Description;

                    // Nếu bạn muốn cập nhật các tour liên quan, thực hiện ở đây
                    var relatedTours = await _context.Tours.Where(t => t.IdType == id).ToListAsync();
                    foreach (var relatedTour in relatedTours)
                    {
                        // Ví dụ: Có thể cập nhật giá trị nào đó trong tour nếu cần
                        // relatedTour.SomeProperty = newValue; 
                        // Hoặc để nguyên nếu không cần thay đổi
                    }

                    // Lưu thay đổi
                    _context.Update(existingTourType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypeOfTourExists(tourType.IdType))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Category", new { area = "Admin" });
            }

            return View(tourType);
        }

        // Helper method to check if TypeOfTour exists
        private bool TypeOfTourExists(long id)
        {
            return _context.TypeOfTours.Any(e => e.IdType == id);
        }

   

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour != null)
            {
                _context.Tours.Remove(tour);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TourExists(long id)
        {
            return _context.Tours.Any(e => e.IdTour == id);
        }
    }
}
