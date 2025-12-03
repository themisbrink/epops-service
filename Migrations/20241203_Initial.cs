using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EpopsService.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                                    Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.SerialColumn),
                    book_uid = table.Column<string>(nullable: false),
                    source_item_id = table.Column<string>(nullable: false),
                    isbn = table.Column<string>(nullable: false),
                    number_in_job = table.Column<string>(nullable: false),
                    print_quantity = table.Column<string>(nullable: false),
                    status = table.Column<string>(nullable: false),
                    updated_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookData");
        }
    }
}
