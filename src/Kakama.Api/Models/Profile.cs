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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SethCS.Exceptions;

namespace Kakama.Api.Models
{
    /// <summary>
    /// A profile represents an Actor that this bot runs.
    /// This acts as the "Profile" for the Actors Kakama manages.
    /// </summary>
    public record class Profile
    {
        /// <summary>
        /// The profile ID.  If set to the default,
        /// then a new profile is added.  Otherwise,
        /// a profile is modified.
        /// </summary>
        [Key]
        [Required]
        public int Id { get; internal set; }

        /// <summary>
        /// The namespace this profile is a part of.
        /// Must be greater than 0.
        /// </summary>
        /// <remarks>
        /// Not internal since we should be able to change
        /// a profile's namespace pretty easily.
        /// </remarks>
        [Required]
        [ForeignKey( "NamespaceId" )]
        public int NamespaceId { get; set; }

        /// <summary>
        /// The RSA keys associated with this profile.
        /// If this is set to the default, a new RSA key will be generated,
        /// and this id will be updated accordingly when configuring
        /// the profile.
        /// </summary>
        /// <remarks>
        /// RSA keys are in their own table since
        /// they are long.
        /// 
        /// Internal set since the manager should handle
        /// setting up keys, not the user.
        /// </remarks>
        [Required]
        [ForeignKey( "RsaKeyId" )]
        public int RsaKeyId { get; internal set; }

        /// <summary>
        /// The name of the profile.
        /// 
        /// A profile name can not exist within the same namespace.
        /// </summary>
        [Required]
        public string Name { get; set; } = "";

        /// <summary>
        /// The slug of the URL for this namespace.
        /// If this is set to null, then it will be <see cref="Name"/>
        /// that is converted to lowercase, and all spaces are replaced
        /// with '_'.
        /// </summary>
        public string? Slug { get; set; } = null;

        /// <summary>
        /// An optional description of the profile.
        /// </summary>
        public string? Description { get; set; } = null;

        /// <summary>
        /// An optional URL to the Profile's image.
        /// </summary>
        public Uri? ImageUrl { get; set; } = null;
    }

    internal static class ProfileExtensions
    {
        public static void Validate( this Profile profile )
        {
            var errors = new List<string>();

            if( profile.Id < 0 )
            {
                errors.Add( $"ID can not be less than zero, got: {profile.Id}." );
            }

            if( profile.NamespaceId <= 0 )
            {
                errors.Add( $"Namespace ID can not be zero or less, got: {profile.NamespaceId}." );
            }

            if( profile.RsaKeyId < 0 )
            {
                errors.Add( $"RSA Key ID can not be less than zero, got: {profile.RsaKeyId}." );
            }

            if( string.IsNullOrWhiteSpace( profile.Name ) )
            {
                errors.Add( $"Profile name can not be null, empty, or whitespace" );
            }

            if( errors.Any() )
            {
                throw new ListedValidationException( "Errors when validating profile", errors );
            }
        }
    }
}
