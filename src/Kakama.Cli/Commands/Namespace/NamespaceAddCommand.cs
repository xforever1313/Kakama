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

namespace Kakama.Cli.Commands.Namespace
{
    public sealed class NamespaceAddCommand : IKakamaCommand
    {
        // ---------------- Fields ----------------

        private readonly TextWriter consoleOut;

        // ---------------- Constructor ----------------

        public NamespaceAddCommand( TextWriter consoleOut, GlobalOptions globalOptions )
        {
            this.consoleOut = consoleOut;
            this.RootCommand = new Command( "add", "Adds a new namespace." );

            var nameArgument = new Option<string>(
                "--name",
                "The name of the namespace to add."
            )
            {
                IsRequired = true
            };
            this.RootCommand.Add( nameArgument );

            var slugArgument = new Option<string?>(
                "--slug",
                () => null,
                "The slug of the URL for this namespace.  If not specified, this becomes the slug of the name."
            )
            {
                IsRequired = false
            };
            this.RootCommand.Add( slugArgument );

            var baseUrlArgument = new Option<Uri?>(
                "--base_uri",
                () => null,
                "Specify to limit the namespace to a specific URL.  Only used when the service is meant to serve multiple URLs at once."
            )
            {
                IsRequired = false
            };
            this.RootCommand.Add( baseUrlArgument );

            this.RootCommand.SetHandler(
                this.Handler,
                globalOptions.EnvFileOption,
                nameArgument,
                slugArgument,
                baseUrlArgument
            );
        }

        // ---------------- Properties ----------------

        public Command RootCommand { get; private set; }

        // ---------------- Functions ----------------

        private void Handler( string envFileLocation, string name, string? slug, Uri? baseUrl )
        {
            var ns = new Api.Models.Namespace
            {
                BaseUrl = baseUrl,
                Name = name,
                Slug = slug
            };

            using KakamaApi api = ApiFactory.CreateApi( envFileLocation );
            api.NamespaceManager.ConfigureNamespace( ns );
        }
    }
}
