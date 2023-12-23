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

using Kakama.Api.Models;
using SethCS.Exceptions;

namespace Kakama.Tests.Api.Models
{
    [TestClass]
    public class ProfileTests
    {
        // ---------------- Tests ----------------

        [TestMethod]
        public void ValidateTest()
        {
            var uut = new Profile
            {
                Id = 0,
                Description = "My Profile",
                ImageUrl = new Uri( "https://shendrick.net/static/img/me.jpg" ),
                Name = "xforever1313",
                NamespaceId = 1,
                RsaKeyId = 1,
                Slug = "xforever-1313"
            };

            var backup = uut with { };

            // Should verify out the gate:
            uut.Validate();
            uut = backup with { };

            // Null URL is fine.
            uut.ImageUrl = null;
            uut.Validate();
            uut = backup with { };

            // Null slug is fine.
            uut.Slug = null;
            uut.Validate();
            uut = backup with { };

            // Empty slug is fine; we'll treat it as null.
            uut.Slug = "";
            uut.Validate();
            uut = backup with { };

            // Whitespace slug is fine; we'll treat it as null.
            uut.Slug = "    ";
            uut.Validate();
            uut = backup with { };

            // Negative ID is not okay.
            uut.Id = -1;
            Assert.ThrowsException<ListedValidationException>( () => uut.Validate() );
            uut = backup with { };

            // 0 ID is okay, means add new profile.
            uut.Id = 0;
            uut.Validate();
            uut = backup with { };

            // 1 ID is okay, means profile exists.
            uut.Id = 1;
            uut.Validate();
            uut = backup with { };

            // Negative Namespace ID is not okay.
            uut.NamespaceId = -1;
            Assert.ThrowsException<ListedValidationException>( () => uut.Validate() );
            uut = backup with { };

            // 0 Namespace ID is not okay.
            uut.NamespaceId = 0;
            Assert.ThrowsException<ListedValidationException>( () => uut.Validate() );
            uut = backup with { };

            // Negative RSA Key is not okay.
            uut.RsaKeyId = -1;
            Assert.ThrowsException<ListedValidationException>( () => uut.Validate() );
            uut = backup with { };

            // 0 RSA Key is okay (means generate new key).
            uut.RsaKeyId = 0;
            uut.Validate();
            uut = backup with { };

            // Positive RSA Key is okay (means key exists).
            uut.RsaKeyId = 1;
            uut.Validate();
            uut = backup with { };

            // Empty string name is not okay.
            uut.Name = "";
            Assert.ThrowsException<ListedValidationException>( () => uut.Validate() );
            uut = backup with { };

            // Whitespace name is not okay.
            uut.Name = "    ";
            Assert.ThrowsException<ListedValidationException>( () => uut.Validate() );
            uut = backup with { };
        }
    }
}
