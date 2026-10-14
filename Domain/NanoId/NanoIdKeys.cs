using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NanoidDotNet;

namespace Domain.Data;

/// <summary>
/// Gives every single-column string primary key named "Id" a NanoID (21 characters, from the Nanoid NuGet package)
/// the moment an entity is added to the context, before SaveChanges.
/// </summary>
public static class NanoIdKeys
{
    /// <summary>
    /// Call it LAST in OnModelCreating, after the scaffolded configuration, because the scaffold sets a default SQL
    /// expression (NEXT VALUE FOR seq_...) and possibly ValueGeneratedNever() on these columns; this overrides both.
    /// Composite-key tables (EventDepartment), keyless views and sysdiagrams are skipped automatically.
    /// </summary>
    public static ModelBuilder UseNanoIdKeys(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.IsKeyless) continue;

            var key = entityType.FindPrimaryKey();
            if (key is null || key.Properties.Count != 1) continue;

            var id = key.Properties[0];
            if (id.Name != "Id" || id.ClrType != typeof(string)) continue;

            id.SetDefaultValueSql(null);
            id.ValueGenerated = ValueGenerated.OnAdd;
            id.SetValueGeneratorFactory((_, _) => new NanoIdValueGenerator());
        }
        return modelBuilder;
    }

    private sealed class NanoIdValueGenerator : ValueGenerator<string>
    {
        public override bool GeneratesTemporaryValues => false;

        // Default NanoID: 21 characters, alphabet A-Z a-z 0-9 _ -
        public override string Next(EntityEntry entry) => Nanoid.Generate();
    }
}
