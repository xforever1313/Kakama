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
    public sealed class ProfileAddCommand : IKakamaCommand
    {
        // ---------------- Fields ----------------

        private readonly TextWriter consoleOut;

        // ---------------- Constructor ----------------

        public ProfileAddCommand( TextWriter consoleOut, GlobalOptions globalOptions )
        {
            this.consoleOut = consoleOut;
            this.RootCommand = new Command( "add", "Adds a new profile." );

            var namespaceIdArgument = new Option<int>(
                "--namespace_id",
                "The ID of the namespace to add the profile to."
            )
            {
                IsRequired = true
            };
            this.RootCommand.Add( namespaceIdArgument );

            var nameArgument = new Option<string>(
                "--name",
                "The name of the profile to add."
            )
            {
                IsRequired = true
            };
            this.RootCommand.Add( nameArgument );

            var slugArgument = new Option<string?>(
                "--slug",
                () => null,
                "The slug of the URL for this profile.  If not specified, this becomes the slug of the name."
            )
            {
                IsRequired = false
            };
            this.RootCommand.Add( slugArgument );

            var descriptionArgument = new Option<string?>(
                "--description",
                () => null,
                "The description of the profile.  This becomes the summary."
            );
            this.RootCommand.Add( descriptionArgument );

            var imageUrlArgument = new Option<Uri?>(
                "--image_url",
                () => null,
                "URL to the profile's image."
            )
            {
                IsRequired = false
            };
            this.RootCommand.Add( imageUrlArgument );

            this.RootCommand.SetHandler(
                this.Handler,
                globalOptions.EnvFileOption,
                namespaceIdArgument,
                nameArgument,
                slugArgument,
                descriptionArgument,
                imageUrlArgument
            );
        }

        // ---------------- Properties ----------------

        public Command RootCommand { get; private set; }

        // ---------------- Functions ----------------

        private void Handler(
            string envFileLocation,
            int namespaceId,
            string name,
            string? slug,
            string? description,
            Uri? imageUrl
        )
        {
            var profile = new Api.Models.Profile
            {
                NamespaceId = namespaceId,
                Name = name,
                Slug = slug,
                Description = description,
                ImageUrl = imageUrl
            };

            using KakamaApi api = ApiFactory.CreateApi( envFileLocation );
            api.ProfileManager.ConfigureProfile( profile );
        }
    }
}
