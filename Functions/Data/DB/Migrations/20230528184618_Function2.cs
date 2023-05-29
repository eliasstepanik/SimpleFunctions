using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Functions.Data.DB.Migrations
{
    /// <inheritdoc />
    public partial class Function2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instances_Functions_FunctionID",
                table: "Instances");

            migrationBuilder.RenameColumn(
                name: "FunctionID",
                table: "Instances",
                newName: "FunctionId");

            migrationBuilder.RenameIndex(
                name: "IX_Instances_FunctionID",
                table: "Instances",
                newName: "IX_Instances_FunctionId");

            migrationBuilder.AddColumn<string>(
                name: "InstanceId",
                table: "Instances",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Instances_Functions_FunctionId",
                table: "Instances",
                column: "FunctionId",
                principalTable: "Functions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instances_Functions_FunctionId",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "InstanceId",
                table: "Instances");

            migrationBuilder.RenameColumn(
                name: "FunctionId",
                table: "Instances",
                newName: "FunctionID");

            migrationBuilder.RenameIndex(
                name: "IX_Instances_FunctionId",
                table: "Instances",
                newName: "IX_Instances_FunctionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Instances_Functions_FunctionID",
                table: "Instances",
                column: "FunctionID",
                principalTable: "Functions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
