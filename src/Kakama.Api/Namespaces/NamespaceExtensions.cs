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

using Kakama.Standard.Namespaces;
using SethCS.Exceptions;

namespace Kakama.Api.Namespaces
{
    internal static class NamespaceExtensions
    {
        public static void Validate( this Namespace ns )
        {
            var errors = new List<string>();

            if( ns.Id < 0 )
            {
                errors.Add( $"ID can not be less than zero, got: {ns.Id}." );
            }

            if( string.IsNullOrWhiteSpace( ns.Name ) )
            {
                errors.Add( $"Namespace name can not be null, empty, or whitespace" );
            }

            if( errors.Any() )
            {
                throw new ListedValidationException( "Errors when validating namespace", errors );
            }
        }
    }
}