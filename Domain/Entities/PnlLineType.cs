using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class PnlLineType
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string PnlCategoryId { get; set; } = null!;

    public string CalculationMethod { get; set; } = null!;

    public decimal? DefaultRate { get; set; }

    public decimal? AmountPerTicket { get; set; }

    public short Sign { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public virtual PnlCategory PnlCategory { get; set; } = null!;

    public virtual ICollection<PnlLine> PnlLines { get; set; } = new List<PnlLine>();
}
