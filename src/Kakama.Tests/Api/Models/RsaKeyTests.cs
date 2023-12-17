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
    public sealed class RsaKeyTests
    {
        // ---------------- Tests ----------------

        /// <summary>
        /// Makes sure that we can import keys
        /// from PEM.
        /// </summary>
        [TestMethod]
        public void KeyRoundTripTest()
        {
            // Setup
            var uut1 = new RsaKey();
            uut1.GenerateKey();

            // Act
            var uut2 = new RsaKey();
            uut2.SetKey( uut1.PrivateKeyPem );

            // Check
            Assert.AreEqual( uut1, uut2 );
            Assert.AreEqual( uut1.PublicKeyPem, uut2.PublicKeyPem );
            Assert.AreEqual( uut2.PrivateKeyPem, uut2.PrivateKeyPem );
        }

        [TestMethod]
        public void ValidateTest()
        {
            var uut = new RsaKey().GenerateKey();
            RsaKey backup = uut with { };

            // Should not fail to validate when generated.
            uut.Validate();

            // ID less than 0 is invalid.
            uut.Id = -1;
            Assert.ThrowsException<ListedValidationException>( () => uut.Validate() );
            uut = backup with { };

            // Empty public key not valid.
            uut.PublicKeyPem = "";
            Assert.ThrowsException<ListedValidationException>( () => uut.Validate() );
            uut = backup with { };

            // Empty private key not valid.
            uut.PrivateKeyPem = "";
            Assert.ThrowsException<ListedValidationException>( () => uut.Validate() );
            uut = backup with { };
        }
    }
}
