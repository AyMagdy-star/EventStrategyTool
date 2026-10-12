using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class PnlCategory
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsRevenue { get; set; }

    public int SortOrder { get; set; }

    public virtual ICollection<PnlLineType> PnlLineTypes { get; set; } = new List<PnlLineType>();
}
