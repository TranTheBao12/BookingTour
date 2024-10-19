using Microsoft.AspNetCore.Mvc;

namespace BookingTour.AdminController
{
    public class QuanLyDanhMucController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
