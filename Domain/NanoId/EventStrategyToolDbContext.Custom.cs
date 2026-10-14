using Microsoft.EntityFrameworkCore;

using Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Domain.Data; // Replace with your DbContext's actual namespace

public partial class EventStrategyToolDbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.UseNanoIdKeys();
    }
}