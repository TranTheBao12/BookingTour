using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookingTour.Data;
using BookingTour.Models;

namespace BookingTour.Areas.Host.Controllers
{
    [Area("Host")]
    public class ToursManagerController : Controller
    {
        private readonly YourExistingDbContextName _context;
        private readonly AppDbContext _context1;
        public ToursManagerController(YourExistingDbContextName context, AppDbContext context1)
        {
            _context = context;
            _context1 = context1;
        }

        // GET: Host/ToursManager
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10; // Số bản ghi mỗi trang

            // Lấy danh sách tour có trạng thái "Chờ Duyệt" và IsDelete = "N"
            var query = _context.Tours
                .Include(t => t.IdHotelNavigation)
                .Include(t => t.IdTransNavigation)
                .Include(t => t.IdTypeNavigation)
                .Where(t => t.IsDelete == "N"); // Chỉ lấy tour có IsDelete = "N"

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

        // GET: Host/ToursManager/Details/5
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
            var images = await _context1.Images.Where(i => i.IdTour == tour.IdTour).ToListAsync();
            tour.images = images;
            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        // GET: Host/ToursManager/Create
        public IActionResult Create()
        {
            // Tạo danh sách chọn cho các thuộc tính IdHotel, IdTrans, và IdType
            ViewData["IdHotel"] = new SelectList(_context.Hotels, "IdHotel", "Name");
            ViewData["IdTrans"] = new SelectList(_context.Transportations, "IdTrans", "Name");
            ViewData["IdType"] = new SelectList(_context.TypeOfTours, "IdType", "Name");
            return View();
        }

        // POST: Host/ToursManager/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,StartDate,EndDate,MaxQuantity,Price,IsDelete,IdType,IdTrans,IdHotel,ApprovalStatus")] Tour tour, IFormFileCollection imageFiles)
        {
            if (ModelState.IsValid)
            {
                // Tính ID mới dựa trên ID lớn nhất hiện tại
                var maxId = await _context.Tours.MaxAsync(p => p.IdTour);
                tour.IdTour = maxId + 1; // Thiết lập ID mới
                tour.IsDelete = "N";
                tour.ApprovalStatus = "Chờ Duyệt";

                // Lưu tour vào YourExistingDbContextName
                _context.Add(tour);
                await _context.SaveChangesAsync(); // Lưu tour trước để lấy ID

                try
                {
                    // Xử lý upload nhiều file hình ảnh
                    if (imageFiles != null && imageFiles.Count > 0)
                    {
                        foreach (var imageFile in imageFiles)
                        {
                            if (imageFile.Length > 0)
                            {
                                var fileName = Path.GetFileName(imageFile.FileName);
                                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                                // Lưu hình ảnh vào thư mục wwwroot/images
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await imageFile.CopyToAsync(stream);
                                }

                                // Tạo đối tượng Image và thêm vào AppDbContext
                                var image = new Image
                                {
                                    ImageUrl = fileName,
                                    IdTour = tour.IdTour,
                                    CreatedAt = DateTime.Now // Đặt thời gian tạo
                                };

                                // Thêm hình ảnh vào AppDbContext
                                _context1.Images.Add(image);
                                Console.WriteLine($"Thêm hình ảnh: {image.ImageUrl} với ID Tour: {image.IdTour}");
                            }
                        }

                        // Lưu các hình ảnh
                        await _context1.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Thêm tour và hình ảnh thành công!";
                    }
                    else
                    {
                        Console.WriteLine("Không có hình ảnh nào được tải lên.");
                        TempData["WarningMessage"] = "Không có hình ảnh nào được tải lên.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi lưu hình ảnh: " + ex.Message;
                    Console.WriteLine($"Lỗi khi lưu hình ảnh: {ex.Message}");
                }
                return View(tour);
               /* return RedirectToAction(nameof(Index));*/ // Chuyển hướng về trang Index
            }

            // Nếu ModelState không hợp lệ, trả về các lựa chọn đã chọn
            ViewData["IdHotel"] = new SelectList(_context.Hotels, "IdHotel", "Name", tour.IdHotel);
            ViewData["IdTrans"] = new SelectList(_context.Transportations, "IdTrans", "Name", tour.IdTrans);
            ViewData["IdType"] = new SelectList(_context.TypeOfTours, "IdType", "Name", tour.IdType);

            return View(tour); // Trả về view với thông tin tour
        }





