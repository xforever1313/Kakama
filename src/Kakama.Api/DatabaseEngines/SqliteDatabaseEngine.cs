//
// Kakama - An ActivityPub Bot Framework
// Copyright (C) 2023 Seth Hendrick
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

using Microsoft.EntityFrameworkCore;

namespace Kakama.Api.DatabaseEngines
{
    internal class SqliteDatabaseEngine : IDatabaseEngine
    {
        // ---------------- Fields ----------------

        private readonly bool pool;

        // ---------------- Constructor ----------------

        public SqliteDatabaseEngine( FileInfo databaseLocation, bool pool )
        {
            this.DatabaseLocation = databaseLocation;
            this.pool = pool;
        }

        // ---------------- Properties ----------------

        public FileInfo DatabaseLocation { get; private set; }

        // ---------------- Functions ----------------

        public void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
        {
            string poolString = "";
            if( this.pool == false )
            {
                poolString = ";Pooling=False";
            }

            optionsBuilder.UseSqlite( $"Data Source={this.DatabaseLocation.FullName}{poolString}" );
        }

        public void OnModelCreating( ModelBuilder modelBuilder )
        {
        }
    }
}
