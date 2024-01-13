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
using SethCS.Exceptions;

namespace Kakama.Tests.Api.Models
{
    [TestClass]
    public sealed class NamespaceTests
    {
        // ---------------- Tests ----------------

        [TestMethod]
        public void ValidateTest()
        {
            var uut = new Namespace
            {
                Id = 0,
                BaseUrl = new Uri( "https://shendrick.net" ),
                Name = "xforever1313",
                Slug = "xforever_1313"
            };
            var backup = uut with { };

            // Should verify out the gate:
            uut.Validate();
            uut = backup with { };

            // Null URL is fine.
            uut.BaseUrl = null;
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

            // 0 ID is okay, it means add new namespace.
            uut.Id = 0;
            uut.Validate();
            uut = backup with { };

            // Positive ID is okay.
            uut.Id = 1;
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
