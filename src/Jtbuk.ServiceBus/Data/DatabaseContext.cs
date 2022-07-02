using Jtbuk.ServiceBus.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jtbuk.ServiceBus.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Application> Applications { get; init; } = null!;
    }
}
