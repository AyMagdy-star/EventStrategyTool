using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class AnnualBudget
{
    public string Id { get; set; } = null!;

    public short FiscalYear { get; set; }

    public decimal BudgetAmount { get; set; }

    public string Currency { get; set; } = null!;
}
