using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EpopsService.Migrations
{
    [DbContext(typeof(EpopsService.Data.EpopsDbContext))]
    partial class EpopsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("EpopsService.Data.BookData", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                b.Property<string>("BookUid")
                    .HasColumnName("book_uid")
                    .HasColumnType("text");

                b.Property<string>("SourceItemId")
                    .HasColumnName("source_item_id")
                    .HasColumnType("text");

                b.Property<string>("ISBN")
                    .HasColumnName("isbn")
                    .HasColumnType("text");

                b.Property<string>("NumberInJob")
                    .HasColumnName("number_in_job")
                    .HasColumnType("text");

                b.Property<string>("PrintQuantity")
                    .HasColumnName("print_quantity")
                    .HasColumnType("text");

                b.Property<string>("Status")
                    .HasColumnName("status")
                    .HasColumnType("text");

                b.Property<DateTime>("UpdatedAt")
                    .HasColumnName("updated_at")
                    .HasColumnType("timestamp without time zone");

                b.HasKey("Id");

                b.ToTable("BookData");
            });
        }
    }
}
