using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EasyBooksWebApp.Data.Migrations
{
    public partial class UpdateInvoiceAndBillTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Memo",
                table: "Invoice",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Memo",
                table: "Bill",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Memo",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "Memo",
                table: "Bill");
        }
    }
}
