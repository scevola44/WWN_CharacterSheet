using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WWN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArtSourceLookupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Replace Arts.Source (TEXT) with Arts.SourceId (INTEGER FK → ArtSources).
            //
            // SQLite cannot alter a column type or add a NOT NULL FK column in-place,
            // so we follow the same explicit table-rebuild pattern used in EffortCostToInt.
            // Existing rows with Source = 'PartialMage' map to Id = 2; everything else
            // (overwhelmingly 'Mage') maps to Id = 1.

            migrationBuilder.Sql(@"
                CREATE TABLE ArtSources (
                    Id      INTEGER NOT NULL CONSTRAINT PK_ArtSources PRIMARY KEY,
                    Code    TEXT NOT NULL,
                    DisplayName TEXT NOT NULL,
                    Description TEXT NULL,
                    SortOrder INTEGER NOT NULL
                );

                CREATE UNIQUE INDEX IX_ArtSources_Code ON ArtSources (Code);

                INSERT INTO ArtSources (Id, Code, DisplayName, Description, SortOrder) VALUES
                    (1, 'Mage',        'Mage',         NULL, 1),
                    (2, 'PartialMage', 'Partial Mage', NULL, 2);

                PRAGMA foreign_keys = OFF;

                CREATE TABLE Arts_new (
                    Id          TEXT    NOT NULL CONSTRAINT PK_Arts PRIMARY KEY,
                    Name        TEXT    NOT NULL,
                    Description TEXT    NOT NULL,
                    Summary     TEXT    NULL,
                    MinLevel    INTEGER NOT NULL,
                    EffortCost  INTEGER NOT NULL DEFAULT 0,
                    SourceId    INTEGER NOT NULL,
                    CONSTRAINT FK_Arts_ArtSources_SourceId FOREIGN KEY (SourceId)
                        REFERENCES ArtSources (Id) ON DELETE RESTRICT
                );

                INSERT INTO Arts_new (Id, Name, Description, Summary, MinLevel, EffortCost, SourceId)
                SELECT Id, Name, Description, Summary, MinLevel, EffortCost,
                    CASE WHEN Source = 'PartialMage' THEN 2 ELSE 1 END
                FROM Arts;

                DROP TABLE Arts;
                ALTER TABLE Arts_new RENAME TO Arts;

                CREATE INDEX IX_Arts_SourceId ON Arts (SourceId);

                PRAGMA foreign_keys = ON;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                PRAGMA foreign_keys = OFF;

                CREATE TABLE Arts_new (
                    Id          TEXT NOT NULL CONSTRAINT PK_Arts PRIMARY KEY,
                    Name        TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    Summary     TEXT NULL,
                    MinLevel    INTEGER NOT NULL,
                    EffortCost  INTEGER NOT NULL DEFAULT 0,
                    Source      TEXT NOT NULL
                );

                INSERT INTO Arts_new (Id, Name, Description, Summary, MinLevel, EffortCost, Source)
                SELECT a.Id, a.Name, a.Description, a.Summary, a.MinLevel, a.EffortCost,
                    COALESCE(s.Code, 'Mage')
                FROM Arts a
                LEFT JOIN ArtSources s ON s.Id = a.SourceId;

                DROP TABLE Arts;
                ALTER TABLE Arts_new RENAME TO Arts;

                PRAGMA foreign_keys = ON;

                DROP TABLE ArtSources;
            ");
        }
    }
}
