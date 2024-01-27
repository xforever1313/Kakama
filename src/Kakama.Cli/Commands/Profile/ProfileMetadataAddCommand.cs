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
    public sealed class ProfileMetadataAddCommand : IKakamaCommand
    {
        // ---------------- Fields ----------------

        private readonly TextWriter consoleOut;

        // ---------------- Constructor ----------------

        public ProfileMetadataAddCommand( TextWriter consoleOut, GlobalOptions globalOptions )
        {
            this.consoleOut = consoleOut;
            this.RootCommand = new Command( "add_metadata", "Adds a new profile metadata." );

            var profileIdArgument = new Option<int>(
                "--profile_id",
                "The ID of the profile to add the metadata to."
            )
            {
                IsRequired = true
            };
            this.RootCommand.Add( profileIdArgument );

            var nameArgument = new Option<string>(
                "--name",
                "The name of the metadata to add."
            )
            {
                IsRequired = true
            };
            this.RootCommand.Add( nameArgument );

            var valueArguent = new Option<string>(
                "--value",
                "The value of the metadata to add."
            )
            {
                IsRequired = true
            };
            this.RootCommand.Add( valueArguent );

            var orderArgument = new Option<int>(
                "--explicit_order",
                () => 0,
                "The order in which the metadata should show up on the profile.  The lesser this number is, the sooner it appears."
            )
            {
                IsRequired = false
            };
            this.RootCommand.Add( orderArgument );

            this.RootCommand.SetHandler(
                this.Handler,
                globalOptions.EnvFileOption,
                profileIdArgument,
                nameArgument,
                valueArguent,
                orderArgument
            );
        }

        // ---------------- Properties ----------------

        public Command RootCommand { get; private set; }

        // ---------------- Functions ----------------

        private void Handler(
            string envFileLocation,
            int profileId,
            string name,
            string value,
            int explicitOrder
        )
        {
            var metaData = new Api.Models.ProfileMetaData
            {
                ProfileId = profileId,
                Name = name,
                Value = value,
                ExplictOrder = explicitOrder
            };

            using KakamaApi api = ApiFactory.CreateApi( envFileLocation );
            api.ProfileManager.ConfigureMetadata( metaData );
        }
    }
}
