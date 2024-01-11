using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class UpdateGPS3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_gps3",
                table: "gps3");

            migrationBuilder.AlterColumn<string>(
                name: "VehicleCode",
                table: "gps3",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gps3",
                table: "gps3",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_gps3",
                table: "gps3");

            migrationBuilder.AlterColumn<string>(
                name: "VehicleCode",
                table: "gps3",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_gps3",
                table: "gps3",
                column: "VehicleCode");
        }
    }
}
