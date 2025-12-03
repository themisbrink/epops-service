using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EpopsService.Data
{
    public class EpopsDbContext : DbContext
    {
        public DbSet<BookData> BookData { get; set; }
        public EpopsDbContext(DbContextOptions<EpopsDbContext> options) : base(options) { }
    }

    public class BookData
    {
        public int Id { get; set; }
        public string sourceItemId { get; set; }
        public string ISBN { get; set; }
        public string numberInJob { get; set; }
        public string printQuantity { get; set; }
        public string Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
