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

namespace Kakama.Api.Models
{
    /// <summary>
    /// Meta data for a profile.  This is usually
    /// a key-value pair.  These show up as account headers in Mastodon,
    /// for example.
    /// 
    /// These can include stuff like websites and other information.
    /// </summary>
    public record class ProfileMetaData
    {
        /// <summary>
        /// The ID.  If this is set to the default,
        /// a new profile metadata will be added.
        /// Otherwise, it will be modified.
        /// </summary>
        [Key]
        [Required]
        public int Id { get; internal set; }

        /// <summary>
        /// The <see cref="Profile"/>
        /// this meta data belongs to.
        /// </summary>
        [ForeignKey( "ProfileId" )]
        public int ProfileId { get; set; }

        /// <summary>
        /// The "key" or name of the meta data.
        /// </summary>
        [Required]
        public string Name { get; set; } = "";

        /// <summary>
        /// The value of the meta data.
        /// </summary>
        [Required]
        public string Value { get; set; } = "";
    }
}
