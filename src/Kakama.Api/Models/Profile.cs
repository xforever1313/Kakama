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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using KristofferStrube.ActivityStreams;
using KristofferStrube.ActivityStreams.JsonLD;
using SethCS.Exceptions;

using Service = KristofferStrube.ActivityStreams.Service;

namespace Kakama.Api.Models
{
    /// <summary>
    /// A profile represents an Actor that this bot runs.
    /// This acts as the "Profile" for the Actors Kakama manages.
    /// </summary>
    public record class Profile
    {
        // ---------------- Fields ----------------

        internal static readonly DateTime DefaultCreationDateTime = new DateTime( 1900, 1, 1, 0, 0, 0, DateTimeKind.Utc );

        // ---------------- Properties ----------------

        /// <summary>
        /// The profile ID.  If set to the default,
        /// then a new profile is added.  Otherwise,
        /// a profile is modified.
        /// </summary>
        [Key]
        [Required]
        public int Id { get; internal set; }

        /// <summary>
        /// The time this profile was created.
        /// If <see cref="Id"/> is set to the default value
        /// (where we expect the profile to be new),
        /// then will be set to <see cref="DateTime.UtcNow"/>
        /// when passed into <see cref="ProfileManager.ConfigureProfile(Profile)"/>.
        /// Otherwise, this will be left alone.
        /// </summary>
        /// <remarks>
        /// Use UTC when saving stuff to the database,
        /// present local time to the client-facing stuff.
        /// 
        /// Default this to 1900, we're just going to override it to the current
        /// time when its created anyways.
        /// </remarks>
        [Required]
        public DateTime CreationTime { get; internal set; } = DefaultCreationDateTime;

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

        /// <summary>
        /// The link to the human-readable profile.
        /// This is useful if the information about the profile
        /// lives under a different URL, while Kakama serves as the 
        /// activity pub integration.
        /// 
        /// If this is set to null, Kakama's default profile
        /// page will be used.
        /// </summary>
        public Uri? ProfileUrl { get; set; } = null;
    }

    public static class ProfileExtensions
    {
        internal static void Validate( this Profile profile )
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

