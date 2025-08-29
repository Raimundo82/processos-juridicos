using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Processos_Juridicos.Migrations;

/// <inheritdoc />
public partial class UsersAndRolesMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Roles",
            columns: table => new
            {
                role_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                role_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Roles", x => x.role_id);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                user_nii = table.Column<string>(type: "nvarchar(450)", nullable: false),
                user_role = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.user_nii);
                table.ForeignKey(
                    name: "FK_Users_Roles_user_role",
                    column: x => x.user_role,
                    principalTable: "Roles",
                    principalColumn: "role_id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_Users_user_role",
            table: "Users",
            column: "user_role");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Users");

        migrationBuilder.DropTable(
            name: "Roles");
    }
}
