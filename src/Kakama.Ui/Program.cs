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

using System.CommandLine;
using Kakama.Cli.Commands;

namespace Kakama.Ui
{
    internal class Program
    {
        static int Main( string[] args )
        {
            var mainCommand = new MainCommand( Console.Out );
            return mainCommand.Invoke( args );

            var rootCommand = new RootCommand( "Admin program for Kakama." );
            rootCommand.SetHandler(
                () =>
                {
                    Console.WriteLine( "Hello world!" );
                }
            );

            var subCommand1 = new Command( "namespace", "Manage Namespaces" );
            rootCommand.Add( subCommand1 );
            subCommand1.SetHandler(
                () =>
                {
                    Console.WriteLine( "Namespace Stuff." );
                }
            );


            return rootCommand.Invoke( args );
        }
    }
}
