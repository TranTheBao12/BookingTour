using Microsoft.AspNetCore.Mvc;

namespace BookingTour.AdminController
{
    public class QuanLyTaiKhoanController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
