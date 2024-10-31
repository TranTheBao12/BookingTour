using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingTour.Models;

public partial class TypeOfTour
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdType { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
