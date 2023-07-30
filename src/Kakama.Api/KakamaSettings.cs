//
// Kakama - An ActivityPub Bot Framework
// Copyright (C) 2023 Seth Hendrick
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

namespace Kakama.Api
{
    public record class KakamaSettings
    {
        // ---------------- Properties ----------------

        public DatabaseEngine DatabaseEngine { get; init; } = DatabaseEngine.Sqlite;

        /// <summary>
        /// The file location of the SQLite database.
        /// Only used if the <see cref="DatabaseEngine"/> is set to
        /// <see cref="DatabaseEngine.Sqlite"/>.
        /// </summary>
        public FileInfo SqliteDatabaseLocation { get; init; } =
            new FileInfo( Path.Combine( Environment.CurrentDirectory, "kakama.db" ) );

        /// <summary>
        /// Whether or not to pool the SQLite connections.
        /// Defauted to true.  This really should only be false
        /// when running unit tests.
        /// </summary>
        public bool SqlitePoolConnection { get; init; } = true;
    }

    public static class KakamaSettingsExtensions
    {
        // ---------------- Functions ----------------

        public static KakamaSettings FromEnvVar()
        {
            bool NotNull( string envName, out string envValue )
            {
                envValue = Environment.GetEnvironmentVariable( envName ) ?? "";
                return string.IsNullOrWhiteSpace( envValue ) == false;
            }

            var settings = new KakamaSettings();

            if( NotNull( "DATABASE_ENGINE", out string engine ) )
            {
                settings = settings with
                {
                    DatabaseEngine = Enum.Parse<DatabaseEngine>( engine, true )
                };
            }

            if( NotNull( "DATABASE_SQLITE_FILE", out string dbFile ) )
            {
                settings = settings with
                {
                    SqliteDatabaseLocation = new FileInfo( dbFile )
                };
            }

            if( NotNull( "DATABASE_SQLITE_POOL_CONNECTION", out string poolConnection ) )
            {
                settings = settings with
                {
                    SqlitePoolConnection = bool.Parse( poolConnection )
                };
            }

            return settings;
        }
    }
}
