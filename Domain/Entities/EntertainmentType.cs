using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class EntertainmentType
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal EntertainmentTaxRate { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
