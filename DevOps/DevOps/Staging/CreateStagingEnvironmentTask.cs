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

using Cake.ArgumentBinder;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

namespace DevOps.Staging
{
    [TaskName( "create_staging" )]
    public sealed class CreateStagingEnvironmentTask : DevopsTask
    {
        // ---------------- Functions ----------------

        public override void Run( BuildContext context )
        {
            var config = context.CreateFromArguments<CreateStagingEnvironmentConfig>();

            WriteEnvFile( context, config );

            if( context.FileExists( config.DbFile ) )
            {
                context.DeleteFile( config.DbFile );
            }

            var commands = new string[]
            {
                "namespace add --name=\"Empty Namespace\" --slug=\"some-empty-namespace\"",

                "namespace add --name=\"Default Namespace\"",
                "profile add --namespace_id=2 --name=\"Seth Hendrick\" --slug=shendrick13 --description=\"This is me!\" --image_url=https://shendrick.net/static/img/me.jpg",
                "profile add_metadata --profile_id=1 --name=Pronouns --value=he/him/his --explicit_order=1",
                "profile add_metadata --profile_id=1 --name=Website --value=https://shendrick.net --explicit_order=0",

                "profile add --namespace_id=2 --name=\"Chester Hendrick\" --description=\"Seth's old dog #RIP\"",
            };

            foreach( string command in commands )
            {
                RunCli( context, config, command );
            }
        }

        private void WriteEnvFile( BuildContext context, CreateStagingEnvironmentConfig config )
        {
            var lines = new string[]
            {
                "ASPNETCORE_URLS=http://127.0.0.1:9913",
                "WEB_ALLOW_PORTS=true",
                "WEB_METRICS_URL=/Metrics",
                "WEB_STRIP_DOUBLE_SLASH=false",
                "WEB_BASE_URL=http://localhost:9913",
                "DATABASE_ENGINE=Sqlite",
                $"DATABASE_SQLITE_FILE={config.DbFile}",
                "DATABASE_SQLITE_POOL_CONNECTION=true"
            };

            System.IO.File.WriteAllLines( config.EnvFile.FullPath, lines );
        }

        private void RunCli( BuildContext context, CreateStagingEnvironmentConfig config , string arguments )
        {
            context.Information( $"Running: {arguments}" );
            arguments = $"run --project=\"{context.CliCsProj.FullPath}\" --no-build -- env \"{config.EnvFile.FullPath}\" {arguments}";

            IProcess process = context.ProcessRunner.Start(
                $"dotnet",
                new ProcessSettings
                {
                    Arguments = arguments
                }
            );

            process.WaitForExit();
            if( process.GetExitCode() != 0 )
            {
                throw new CakeException( $"Failed run CLI with arguments: {arguments}" );
            }

            context.Information( "" );
        }
    }
}
