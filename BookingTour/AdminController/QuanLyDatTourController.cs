using Microsoft.AspNetCore.Mvc;

namespace BookingTour.AdminController
{
    public class QuanLyDatTourController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
