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
using Kakama.Cli.Commands.Namespaces;
using Kakama.Cli.Commands.Profile;

namespace Kakama.Cli.Commands
{
    public sealed class MainCommand
    {
        // ---------------- Fields ----------------

        private readonly TextWriter consoleOut;
        private readonly RootCommand rootCommand;

        // -------- Child Commands --------

        private readonly NamespaceCommand namespaceCommand;

        private readonly ProfileCommand profileCommand;

        // ---------------- Constructor ----------------

        public MainCommand( TextWriter consoleOut )
        {
            this.consoleOut = consoleOut;
            this.rootCommand = new RootCommand(
                $"Admin program for {nameof( Kakama )}"
            );

            var globalOptions = new GlobalOptions();

            this.rootCommand.AddGlobalOption( globalOptions.EnvFileOption );

            this.rootCommand.SetHandler( this.Handler );

            this.namespaceCommand = new NamespaceCommand( consoleOut, globalOptions );
            this.rootCommand.Add( this.namespaceCommand.RootCommand );

            this.profileCommand = new ProfileCommand( consoleOut, globalOptions );
            this.rootCommand.Add( this.profileCommand.RootCommand );
        }

        // ---------------- Functions ----------------

        public int Invoke( string[] args )
        {
            return this.rootCommand.Invoke( args );
        }

        private void Handler()
        {
            this.consoleOut.WriteLine( "Please specify a sub-command.  Valid sub-commands are:" );
            foreach( var subCommand in this.rootCommand.Subcommands )
            {
                this.consoleOut.WriteLine( $"\t- {subCommand.Name}" );
            }
        }
    }
}
