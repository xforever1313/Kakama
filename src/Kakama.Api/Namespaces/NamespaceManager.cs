﻿//
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

using Kakama.Standard.Namespaces;
using Microsoft.EntityFrameworkCore;
using SethCS.Exceptions;
using Slugify;

namespace Kakama.Api.Namespaces
{
    public class NamespaceManager : INamespaceManager
    {
        // ---------------- Fields ----------------

        private readonly IKakamaApi api;

        // ---------------- Constructor ----------------

        public NamespaceManager( IKakamaApi api )
        {
            this.api = api;
        }

        // ---------------- Functions ----------------

        /// <inheritdoc/>
        public int ConfigureNamespace( Namespace ns )
        {
            ns.Validate();

            if( ns.Slug is null )
            {
                var slugHelper = new SlugHelper();
                ns.Slug = slugHelper.GenerateSlug( ns.Name );
            }

            int id;
            using( KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection() )
            {
                DbSet<Namespace> namespaces = db.SafeGetNamespaces();

                if( namespaces.Any( n => ( n.Slug == ns.Slug ) && ( n.Id != ns.Id ) ) )
                {
                    throw new ValidationException(
                        $"A namespace with the slug {ns.Slug} already exists.  We can not have duplicate slugs."
                    );
                }

                namespaces.Update( ns );
                db.SaveChanges();

                this.api.Log.Debug( $"Namespace '{ns.Name}' has been added or modified.  Its ID is {ns.Id}." );
                id = ns.Id;
            }

            return id;
        }

        /// <inheritdoc/>
        public IEnumerable<Namespace> GetAllNamespaces()
        {
            using( KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection() )
            {
                return db.SafeGetNamespaces().Where( n => true ).ToArray();
            }
        }

        /// <inheritdoc/>
        public List<Namespace> GetNamespacesCompatibleWithUrl( Uri uri )
        {
            using( KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection() )
            {
                return db.SafeGetNamespaces().Where(
                    n => ( n.BaseUri == null ) || uri.Equals( n.BaseUri )
                ).ToList();
            }
        }

        /// <inheritdoc/>
        public Namespace? TryGetNamespaceBySlug( string slug )
        {
            using( KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection() )
            {
                DbSet<Namespace> namespaces = db.SafeGetNamespaces();

                return namespaces.FirstOrDefault( n => n.Slug == slug );
            }
        }

        /// <inheritdoc/>
        public Namespace GetNamespaceBySlug( string slug )
        {
            return TryGetNamespaceBySlug( slug ) ??
                throw new NamespaceNotFoundException( $"Could not find namespace located at slug: {slug}" );
        }

        /// <inheritdoc/>
        public Namespace? TryGetNamespaceById( int id )
        {
            using( KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection() )
            {
                DbSet<Namespace> namespaces = db.SafeGetNamespaces();

                return namespaces.FirstOrDefault( n => n.Id == id );
            }
        }

        /// <inheritdoc/>
        public Namespace GetNamespaceById( int id )
        {
            return TryGetNamespaceById( id ) ??
                throw new NamespaceNotFoundException( $"Could not find namespace by id: {id}" );
        }

        /// <inheritdoc/>
        public bool NamespaceExists( int id )
        {
            using( KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection() )
            {
                DbSet<Namespace> namespaces = db.SafeGetNamespaces();

                return namespaces.Any( n => n.Id == id );
            }
        }

        /// <inheritdoc/>
        public int GetNumberOfNamespaces()
        {
            using( KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection() )
            {
                DbSet<Namespace> namespaces = db.SafeGetNamespaces();

                return namespaces.Count();
            }
        }
    }
}
