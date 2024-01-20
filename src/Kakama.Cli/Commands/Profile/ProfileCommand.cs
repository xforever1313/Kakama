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

namespace Kakama.Cli.Commands.Profile
{
    public sealed class ProfileCommand : IKakamaCommand
    {
        // ---------------- Fields ----------------

        private readonly TextWriter consoleOut;

        // -------- Sub Commands --------

        // ---------------- Constructor ----------------

        public ProfileCommand( TextWriter consoleOut, GlobalOptions globalOptions )
        {
            this.consoleOut = consoleOut;
            this.RootCommand = new Command( "profile", "Manage Profiles" );
            this.RootCommand.SetHandler( this.Handler );
        }

        // ---------------- Properties ----------------

        public Command RootCommand { get; private set; }

        // ---------------- Functions ----------------

        private void Handler()
        {
            this.consoleOut.WriteLine( $"Please specify a sub-command for {this.RootCommand.Name}.  Valid sub-commands are:" );
            foreach( var subCommand in this.RootCommand.Subcommands )
            {
                this.consoleOut.WriteLine( $"- {subCommand.Name}" );
            }
        }
    }
}