        // GET: Host/ToursManager/Edit/5
        // GET: Host/ToursManager/Edit/5
        public IActionResult Edit(long id)
        {
            var tour = _context.Tours.Find(id);
            if (tour == null)
            {
                return NotFound(); // Trả về 404 nếu tour không tồn tại
            }

            // Lấy danh sách hình ảnh từ context riêng
            var images = _context1.Images.Where(i => i.IdTour == tour.IdTour).ToList();

            ViewBag.Images = images; // Gán danh sách hình ảnh vào ViewBag

            ViewBag.TypeList = new SelectList(_context.TypeOfTours, "IdType", "Name", tour.IdType);
            ViewBag.TransportationList = new SelectList(_context.Transportations, "IdTrans", "Name", tour.IdTrans);
            ViewBag.HotelList = new SelectList(_context.Hotels, "IdHotel", "Name", tour.IdHotel);
            return View(tour);
        }

        // POST: Tour/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long id, [Bind("IdTour,Name,Description,StartDate,EndDate,MaxQuantity,Price,IsDelete,IdType,IdTrans,IdHotel,ApprovalStatus")] Tour tour, List<Image> images)
        {
            if (id != tour.IdTour)
            {
                return NotFound(); // Kiểm tra xem ID có khớp không
            }

            if (ModelState.IsValid) // Kiểm tra tính hợp lệ của model
            {
                try
                {
                    // Tìm bản ghi hiện tại trong cơ sở dữ liệu
                    var existingTour = _context.Tours.Find(id);
                    if (existingTour == null)
                    {
                        return NotFound(); // Nếu tour không tồn tại
                    }

                    // Cập nhật các thuộc tính của tour
                    existingTour.Name = tour.Name;
                    existingTour.Description = tour.Description;
                    existingTour.StartDate = tour.StartDate;
                    existingTour.EndDate = tour.EndDate;
                    existingTour.MaxQuantity = tour.MaxQuantity;
                    existingTour.Price = tour.Price;
                    existingTour.IsDelete = tour.IsDelete;
                    existingTour.IdType = tour.IdType;
                    existingTour.IdTrans = tour.IdTrans;
                    existingTour.IdHotel = tour.IdHotel;
                    existingTour.ApprovalStatus = tour.ApprovalStatus;

                    // Cập nhật danh sách hình ảnh
                    foreach (var image in images)
                    {
                        var existingImage = _context1.Images.Find(image.IdTour); // Lấy hình ảnh theo ID
                        if (existingImage != null)
                        {
                            existingImage.ImageUrl = image.ImageUrl; // Cập nhật URL
                            existingImage.CreatedAt = existingImage.CreatedAt; // Giữ nguyên CreatedAt
                        }
                    }

                    // Lưu các thay đổi vào cơ sở dữ liệu
                    _context.SaveChanges();
                    _context1.SaveChanges(); // Đừng quên lưu thay đổi cho context hình ảnh

                    return RedirectToAction(nameof(Index)); // Chuyển hướng về action Index
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TourExists(tour.IdTour))
                    {
                        return NotFound(); // Nếu tour không tồn tại
                    }
                    else
                    {
                        throw; // Nếu có lỗi khác
                    }
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu dữ liệu: " + ex.Message);
                }
            }

            // Ghi lại lỗi ModelState để debug nếu có
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            ViewBag.TypeList = new SelectList(_context.TypeOfTours, "IdType", "Name", tour.IdType);
            ViewBag.TransportationList = new SelectList(_context.Transportations, "IdTrans", "Name", tour.IdTrans);
            ViewBag.HotelList = new SelectList(_context.Hotels, "IdHotel", "Name", tour.IdHotel);

            // Lấy lại danh sách hình ảnh
            var images1 = _context1.Images.Where(i => i.IdTour == tour.IdTour).ToList();
            ViewBag.Images = images;

            return View(tour); // Nếu không hợp lệ, hiển thị lại form với lỗi
        }



        // Phương thức riêng để lấy dữ liệu cho dropdown
        private async Task<IEnumerable<SelectListItem>> GetTourTypes()
        {
            return new SelectList(await _context.TypeOfTours.ToListAsync(), "IdType", "Name");
        }

        private async Task<IEnumerable<SelectListItem>> GetTransportations()
        {
            return new SelectList(await _context.Transportations.ToListAsync(), "IdTrans", "Name");
        }

        private async Task<IEnumerable<SelectListItem>> GetHotels()
        {
            return new SelectList(await _context.Hotels.ToListAsync(), "IdHotel", "Name");
        }

        // GET: Host/ToursManager/Delete/5
        public async Task<IActionResult> Delete(long? id)
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

        // POST: Host/ToursManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour != null)
            {
                _context.Tours.Remove(tour);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TourExists(long id)
        {
            return _context.Tours.Any(e => e.IdTour == id);
        }
    }
}
