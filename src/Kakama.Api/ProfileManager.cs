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

using Kakama.Api.Models;
using Microsoft.EntityFrameworkCore;
using SethCS.Exceptions;
using Slugify;

namespace Kakama.Api
{
    public class ProfileManager
    {
        // ---------------- Fields ----------------

        private readonly IKakamaApi api;

        // ---------------- Constructor ----------------

        public ProfileManager( IKakamaApi api )
        {
            this.api = api;
        }

        // ---------------- Functions ----------------

        /// <summary>
        /// Adds or modifies the given profile in the database.
        /// If <see cref="Profile.Id"/> is set to 0,
        /// the profile will be added.  Otherwise, the profile
        /// at that ID will be modified.
        /// </summary>
        /// <param name="profile">
        /// The profile to add or modify.
        /// Note, the passed in object will be modified to relfect what is
        /// in the database/  If this is not desired, it can be cloned
        /// with the record class's "with" syntax before being
        /// passed in.
        /// </param>
        /// <returns>
        /// The ID of the profile that was modified, or the new ID
        /// of the profile that was added.
        /// </returns>
        public int ConfigureProfile( Profile profile )
        {
            profile.Validate();

            if( this.api.NamespaceManager.NamespaceExists( profile.NamespaceId ) == false )
            {
                throw new NamespaceNotFoundException(
                    $"Namespace at id {profile.NamespaceId} does not exist!"
                );
            }

            if( profile.Slug is null )
            {
                var slugHelper = new SlugHelper();
                profile.Slug = slugHelper.GenerateSlug( profile.Name );
            }

            using( KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection() )
            {
                DbSet<Profile> profiles = db.SafeGetProfiles();

                if( profiles.Any(
                    n => ( n.NamespaceId == profile.NamespaceId ) &&
                         ( n.Slug == profile.Slug ) &&
                         ( n.Id != profile.Id )
                    )
                )
                {
                    throw new ValidationException(
                        $"A profile with the slug {profile.Slug} within namespace {profile.NamespaceId} already exists.  We can not have duplicate slugs."
                    );
                }

                if( profile.RsaKeyId == 0 )
                {
                    DbSet<RsaKey> rsaKeys = db.SafeGetRsaKeys();
                    var newKey = new RsaKey().GenerateKey();
                    rsaKeys.Update( newKey );
                    // Must save changes before pulling the ID
                    // to save to the profile.
                    db.SaveChanges();

                    this.api.Log.Debug(
                        $"New RSA Key created for profile '{profile.Name}' has been added or modified.  Its ID is {newKey.Id}."
                    );

                    profile.RsaKeyId = newKey.Id;
                }

                profiles.Update( profile );
                db.SaveChanges();

                this.api.Log.Debug( $"Profile '{profile.Name}' has been added or modified.  Its ID is {profile.Id}." );
            }

            return profile.Id;
        }

        /// <summary>
        /// Regenerates the RSA key for the given profile.
        /// </summary>
        public void RegenerateKey( int profileId )
        {
            using( KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection() )
            {
                Profile profile = GetProfileInternal( profileId, db );

                DbSet<RsaKey> rsaKeys = db.SafeGetRsaKeys();
                RsaKey? rsaKey = rsaKeys.FirstOrDefault( r => r.Id == profile.RsaKeyId );
                if( rsaKey is null )
                {
                    throw new RsaKeyNotFoundException(
                        $"Could not find RSA key ID {profile.RsaKeyId} for profile {profileId}"
                    );
                }

                rsaKey.GenerateKey();

                rsaKeys.Update( rsaKey );
                db.SaveChanges();

                this.api.Log.Debug( $"RSA Key for profile {profileId} has been re-generated." );
            }
        }

        public RsaKey GetRsaKey( int profileId )
        {
            using( KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection() )
            {
                Profile profile = GetProfileInternal( profileId, db );

                DbSet<RsaKey> rsaKeys = db.SafeGetRsaKeys();
                RsaKey? rsaKey = rsaKeys.FirstOrDefault( r => r.Id == profile.RsaKeyId );
                if( rsaKey is null )
                {
                    throw new RsaKeyNotFoundException(
                        $"Could not find RSA key ID {profile.RsaKeyId} for profile {profileId}"
                    );
                }

                return rsaKey;
            }
        }

