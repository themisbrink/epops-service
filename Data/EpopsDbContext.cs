using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EpopsService.Data
{
    public class EpopsDbContext : DbContext
    {
        public DbSet<BookData> BookData { get; set; }
        public EpopsDbContext(DbContextOptions<EpopsDbContext> options) : base(options) { }
    }

    [Table("book_data")]
    public class BookData
    {
        [Key]
        public int Id { get; set; }   // auto-increment PK

        [Column("book_uid")]
        public string BookUid { get; set; } = "";

        [Column("source_item_id")]
        public string SourceItemId { get; set; } = "";

        [Column("isbn")]
        public string ISBN { get; set; } = "";

        [Column("number_in_job")]
        public string NumberInJob { get; set; } = "";

        [Column("print_quantity")]
        public string PrintQuantity { get; set; } = "";

        [Column("status")]
        public string Status { get; set; } = "";

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
