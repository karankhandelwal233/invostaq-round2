using Microsoft.EntityFrameworkCore;
using InvoiceApi.Models;

namespace InvoiceApi.Data
{
    public class InvoiceDbContext : DbContext
    {
        public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options)
            : base(options)
        {
        }

        public DbSet<Invoice> Invoices => Set<Invoice>();
    }
}
