using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookingTour.Data;
using BookingTour.Models;
using SQLitePCL;

namespace BookingTour.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PromotionsController : Controller
    {
        private readonly YourExistingDbContextName _context;

        public PromotionsController(YourExistingDbContextName context)
        {
            _context = context;
        }

        // GET: Admin/Promotions
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            // Tính tổng số bản ghi
            var totalPromotions = await _context.Promotions.CountAsync();

            // Tính số trang
            var totalPages = (int)Math.Ceiling((double)totalPromotions / pageSize);

            // Lấy dữ liệu với phân trang
            var promotions = await _context.Promotions
                .Include(p => p.IdHotelNavigation)
                .Include(p => p.IdTourNavigation)
                .OrderBy(p => p.StartDate) // Sắp xếp theo ngày bắt đầu (hoặc thuộc tính bạn muốn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Thiết lập ViewBag cho phân trang
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(promotions);
        }

        // GET: Admin/Promotions/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotion = await _context.Promotions
                .Include(p => p.IdHotelNavigation)
                .Include(p => p.IdTourNavigation)
                .FirstOrDefaultAsync(m => m.IdPmt == id);
            if (promotion == null)
            {
                return NotFound();
            }

            return View(promotion);
        }

        // GET: Admin/Promotions/Create
        public IActionResult Create()
        {
            ViewData["IdHotel"] = new SelectList(_context.Hotels, "IdHotel", "IdHotel");
            ViewData["IdTour"] = new SelectList(_context.Tours, "IdTour", "IdTour");
            return View();
        }

        // POST: Admin/Promotions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,StartDate,EndDate,DiscountAmount,EligibilityCriteria,Status,IdTour,IdHotel")] Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                // Tạo một đối tượng Promotion mới
                Promotion newPromotion = new Promotion
                {
                    Name = promotion.Name,
                    Description = promotion.Description,
                    StartDate = promotion.StartDate,
                    EndDate = promotion.EndDate,
                    DiscountAmount = promotion.DiscountAmount,
                    EligibilityCriteria = promotion.EligibilityCriteria,
                    Status = promotion.Status,
                    IdTour = promotion.IdTour,
                    IdHotel = promotion.IdHotel
                };

                // Thêm vào cơ sở dữ liệu
                _context.Add(newPromotion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Chuyển hướng về danh sách sau khi tạo thành công
            }

            // Nếu ModelState không hợp lệ, hiển thị lại view với thông tin đã nhập
            ViewData["IdHotel"] = new SelectList(_context.Hotels, "IdHotel", "Name", promotion.IdHotel);
            ViewData["IdTour"] = new SelectList(_context.Tours, "IdTour", "Name", promotion.IdTour);
            return View(promotion);
        }


        // GET: Admin/Promotions/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotion = await _context.Promotions
                .Include(p => p.IdHotelNavigation)
                .Include(p => p.IdTourNavigation)
                .FirstOrDefaultAsync(p => p.IdPmt == id);

            if (promotion == null)
            {
                return NotFound();
            }

            // Gửi danh sách Hotels và Tours cho view để hiển thị trong dropdown
            ViewData["IdHotel"] = new SelectList(_context.Hotels, "IdHotel", "HotelName", promotion.IdHotel);
            ViewData["IdTour"] = new SelectList(_context.Tours, "IdTour", "TourName", promotion.IdTour);
            return View(promotion);
        }

        // POST: Promotion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("IdPmt,Name,Description,StartDate,EndDate,DiscountAmount,EligibilityCriteria,Status,IdHotel,IdTour")] Promotion promotion)
        {
            if (id != promotion.IdPmt)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm Promotion trong cơ sở dữ liệu
                    var promotionToUpdate = await _context.Promotions.FindAsync(id);
                    if (promotionToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các thuộc tính
                    promotionToUpdate.Name = promotion.Name;
                    promotionToUpdate.Description = promotion.Description;
                    promotionToUpdate.StartDate = promotion.StartDate;
                    promotionToUpdate.EndDate = promotion.EndDate;
                    promotionToUpdate.DiscountAmount = promotion.DiscountAmount;
                    promotionToUpdate.EligibilityCriteria = promotion.EligibilityCriteria;
                    promotionToUpdate.Status = promotion.Status;
                    promotionToUpdate.IdHotel = promotionToUpdate.IdHotel;
                    promotionToUpdate.IdTour = promotionToUpdate.IdTour;
                    promotionToUpdate.IdPmt = promotionToUpdate.IdPmt;
                    _context.Update(promotionToUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromotionExists(promotion.IdPmt))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Nếu có lỗi, nạp lại danh sách Hotels và Tours để hiển thị lại
            ViewData["IdHotel"] = new SelectList(_context.Hotels, "IdHotel", "HotelName", promotion.IdHotel);
            ViewData["IdTour"] = new SelectList(_context.Tours, "IdTour", "TourName", promotion.IdTour);
            return View(promotion);
        }


        // GET: Admin/Promotions/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            // Kiểm tra xem id có null hay không
            if (id == null)
            {
                return NotFound(); // Trả về NotFound nếu không có id
            }

            // Tìm khuyến mãi theo id
            var promotion = await _context.Promotions
                .Include(p => p.IdHotelNavigation) // Bao gồm thông tin khách sạn nếu cần
                .Include(p => p.IdTourNavigation) // Bao gồm thông tin tour nếu cần
                .FirstOrDefaultAsync(m => m.IdPmt == id); // Tìm khuyến mãi đầu tiên có IdPmt bằng id

            // Kiểm tra xem khuyến mãi có tồn tại không
            if (promotion == null)
            {
                return NotFound(); // Trả về NotFound nếu không tìm thấy khuyến mãi
            }

            return View(promotion); // Trả về view để hiển thị thông tin khuyến mãi
        }


        // POST: Admin/Promotions/Delete/5
        // POST: Admin/Promotions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            // Tìm khuyến mãi theo id
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion != null)
            {
                // Tìm tất cả các bản ghi trong bảng PROMOTION_USER có ID_PMT trùng với khuyến mãi
                var relatedPromotions = await _context.Promotions
                    .Where(pu => pu.IdPmt == id) // Lọc theo ID_PMT
                    .ToListAsync();

                // Nếu tìm thấy các bản ghi liên quan, xóa chúng
                if (relatedPromotions.Any())
                {
                    _context.Promotions.RemoveRange(relatedPromotions); // Xóa các bản ghi liên quan
                }

                // Cuối cùng, xóa khuyến mãi
                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync(); // Lưu các thay đổi vào cơ sở dữ liệu
            }

            return RedirectToAction(nameof(Index)); // Chuyển hướng về trang danh sách
        }



        private bool PromotionExists(long id)
        {
            return _context.Promotions.Any(e => e.IdPmt == id);
        }
    }
}
