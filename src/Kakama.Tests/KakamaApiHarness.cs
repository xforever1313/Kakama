﻿//
// Kakama - An ActivityPub Bot Framework
// Copyright (C) 2023-2024 Seth Hendrick
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

using Kakama.Api;
using Serilog;

namespace Kakama.Tests
{
    public class KakamaApiHarness : KakamaApi
    {
        // ---------------- Constructor ----------------

        public KakamaApiHarness( string dbFileName ) :
            base(
                new KakamaSettings
                {
                    DatabaseEngine = DatabaseEngine.Sqlite,
                    SqliteDatabaseLocation = new FileInfo(
                        Path.Combine(
                            Path.GetDirectoryName( typeof( KakamaApiHarness ).Assembly.Location ) ?? "",
                            dbFileName
                        )
                    ),
                    // This must be false for some reason in unit test
                    // land, or the file doesn't seem to be cleaned
                    // up between tests.
                    SqlitePoolConnection = false
                },
                new LoggerConfiguration().WriteTo.Console().CreateLogger()
            )
        {
        }

        // ---------------- Properties ----------------

        public FileInfo DbFileLocation => this.settings.SqliteDatabaseLocation;

        // ---------------- Functions ----------------

        public void PerformTestSetup()
        {
            if( this.DbFileLocation.Exists )
            {
                File.Delete( DbFileLocation.FullName );
            }

            this.Init();
        }

        public void PerformTestTearDown()
        {
            this.Dispose();

            if( this.DbFileLocation.Exists )
            {
                File.Delete( DbFileLocation.FullName );
            }
        }
    }
}
