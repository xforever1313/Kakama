//
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

using Kakama.Cli.Commands;
using SethCS.Extensions;

namespace Kakama.Tests
{
    public class KakamaCliHarness
    {
        // ---------------- Constructor ----------------

        public KakamaCliHarness( string dbFileName, string dotEnvFileName )
        {
            this.ApiHarness = KakamaApiHarness.Create( dbFileName );
            this.DotEnvFileLocation = new FileInfo(
                Path.Combine(
                    Path.GetDirectoryName( typeof( KakamaCliHarness ).Assembly.Location ) ?? "",
                    dotEnvFileName
                )
            );
        }

        // ---------------- Properties ----------------

        public KakamaApiHarness ApiHarness { get; private set; }

        public FileInfo DotEnvFileLocation { get; private set; }

        // ---------------- Setup / Teardown ----------------

        public void PerformTestSetup()
        {
            if( this.DotEnvFileLocation.Exists )
            {
                File.Delete( this.DotEnvFileLocation.FullName );
            }
            File.WriteAllText(
                this.DotEnvFileLocation.FullName,
$@"DATABASE_ENGINE=Sqlite
DATABASE_SQLITE_FILE={this.ApiHarness.DbFileLocation.FullName}

# Must be false since otherwise this doesn't get cleaned up
# in unit test land for some reason.
DATABASE_SQLITE_POOL_CONNECTION=false
"
            );

            this.ApiHarness.PerformTestSetup();
        }

        public void PerformTestTearDown()
        {
            this.ApiHarness.PerformTestTearDown();

            if( this.DotEnvFileLocation.Exists )
            {
                File.Delete( this.DotEnvFileLocation.FullName );
            }
        }

        // ---------------- Functions ----------------

        public int RunArgumentsWithDefaultGlobalSettings( string[] args )
        {
            var allArgs = new List<string>( args )
            {
                "env",
                this.DotEnvFileLocation.FullName
            };

            return RunArguments( allArgs.ToArray() );
        }

        public int RunArguments( string[] args )
        {
            Console.WriteLine( "Invoking command line with the following args:" );
            Console.WriteLine( args.ToListString( "  " ) );
            Console.WriteLine();

            var mainCommand = new MainCommand( Console.Out );
            return mainCommand.Invoke( args );
        }
    }
}