        /// <summary>
        /// Returns the profile meta data for the given profile ID,
        /// if any.
        /// </summary>
        public IList<ProfileMetaData> GetMetaData( int profileId )
        {
            using KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection();
            ThrowIfProfileDoesntExist( profileId, db );

            DbSet<ProfileMetaData> metaDatum = db.SafeGetProfileMetaData();
            return metaDatum.Where( m => m.ProfileId == profileId )
                .OrderBy( m => m.ExplictOrder )
                .ToList();
        }

        /// <summary>
        /// Adds or modifies the given profile metadata in the database.
        /// If <see cref="ProfileMetaData.Id"/> is set to 0,
        /// it is added to the given profile under <see cref="ProfileMetaData.ProfileId"/>.
        /// Otherwise, the metadata associated with that ID will be modified.
        /// </summary>
        /// <param name="metaData">
        /// The meta data to add or modify.
        /// Note, the passed in object will be modified to reflect what is in the database.
        /// If this is not desired, it can be cloned with the record class's "with" syntax
        /// before being passed in.
        /// </param>
        /// <returns>
        /// The ID of the metadata that was modified, or
        /// the new ID of the metadata that was added.
        /// </returns>
        public int ConfigureMetadata( ProfileMetaData metaData )
        {
            int id;
            using( KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection())
            {
                ThrowIfProfileDoesntExist( metaData.ProfileId, db );

                DbSet<ProfileMetaData> metaDatum = db.SafeGetProfileMetaData();

                metaDatum.Update( metaData );
                db.SaveChanges();

                this.api.Log.Debug( $"Profile metadata ID '{metaData.Id}' has been added or modified.  Its associated with profile ID {metaData.ProfileId}." );

                id = metaData.Id;
            }

            return id;
        }

        public IEnumerable<Profile> GetAllProfilesWithinNamespace( int namespaceId )
        {
            using KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection();
            return db.SafeGetProfiles().Where( p => p.NamespaceId == namespaceId ).ToArray();
        }

        public Profile? TryGetProfileBySlug( int namespaceId, string slug )
        {
            using KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection();
            DbSet<Profile> profiles = db.SafeGetProfiles();

            return profiles.FirstOrDefault(
                p => ( p.NamespaceId == namespaceId ) && ( p.Slug == slug )
            );
        }

        public Profile GetProfileBySlug( int namespaceId, string slug )
        {
            return TryGetProfileBySlug( namespaceId, slug ) ??
                throw new ProfileNotFoundException(
                    $"Could not find profile within namespace at slug: {slug}"
                );
        }

        public Profile? TryGetProfileById( int id )
        {
            using KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection();

            return TryGetProfileInternal( id, db );
        }

        public Profile GetProfileById( int id )
        {
            using KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection();
            return GetProfileInternal( id, db );
        }

        /// <summary>
        /// Does the given namespace exist?
        /// </summary>
        public bool ProfileExists( int id )
        {
            using KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection();

            return ProfileExistsInternal( id, db );
        }

        /// <summary>
        /// Gets the total number of profiles across all namespaces.
        /// </summary>
        public int GetNumberOfProfiles()
        {
            using KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection();

            DbSet<Profile> profiles = db.SafeGetProfiles();

            return profiles.Count();
        }

        /// <summary>
        /// Gets the total number of profiles within
        /// the given namespace.
        /// </summary>
        public int GetTotalNumberOfProfilesInNamespace( int namespaceId )
        {
            using KakamaDatabaseConnection db = this.api.CreateKakamaDatabaseConnection();

            DbSet<Profile> profiles = db.SafeGetProfiles();

            return profiles.Where( p => p.NamespaceId == namespaceId ).Count();
        }

        private static Profile? TryGetProfileInternal( int id, KakamaDatabaseConnection db )
        {
            DbSet<Profile> profiles = db.SafeGetProfiles();
            return profiles.FirstOrDefault( p => p.Id == id );
        }

        private static Profile GetProfileInternal( int id, KakamaDatabaseConnection db )
        {
            return TryGetProfileInternal( id, db ) ??
                throw new ProfileNotFoundException( $"Could not find profile of id {id}." );
        }

        private static bool ProfileExistsInternal( int id, KakamaDatabaseConnection db )
        {
            DbSet<Profile> profiles = db.SafeGetProfiles();

            return profiles.Any( p => p.Id == id );
        }

        private static void ThrowIfProfileDoesntExist( int id, KakamaDatabaseConnection db )
        {
            if( ProfileExistsInternal( id, db ) == false )
            {
                throw new ProfileNotFoundException( $"Could not find profile of id {id}." );
            }
        }
    }
}
