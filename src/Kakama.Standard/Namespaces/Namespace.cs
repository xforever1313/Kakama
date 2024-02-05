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

namespace Kakama.Standard.Namespaces
{
    /// <summary>
    /// A namespace contains profiles.
    /// Namespaces are useful if you want to use this as a monolith
    /// and manage multiple different types of bots instead
    /// of using a more micro-service architecture.
    /// </summary>
    public record class Namespace
    {
        /// <summary>
        /// The namespace ID.  If set to the default,
        /// then a new namespace is added.  Otherwise,
        /// a namespace is modified.
        /// </summary>
        [Key]
        public int Id { get; internal set; }

        /// <summary>
        /// The namespace name.
        /// </summary>
        [Required]
        public string Name { get; set; } = "";

        /// <summary>
        /// The slug of the URL for this namespace.
        /// If this is set to null, empty, or just whitespace,
        /// then it will become the slug of <see cref="Name"/>
        /// </summary>
        public string? Slug { get; set; }

        /// <summary>
        /// If this is set to null, then the namespace
        /// is not limited to a specific URI.  Therefore,
        /// and URL that points to this service can return
        /// a namespace when requested.
        /// 
        /// However, if this is set, the namespace is tied
        /// to a specific URI, and therefore it can not be returned
        /// if the requested URI does not match this.
        /// 
        /// This is really meant to be used when this is used as a monolith
        /// service serving multiple URLs at once.
        /// 
        /// When in doubt, leave this null; but make sure
        /// the base web config is set to something if that's the case.
        /// </summary>
        public Uri? BaseUri { get; set; } = null;
    }
}
