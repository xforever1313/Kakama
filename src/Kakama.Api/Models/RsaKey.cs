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
using System.Security.Cryptography;
using SethCS.Exceptions;

namespace Kakama.Api.Models
{
    public record class RsaKey
    {
        /// <summary>
        /// The RSA key ID.  If set to the default,
        /// then a new profile is added.  Otherwise,
        /// they key is modified.
        /// </summary>
        [Key]
        [Required]
        public int Id { get; internal set; }

        [Required]
        public string PublicKeyPem { get; set; } = "";

        [Required]
        public string PrivateKeyPem { get; set; } = "";

        // ---------------- Functions ----------------

        /// <summary>
        /// Genreates a new <see cref="RSA"/> 2048 key
        /// and sets <see cref="PublicKeyPem"/> and <see cref="PrivateKeyPem"/>.
        /// </summary>
        /// <returns>
        /// This instance of <see cref="RsaKey"/> so calls can be chained.
        /// </returns>
        public RsaKey GenerateKey()
        {
            using var rsa = RSA.Create( 2048 );
            return SetKey( rsa );
        }

        /// <returns>
        /// This instance of <see cref="RsaKey"/> so calls can be chained.
        /// </returns>
        public RsaKey SetKey( string privateKeyPem )
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem( privateKeyPem );
            return SetKey( rsa );
        }

        /// <returns>
        /// This instance of <see cref="RsaKey"/> so calls can be chained.
        /// </returns>
        public RsaKey SetKey( RSA rsa )
        {
            this.PublicKeyPem = rsa.ExportRSAPublicKeyPem();
            this.PrivateKeyPem = rsa.ExportPkcs8PrivateKeyPem();

            return this;
        }
    }

    internal static class RsaKeyExtensions
    {
        /// <summary>
        /// Tries to make sure the given key is valid.
        /// </summary>
        /// <returns>
        /// An empty list if there are no validation errors,
        /// otherwise a non-empty list to show that there are validation errors.
        /// </returns>
        public static IEnumerable<string> TryValidate( this RsaKey key )
        {
            var errors = new List<string>();

            if( key.Id < 0 )
            {
                errors.Add( $"{nameof( RsaKey )} ID can not be less than zero, got: {key.Id}." );
            }

            if( string.IsNullOrWhiteSpace( key.PublicKeyPem ) )
            {
                errors.Add( $"{nameof( RsaKey )}.{nameof( key.PublicKeyPem )} can not be null, empty, or whitespace." );
            }

            if( string.IsNullOrWhiteSpace( key.PrivateKeyPem ) )
            {
                errors.Add( $"{nameof( RsaKey )}.{nameof( key.PrivateKeyPem )} can not be null, empty, or whitespace." );
            }

            return errors;
        }

        public static void Validate( this RsaKey key )
        {
            IEnumerable<string> errors = key.TryValidate();

            if( errors.Any() )
            {
                throw new ListedValidationException( "Errors when validating RSA Key.", errors );
            }
        }
    }
}
