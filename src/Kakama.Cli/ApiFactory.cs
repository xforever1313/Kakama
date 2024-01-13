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

using dotenv.net;
using Kakama.Api;
using Serilog;

namespace Kakama.Cli
{
    internal static class ApiFactory
    {
        public static KakamaApi CreateApi( string? envFileLocation )
        {
            ILogger log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            if( envFileLocation is not null )
            {
                log.Information(
                    $"Loading .env file located at '{envFileLocation}'."
                );

                DotEnv.Load( new DotEnvOptions( envFilePaths: new string[] { envFileLocation } ) );
            }

            KakamaSettings settings = KakamaSettingsExtensions.FromEnvVar();
            // For command-line, do not start scheduled events, as the CLI
            // is only meant to talk directly to the database,
            // not run any events.
            var api = new KakamaApi( settings, log, false, Array.Empty<FileInfo>() );

            api.Init();

            return api;
        }
    }
}
