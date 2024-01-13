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
    public sealed class NamespaceModifyCommand : IKakamaCommand
    {
        // ---------------- Fields ----------------

        /// <summary>
        /// If this is passed in to an argument,
        /// the argument gets set to null in the database.
        /// </summary>
        /// <remarks>
        /// Used since this is not a valid slug (slugs prefer '-' ove '_')
        /// or URI, so it works.
        /// </remarks>
        private const string clearString = "_";

        private readonly TextWriter consoleOut;

        // ---------------- Constructor ----------------

        public NamespaceModifyCommand( TextWriter consoleOut, GlobalOptions globalOptions )
        {
            this.consoleOut = consoleOut;
            this.RootCommand = new Command( "modify", "Modifies an existing namespace." );

            var idArgument = new Option<int>(
                "--id",
                "The ID of the namespace to modify."
            )
            {
                IsRequired = true
            };
            this.RootCommand.Add( idArgument );

            var nameArgument = new Option<string?>(
                "--name",
                () => null,
                "The new name to give the namespace."
            )
            {
                IsRequired = false,
            };
            this.RootCommand.Add( nameArgument );

            var slugArgument = new Option<string?>(
                "--slug",
                () => null,
                $"The new slug of the namespace.  Setting to '{clearString}' resets this to the default, which is the slug of the name."
            )
            {
                IsRequired = false
            };
            this.RootCommand.Add( slugArgument );

            var baseUrlArgument = new Option<string?>(
                "--base_uri",
                () => null,
                $"The new base uri.  Setting to '{clearString}' makes this null in the database."
            )
            {
                IsRequired = false
            };
            this.RootCommand.Add( baseUrlArgument );

            this.RootCommand.SetHandler(
                this.Handler,
                globalOptions.EnvFileOption,
                idArgument,
                nameArgument,
                slugArgument,
                baseUrlArgument
            );
        }

        // ---------------- Properties ----------------

        public Command RootCommand { get; private set; }

        // ---------------- Functions ----------------

        private void Handler(
            string envFileLocation,
            int id,
            string? newName,
            string? slug,
            string? baseUrl
        )
        {
            using KakamaApi api = ApiFactory.CreateApi( envFileLocation );

            Api.Models.Namespace ns = api.NamespaceManager.GetNamespaceById( id );
            if( newName is not null )
            {
                ns = ns with { Name = newName };
            }

            if( slug is not null )
            {
                if( slug == clearString )
                {
                    ns = ns with { Slug = null };
                }
                else
                {
                    ns = ns with { Slug = slug };
                }
            }

            if( baseUrl is not null )
            {
                if( baseUrl == clearString )
                {
                    ns = ns with { BaseUrl = null };
                }
                else
                {
                    ns = ns with { BaseUrl = new Uri( baseUrl ) };
                }
            }

            api.NamespaceManager.ConfigureNamespace( ns );
        }
    }
}
