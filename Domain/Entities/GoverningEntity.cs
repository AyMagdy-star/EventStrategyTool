using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class GoverningEntity
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal? MinScore { get; set; }

    public decimal? MaxScore { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