        public static Service ToActivityPubJson(
            this Profile profile,
            Namespace ns,
            RsaKey rsaKey,
            IEnumerable<ProfileMetaData> profileMetaData,
            Uri baseSiteUri
        )
        {
            if( profile.Slug is null )
            {
                throw new ArgumentException(
                    "Slug can not be null in the profile.",
                    nameof( profile )
                );
            }
            else if( profile.NamespaceId != ns.Id )
            {
                throw new ArgumentException(
                    $"Passed in namespace's ID of {ns.Id} does not match profile's namespace ID of {profile.NamespaceId}",
                    nameof( ns )
                );
            }
            else if( ns.Slug is null )
            {
                throw new ArgumentException(
                    "Slug can not be null in the namespace.",
                    nameof( ns )  
                );
            }
            else if( rsaKey.Id != profile.RsaKeyId )
            {
                throw new ArgumentException(
                    $"Passed in RSA Key's ID of {rsaKey.Id} does not match profile's RSA Key ID of {profile.RsaKeyId}",
                    nameof( rsaKey )
                );
            }
            else if( profileMetaData.Any( p => p.ProfileId != profile.Id ) )
            {
                throw new ArgumentException(
                    $"At least one passed in piece of profile metadata's ID does ot match the profile ID of {profile.Id}",
                    nameof( profileMetaData )
                );
            }

            // The URL is http://website.com/namespace/profile/
            // Things under profile include the stuff required for ActivityPub
            // (e.g. the json files).
            // index.html under the profile folder will be the human-readable
            // section of the profile.
            //
            // Uri type's ToString includes a '/' at the end, no need to include it.
            string profileFolderUrl = $"{baseSiteUri}{ns.Slug}/{profile.Slug}";

            // Activity Pub URLs
            string profileJsonUrl = $"{profileFolderUrl}/profile.json";
            string outboxUrl = $"{profileFolderUrl}/outbox.json";
            string inboxUrl = $"{profileFolderUrl}/inbox.json";
            string followingUrl = $"{profileFolderUrl}/following.json";
            string followersUrl = $"{profileFolderUrl}/followers.json";

            // Human Readable URLs
            Uri profileUrl = profile.ProfileUrl ?? new Uri( $"{profileFolderUrl}/index.html" );

            List<IObjectOrLink>? metadata;
            if( profileMetaData.Any() == false )
            {
                metadata = null;
            }
            else
            {
                metadata = new List<IObjectOrLink>();
                foreach( ProfileMetaData data in profileMetaData )
                {
                    metadata.Add(
                        new PropertyValue
                        {
                            Name = new string[] { data.Name },
                            Type = new string[] { "PropertyValue" },
                            Value = data.Value
                        }
                    );
                }
            }

            var key = new ProfilePublicKey
            {
                // ID Must match the Profile's ID.
                Id = $"{profile.Id}#main-key",
                Owner = profile.Id.ToString(),
                PublicKeyPem = rsaKey.PublicKeyPem
            };

            var extensionData = new Dictionary<string, JsonElement>
            {
                ["discoverable"] = JsonSerializer.SerializeToElement( true ),
                ["manuallyApprovesFollowers"] = JsonSerializer.SerializeToElement( false ),
                ["@context"] = JsonSerializer.SerializeToElement(
                    new object[]
                    {
                        "https://www.w3.org/ns/activitystreams",
                        "https://w3id.org/security/v1",
                        new PropertyValueSchema()
                        {
                            PropertyValue = "schema:PropertyValue",
                            Value = "schema:value"
                        }
                    }
                ),
                ["publicKey"] = JsonSerializer.SerializeToElement( key )
            };
        
            string[]? summary;
            if( profile.Description is null )
            {
                summary = null;
            }
            else
            {
                summary = new string[]
                {
                    profile.Description
                };
            }

            Image[]? icon;
            if( profile.ImageUrl is null )
            {
                icon = null;
            }
            else
            {
                icon = new Image[]
                {
                    new Image
                    {
                        Type = new string[]{ "Image" },
                        MediaType = $"image/{Path.GetExtension( profile.ImageUrl.ToString() ).TrimStart( '.' )}",
                        Url = new Link[]
                        {
                            new Link
                            {
                                Href = profile.ImageUrl
                            }
                        }
                    }
                };
            }

            var service = new Service
            {
                // ID must be the same as the URL to the profile JSON (its a self-reference).
                Id = profileJsonUrl,
                Type = new string[]{ "Service" },

                // URL in this context is the link to the human-readable URL,
                // not the profile json.
                Url = new Link[]
                { 
                    new Link
                    {
                        Href = profileUrl
                    }
                },
                Inbox = new Link
                {
                    Href = new Uri( inboxUrl )  
                },
                Outbox = new Link
                {
                    Href = new Uri( outboxUrl )
                },
                Following = new Link
                {
                    Href = new Uri( followingUrl )
                },
                Followers = new Link
                {
                    Href = new Uri( followersUrl )
                },

                // Profile Info
                PreferredUsername = profile.Slug,
                Name = new string[]
                {
                    profile.Name
                },
                Summary = summary,
                Published = profile.CreationTime,
                Icon = icon,
                Attachment = metadata,
                ExtensionData = extensionData
            };

            return service;
        }

        private class PropertyValue : KristofferStrube.ActivityStreams.Object
        {
            [JsonPropertyName("value")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public string? Value { get; set; }
        }

        private class PropertyValueSchema : ITermDefinition
        {
            [JsonPropertyName( "PropertyValue" )]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public string? PropertyValue { get; set; }

            [JsonPropertyName( "value" )]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public string? Value { get; set; }
        }

        private record class ProfilePublicKey
        {
            [JsonPropertyName( "id" )]
            [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
            public string? Id { get; init; }

            [JsonPropertyName( "owner" )]
            [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
            public string? Owner { get; init; }

            [JsonPropertyName( "publicKeyPem" )]
            [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
            public string? PublicKeyPem { get; init; }
        }
    }
}
