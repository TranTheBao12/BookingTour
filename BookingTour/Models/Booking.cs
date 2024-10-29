using System;
using System.Collections.Generic;

namespace BookingTour.Models;

public partial class Booking
{
    public long IdBooking { get; set; }

    public DateTime? CheckInDate { get; set; }

    public DateTime? CheckOutDate { get; set; }

    public DateTime? BookingTime { get; set; }

    public string Id { get; set; } = null!;

    public long IdHotel { get; set; }

    public long IdTour { get; set; }

    public long IdStatus { get; set; }

    public virtual Hotel IdHotelNavigation { get; set; } = null!;

    public virtual AspNetUser IdNavigation { get; set; } = null!;

    public virtual BookingStatus IdStatusNavigation { get; set; } = null!;

    public virtual Tour IdTourNavigation { get; set; } = null!;
}
