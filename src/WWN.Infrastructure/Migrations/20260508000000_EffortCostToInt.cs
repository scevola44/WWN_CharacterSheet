using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WWN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EffortCostToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Move Arts.EffortCost from a nullable string ("Scene"/"Day"/"Sustained"/NULL)
            // to a non-nullable integer matching the EffortCommitment enum
            // (None=0, Scene=1, Day=2, Sustained=3).
            //
            // SQLite cannot ALTER a column's type in place. We do an explicit
            // table rebuild in raw SQL: KnownArts has a FK on Arts(Id), so we
            // turn FKs off for the rebuild — Arts.Id values are preserved.

            migrationBuilder.Sql(@"
                PRAGMA foreign_keys=OFF;

                CREATE TABLE Arts_new (
                    Id TEXT NOT NULL CONSTRAINT PK_Arts PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    Summary TEXT NULL,
                    MinLevel INTEGER NOT NULL,
                    EffortCost INTEGER NOT NULL DEFAULT 0,
                    Source TEXT NOT NULL
                );

                INSERT INTO Arts_new (Id, Name, Description, Summary, MinLevel, EffortCost, Source)
                SELECT Id, Name, Description, Summary, MinLevel,
                    CASE EffortCost
                        WHEN 'Scene' THEN 1
                        WHEN 'Day' THEN 2
                        WHEN 'Sustained' THEN 3
                        ELSE 0
                    END,
                    Source
                FROM Arts;

                DROP TABLE Arts;
                ALTER TABLE Arts_new RENAME TO Arts;

                PRAGMA foreign_keys=ON;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                PRAGMA foreign_keys=OFF;

                CREATE TABLE Arts_new (
                    Id TEXT NOT NULL CONSTRAINT PK_Arts PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    Summary TEXT NULL,
                    MinLevel INTEGER NOT NULL,
                    EffortCost TEXT NULL,
                    Source TEXT NOT NULL
                );

                INSERT INTO Arts_new (Id, Name, Description, Summary, MinLevel, EffortCost, Source)
                SELECT Id, Name, Description, Summary, MinLevel,
                    CASE EffortCost
                        WHEN 1 THEN 'Scene'
                        WHEN 2 THEN 'Day'
                        WHEN 3 THEN 'Sustained'
                        ELSE NULL
                    END,
                    Source
                FROM Arts;

                DROP TABLE Arts;
                ALTER TABLE Arts_new RENAME TO Arts;

                PRAGMA foreign_keys=ON;
            ");
        }
    }
}
