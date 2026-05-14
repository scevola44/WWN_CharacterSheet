using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WWN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWeaponTypeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WeaponType",
                table: "Items",
                type: "INTEGER",
                nullable: true);

            // Derive WeaponType from the retired Ranged bit (2) in Tags.
            // 0 = Melee (enum ordinal), 1 = Ranged (enum ordinal).
            migrationBuilder.Sql(@"
                UPDATE Items
                SET WeaponType = CASE WHEN (Tags & 2) = 2 THEN 1 ELSE 0 END
                WHERE ItemType = 'Weapon';

                UPDATE Items
                SET Tags = (Tags & ~3)
                WHERE ItemType = 'Weapon' AND Tags IS NOT NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore Melee (1) and Ranged (2) bits from WeaponType before dropping the column.
            migrationBuilder.Sql(@"
                UPDATE Items
                SET Tags = COALESCE(Tags, 0) | CASE WHEN WeaponType = 1 THEN 2 ELSE 1 END
                WHERE ItemType = 'Weapon';
            ");

            migrationBuilder.DropColumn(
                name: "WeaponType",
                table: "Items");
        }
    }
}
