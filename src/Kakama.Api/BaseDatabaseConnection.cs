﻿//
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

using Kakama.Api.DatabaseEngines;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Kakama.Api
{
    public abstract class BaseDatabaseConnection : DbContext
    {
        // ---------------- Fields ----------------

        private IDatabaseEngine? dbEngine;

        private ILogger? log;

        // ---------------- Constructor ----------------

        protected BaseDatabaseConnection()
        {
        }

        // ---------------- Properties ----------------

        protected ILogger Log =>
            this.log ?? throw new ArgumentNullException( nameof( this.log ) );

        private IDatabaseEngine DbEngine
            => this.dbEngine ?? throw new ArgumentNullException( nameof( this.dbEngine ) );
       
        // ---------------- Functions ----------------

        public void EnsureCreated()
        {
            this.Log.Information( "Creating Database" );
            this.Database.EnsureCreated();
        }

        internal void Init( IDatabaseEngine dbEngine, ILogger log )
        {
            this.dbEngine = dbEngine;
            this.log = log;
        }

        protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
        {
            this.DbEngine.OnConfiguring( optionsBuilder );
            base.OnConfiguring( optionsBuilder );
        }

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
            this.DbEngine.OnModelCreating( modelBuilder );
            base.OnModelCreating( modelBuilder );
        }
    }
}
