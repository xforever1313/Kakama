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
    public sealed class ProfileRekeyCommand : IKakamaCommand
    {
        // ---------------- Fields ----------------

        private readonly TextWriter consoleOut;

        // ---------------- Constructor ----------------

        public ProfileRekeyCommand( TextWriter consoleOut, GlobalOptions globalOptions )
        {
            this.consoleOut = consoleOut;
            this.RootCommand = new Command( "rekey", "Re-generates a profile's private/public keys." );

            var profileIdArgument = new Option<int>(
                "--profile_id",
                "The ID of the profile to rekey."
            )
            {
                IsRequired = true
            };
            this.RootCommand.Add( profileIdArgument );

            this.RootCommand.SetHandler(
                this.Handler,
                globalOptions.EnvFileOption,
                profileIdArgument
            );
        }

        // ---------------- Properties ----------------

        public Command RootCommand { get; private set; }

        // ---------------- Functions ----------------

        private void Handler(
            string envFileLocation,
            int profileId
        )
        {
            using KakamaApi api = ApiFactory.CreateApi( envFileLocation );
            api.ProfileManager.RegenerateKey( profileId );
        }
    }
}
