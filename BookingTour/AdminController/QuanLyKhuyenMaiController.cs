using Microsoft.AspNetCore.Mvc;

namespace BookingTour.AdminController
{
    public class QuanLyKhuyenMaiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
