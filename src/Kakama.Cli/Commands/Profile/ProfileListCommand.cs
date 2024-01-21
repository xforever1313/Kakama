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

namespace Kakama.Cli.Commands.Profile
{
    public sealed class ProfileListCommand : IKakamaCommand
    {
        // ---------------- Fields ----------------

        private readonly TextWriter consoleOut;

        // ---------------- Constructor ----------------

        public ProfileListCommand( TextWriter consoleOut, GlobalOptions globalOptions )
        {
            this.consoleOut = consoleOut;
            this.RootCommand = new Command( "list", "Lists all of the profiles." );

            var namespaceIdArgument = new Option<int>(
                "--namespace_id",
                "The ID of the namespace to list the profiles of."
            )
            {
                IsRequired = true
            };
            this.RootCommand.Add( namespaceIdArgument );

            this.RootCommand.SetHandler( 
                this.Handler,
                globalOptions.EnvFileOption,
                namespaceIdArgument
            );
        }

        // ---------------- Properties ----------------

        public Command RootCommand { get; private set; }

        // ---------------- Functions ----------------

        private void Handler( string envFileLocation, int namespaceId )
        {
            using KakamaApi api = ApiFactory.CreateApi( envFileLocation );

            foreach( Api.Models.Profile profile in api.ProfileManager.GetAllProfilesWithinNamespace( namespaceId ) )
            {
                this.consoleOut.WriteLine( profile );
            }
        }
    }
}
