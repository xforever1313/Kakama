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

using Kakama.Cli.Commands;

namespace Kakama.Cli
{
    internal class Program
    {
        static int Main( string[] args )
        {
            try
            {
                var mainCommand = new MainCommand( Console.Out );
                return mainCommand.Invoke( args );
            }
            catch( Exception e )
            {
                Console.WriteLine( "FATAL: Unhandled Exception:" );
                Console.WriteLine( e.ToString() );
                return -1;
            }
        }
    }
}
