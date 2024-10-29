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
    [Area("ADMIN")]
    [Authorize(Roles = CD.Role_Admin)]
    public class BookingStatusController : Controller
    {
        private readonly YourExistingDbContextName _context;
        private readonly ApplicationDbContext _context1;

        public BookingStatusController(YourExistingDbContextName context, ApplicationDbContext context1)
        {
            _context = context;
            _context1 = context1;
        }

        // GET: BookingStatus
        public async Task<IActionResult> Index(int? page)
        {
            var pageSize = 10; // Số lượng booking mỗi trang
            var pageNumber = page ?? 1; // Nếu không có page thì mặc định là 1

            // Truy vấn các booking cùng thông tin liên quan
            var bookings = await (from booking in _context.Bookings
                                  join status in _context.BookingStatuses on booking.IdStatus equals status.IdStatus
                                  join tour in _context.Tours on booking.IdTour equals tour.IdTour
                                  join user in _context.AspNetUsers on booking.Id equals user.Id // Thay đổi nếu tên thuộc tính không khớp
                                  select new BookingViewModel
                                  {
                                      CustomerName = user.UserName, // Giả định tên khách hàng lưu trong UserName
                                      TourName = tour.Name, // Thay thế bằng thuộc tính đúng từ model Tour
                                      BookingDate = booking.BookingTime ?? DateTime.Now, // Nếu không có BookingTime thì dùng thời gian hiện tại
                                      StatusName = status.StatusName,
                                      IdStatus = status.IdStatus
                                  }).ToListAsync();

            var totalCount = bookings.Count(); // Tổng số booking
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize); // Tính tổng số trang

            // Lấy các booking theo trang
            var pagedBookings = bookings.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Thiết lập các giá trị ViewBag
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;

            return View(pagedBookings); // Trả về các booking đã phân trang
        }

        // GET: BookingStatus/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingStatus = await _context.BookingStatuses
                .FirstOrDefaultAsync(m => m.IdStatus == id);
            if (bookingStatus == null)
            {
                return NotFound();
            }

            return View(bookingStatus);
        }

        // GET: BookingStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BookingStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdStatus,StatusName,Description,CreatedAt")] BookingStatus bookingStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookingStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bookingStatus);
        }

        // GET: BookingStatus/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingStatus = await _context.BookingStatuses.FindAsync(id);
            if (bookingStatus == null)
            {
                return NotFound();
            }
            return View(bookingStatus);
        }

        // POST: BookingStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("IdStatus,StatusName,Description,CreatedAt")] BookingStatus bookingStatus)
        {
            if (id != bookingStatus.IdStatus)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookingStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingStatusExists(bookingStatus.IdStatus))
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
            return View(bookingStatus);
        }
        [HttpGet]
        // GET: BookingStatus/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingStatus = await _context.BookingStatuses
                .FirstOrDefaultAsync(m => m.IdStatus == id);
            if (bookingStatus == null)
            {
                return NotFound();
            }

            return View(bookingStatus);
        }

        // POST: BookingStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var bookings = await _context.Bookings.Where(b => b.IdStatus == id).ToListAsync();
            if (bookings.Any())
            {
                _context.Bookings.RemoveRange(bookings);
            }
            var bookingStatus = await _context.BookingStatuses.FindAsync(id);
            if (bookingStatus != null)
            {
                _context.BookingStatuses.Remove(bookingStatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingStatusExists(long id)
        {
            return _context.BookingStatuses.Any(e => e.IdStatus == id);
        }
    }
}
