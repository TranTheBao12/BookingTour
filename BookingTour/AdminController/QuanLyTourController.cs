using Microsoft.AspNetCore.Mvc;

namespace BookingTour.AdminController
{
    public class QuanLyTourController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
