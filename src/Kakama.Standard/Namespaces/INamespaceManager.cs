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

namespace Kakama.Standard.Namespaces
{
    public interface INamespaceManager
    {
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
        int ConfigureNamespace( Namespace ns );

        IEnumerable<Namespace> GetAllNamespaces();

        List<Namespace> GetNamespacesCompatibleWithUrl( Uri uri );

        Namespace? TryGetNamespaceBySlug( string slug );

        Namespace GetNamespaceBySlug( string slug );

        Namespace? TryGetNamespaceById( int id );

        Namespace GetNamespaceById( int id );

        /// <summary>
        /// Does the given namespace by id exists?
        /// </summary>
        bool NamespaceExists( int id );

        int GetNumberOfNamespaces();
    }
}