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

using System.Windows.Markup;
using Kakama.Api.Models;

namespace Kakama.Tests.Api
{
    [TestClass]
    [DoNotParallelize] // <- Makes sure we don't share the .db file on multiple threads.
    public sealed class NamespaceManagerTests
    {
        // ---------------- Fields ----------------

        private KakamaApiHarness? uut;

        // ---------------- Setup / Teardown ----------------

        [TestInitialize]
        public void TestSetup()
        {
            this.uut = new KakamaApiHarness( "namespacetests.db" );
            this.uut.PerformTestSetup();
        }

        [TestCleanup]
        public void TestTeardown()
        {
            this.uut?.PerformTestTearDown();
        }

        // ---------------- Properties ----------------

        public KakamaApiHarness Uut
        {
            get
            {
                Assert.IsNotNull( this.uut );
                return this.uut;
            }
        }

        // ---------------- Tests ----------------

        [TestMethod]
        public void AddNewNamespaceTest()
        {
            // Setup
            var ns = new Namespace
            {
                BaseUrl = new Uri( "https://shendrick.net" ),
                Name = "xforever1313",
                Slug = null
            };

            var expectedNs = ns with
            {
                Slug = ns.Name,
                Id = 1
            };

            // Act
            int id = this.Uut.NamespaceManager.ConfigureNamespace( ns );
            Namespace nsById = this.Uut.NamespaceManager.GetNamespaceById( id );
            Namespace nsBySlug = this.Uut.NamespaceManager.GetNamespaceBySlug( ns.Name );

            // Check

            Assert.AreEqual( expectedNs.Id, id );
            Assert.AreEqual( expectedNs, nsById );
            Assert.AreEqual( expectedNs, nsBySlug );
        }
    }
}
