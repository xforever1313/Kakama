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

using Kakama.Api.Models;
using Microsoft.EntityFrameworkCore;
using Slugify;

namespace Kakama.Api
{
    public class NamespaceManager
    {
        // ---------------- Fields ----------------

        private readonly IKakamaApi api;

        // ---------------- Constructor ----------------

        public NamespaceManager( IKakamaApi api )
        {
            this.api = api;
        }

        // ---------------- Functions ----------------

        /// <summary>
        /// Adds or modifies the given namespace in the database.
        /// If <see cref="Namespace.Id"/> is set to 0,
        /// the namespace will be added.  Else, the namespace
        /// at that ID will be modified.
        /// </summary>
        /// <param name="ns">
        /// The namespace to add or modify.
        /// Note, the passed in object will be modified to reflect what is in the database.
        /// If this is not desired, it can be cloned with the record class's "with" syntax
        /// before being passed in.
        /// </param>
        /// <returns>
        /// The ID of the namespace that was modified, or the
        /// new ID of the namespace that was added.
        /// </returns>
        public int ConfigureNamespace( Namespace ns )
        {
            if( ns.Slug is null )
            {
                var slugHelper = new SlugHelper();
                ns.Slug = slugHelper.GenerateSlug( ns.Name );
            }

            int id;
            using( KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection() )
            {
                DbSet<Namespace> namespaces = db.SafeGetNamespaces();

                // TODO: Validate.
                // Make sure anything required is not null or whitespace.
                // Make sure slugs are unique.

                namespaces.Add( ns );
                db.SaveChanges();

                this.api.Log.Debug( $"Namespace '{ns.Name}' has been added or modified.  Its ID is {ns.Id}." );
                id = ns.Id;
            }

            return id;
        }

        public IEnumerable<Namespace> GetAllNamespaces()
        {
            using( KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection() )
            {
                return db.SafeGetNamespaces().Where( n => true );
            }
        }

        public Namespace? TryGetNamespaceBySlug( string slug )
        {
            using( KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection() )
            {
                DbSet<Namespace> namespaces = db.SafeGetNamespaces();

                return namespaces.FirstOrDefault( n => n.Slug == slug );
            }
        }

        public Namespace GetNamespaceBySlug( string slug )
        {
            return TryGetNamespaceBySlug( slug ) ??
                throw new NamespaceNotFoundException( $"Could not find namespace located at slug: {slug}" );
        }

        public Namespace? TryGetNamespaceById( int id )
        {
            using( KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection() )
            {
                DbSet<Namespace> namespaces = db.SafeGetNamespaces();

                return namespaces.FirstOrDefault( n => n.Id == id );
            }
        }

        public Namespace GetNamespaceById( int id )
        {
            return TryGetNamespaceById( id ) ??
                throw new NamespaceNotFoundException( $"Could not find namespace by id: {id}" );
        }
    }
}