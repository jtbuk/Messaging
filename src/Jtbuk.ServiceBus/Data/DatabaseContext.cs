using Jtbuk.ServiceBus.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jtbuk.ServiceBus.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options) { }

        public DbSet<Application> Applications { get; init; } = null!;
        public DbSet<Tenant> Tenants { get; init; } = null!;
        public DbSet<User> Users { get; init; } = null!;
        public DbSet<Entitlement> Entitlements { get; init; } = null!;

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetAutomaticFields();
            
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            SetAutomaticFields();
            
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }        
        private void SetAutomaticFields()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is UniqueEntity uniqueEntity && uniqueEntity.UniqueId == Guid.Empty)
                {
                    uniqueEntity.UniqueId = Guid.NewGuid();
                }
            }
        }
    }
}
