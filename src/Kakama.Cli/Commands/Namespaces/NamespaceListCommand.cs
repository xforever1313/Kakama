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

using System.CommandLine;
using Kakama.Api;
using Kakama.Standard.Namespaces;

namespace Kakama.Cli.Commands.Namespaces
{
    public sealed class NamespaceListCommand : IKakamaCommand
    {
        // ---------------- Fields ----------------

        private readonly TextWriter consoleOut;

        // ---------------- Constructor ----------------

        public NamespaceListCommand( TextWriter consoleOut, GlobalOptions globalOptions )
        {
            this.consoleOut = consoleOut;
            this.RootCommand = new Command( "list", "Lists all of the namespaces." );
            this.RootCommand.SetHandler( this.Handler, globalOptions.EnvFileOption );
        }

        // ---------------- Properties ----------------

        public Command RootCommand { get; private set; }

        // ---------------- Functions ----------------

        private void Handler( string envFileLocation )
        {
            using KakamaApi api = ApiFactory.CreateApi( envFileLocation );
            foreach( Namespace ns in api.NamespaceManager.GetAllNamespaces() )
            {
                this.consoleOut.WriteLine( ns );
            }
        }
    }
}
