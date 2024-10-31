using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookingTour.Data;
using BookingTour.Models;

namespace BookingTour.Controllers
{
    public class ToursController : Controller
    {
        private readonly YourExistingDbContextName _context;

        public ToursController(YourExistingDbContextName context)
        {
            _context = context;
        }

        // GET: Tours
        public async Task<IActionResult> Index()
        {
            var yourExistingDbContextName = _context.Tours.Include(t => t.IdHotelNavigation).Include(t => t.IdTransNavigation).Include(t => t.IdTypeNavigation);
            ViewBag.PopularTours = await _context.Tours
       .Include(t => t.IdHotelNavigation)
       //.Where(t => !t.IsDelete.Equals("true")) // Chỉ lấy tour chưa bị xóa
       //.OrderByDescending(t => t.Price) // Sắp xếp theo giá (ví dụ)
       //.Take(4) // Lấy 4 tour phổ biến
       .ToListAsync();
            return View(await yourExistingDbContextName.ToListAsync());
        }

        // GET: Tours/Details/5
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

        // GET: Tours/Create
        public IActionResult Create()
        {
            ViewData["IdHotel"] = new SelectList(_context.Hotels, "IdHotel", "IdHotel");
            ViewData["IdTrans"] = new SelectList(_context.Transportations, "IdTrans", "IdTrans");
            ViewData["IdType"] = new SelectList(_context.TypeOfTours, "IdType", "IdType");
            return View();
        }

        // POST: Tours/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTour,Name,Description,Image,StartDate,EndDate,MaxQuantity,Price,IsDelete,IdType,IdTrans,IdHotel,ApprovalStatus")] Tour tour)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdHotel"] = new SelectList(_context.Hotels, "IdHotel", "IdHotel", tour.IdHotel);
            ViewData["IdTrans"] = new SelectList(_context.Transportations, "IdTrans", "IdTrans", tour.IdTrans);
            ViewData["IdType"] = new SelectList(_context.TypeOfTours, "IdType", "IdType", tour.IdType);
            return View(tour);
        }

        // GET: Tours/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
            {
                return NotFound();
            }
            ViewData["IdHotel"] = new SelectList(_context.Hotels, "IdHotel", "IdHotel", tour.IdHotel);
            ViewData["IdTrans"] = new SelectList(_context.Transportations, "IdTrans", "IdTrans", tour.IdTrans);
            ViewData["IdType"] = new SelectList(_context.TypeOfTours, "IdType", "IdType", tour.IdType);
            return View(tour);
        }

        // POST: Tours/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("IdTour,Name,Description,Image,StartDate,EndDate,MaxQuantity,Price,IsDelete,IdType,IdTrans,IdHotel,ApprovalStatus")] Tour tour)
        {
            if (id != tour.IdTour)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TourExists(tour.IdTour))
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
            ViewData["IdHotel"] = new SelectList(_context.Hotels, "IdHotel", "IdHotel", tour.IdHotel);
            ViewData["IdTrans"] = new SelectList(_context.Transportations, "IdTrans", "IdTrans", tour.IdTrans);
            ViewData["IdType"] = new SelectList(_context.TypeOfTours, "IdType", "IdType", tour.IdType);
            return View(tour);
        }

        // GET: Tours/Delete/5
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

        // POST: Tours/Delete/5
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
