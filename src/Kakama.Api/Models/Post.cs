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

namespace Kakama.Api.Models
{
    /// <summary>
    /// Represets a post or a note (if using ActivityPub terms).
    /// This is what the bot sends out.
    /// </summary>
    public record class Post
    {
        /// <summary>
        /// The post ID.  If set to the default,
        /// then a new post is added.  Otherwise,
        /// a post is modified.
        /// </summary>
        [Key]
        [Required]
        public int Id { get; internal set; }

        /// <summary>
        /// The profile that posted this post.
        /// </summary>
        [ForeignKey( "ProfileId" )]
        [Required]
        public int ProfileId { get; internal set; }

        /// <summary>
        /// The message to post about.
        /// This can not be an empty string.
        /// </summary>
        [Required]
        public string Message { get; internal set; } = "";
    }
}
