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
using Cake.Core.IO;
using Cake.Frosting;

namespace DevOps.Staging
{
    public class CreateStagingEnvironmentConfig
    {
        // ---------------- Fields ----------------

        private const string defaultStagingDb = "kakama_staging.db";

        private const string defaultEnvFile = "kakama_staging.env";

        // ---------------- Properties ----------------

        [FilePathArgument(
            "db_file",
            DefaultValue = defaultStagingDb
        )]
        public FilePath DbFile { get; set; } = new FilePath( defaultStagingDb );

        [FilePathArgument(
            "env_file",
            DefaultValue = defaultEnvFile
        )]
        public FilePath EnvFile { get; set; } = new FilePath( defaultEnvFile );
    }
}