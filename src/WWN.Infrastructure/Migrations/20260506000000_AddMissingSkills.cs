using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WWN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert the 5 missing WWN skills at rank -1 for every character that doesn't already have them.
            // CharacterSkill.Name is stored as TEXT; CharacterSkill.Level (owned SkillRank) is stored as INTEGER.
            foreach (var skill in new[] { "Administer", "Convince", "Craft", "Exert", "Heal" })
            {
                migrationBuilder.Sql($@"
                    INSERT INTO CharacterSkills (Id, CharacterId, Name, CustomName, Level)
                    SELECT lower(hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-4' || substr(hex(randomblob(2)),2) || '-' || substr('89ab', abs(random()) % 4 + 1, 1) || substr(hex(randomblob(2)),2) || '-' || hex(randomblob(6))),
                           c.Id, '{skill}', NULL, -1
                    FROM Characters c
                    WHERE NOT EXISTS (
                        SELECT 1 FROM CharacterSkills cs
                        WHERE cs.CharacterId = c.Id AND cs.Name = '{skill}'
                    );
                ");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM CharacterSkills WHERE Name IN ('Administer', 'Convince', 'Craft', 'Exert', 'Heal');");
        }
    }
}
