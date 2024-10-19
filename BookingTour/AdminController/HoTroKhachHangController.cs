using Microsoft.AspNetCore.Mvc;

namespace BookingTour.AdminController
{
    public class HoTroKhachHangController : Controller
    {
        public IActionResult Xemcauhoithuonggap()
        {
            return View();
        }
        public IActionResult yeucauhotro()
        {
            return View();
        }
    }
}
