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

using Kakama.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Kakama.Api
{
    internal class KakamaDatabaseConnection : BaseDatabaseConnection
    {
        // ---------------- Constructor ----------------

        public KakamaDatabaseConnection() :
            base()
        {
        }

        // ---------------- Properties ----------------

        public DbSet<Namespace>? Namespaces { get; set; }

        public DbSet<Profile>? Profiles { get; set; }

        public DbSet<ProfileMetaData>? ProfileMetadata { get; set; }

        public DbSet<Post>? Posts { get; set; }

        // ---------------- Functions ----------------

        public DbSet<Namespace> SafeGetNamespaces() =>
            this.Namespaces ?? throw new ArgumentNullException( nameof( this.Namespaces ) );

        public DbSet<Profile> SafeGetProfiles() =>
            this.Profiles ?? throw new ArgumentNullException( nameof( this.Profiles ) );

        public DbSet<ProfileMetaData> SafeGetProfileMetaData() =>
            this.ProfileMetadata ?? throw new ArgumentNullException( nameof( this.ProfileMetadata ) );

        public DbSet<Post> SafeGetPosts() =>
            this.Posts ?? throw new ArgumentNullException( nameof( this.Posts ) );
    }
}
