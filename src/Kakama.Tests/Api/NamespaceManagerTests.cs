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

using Kakama.Api;
using Kakama.Api.Models;
using SethCS.Exceptions;

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

        [TestMethod]
        public void AddNewNamespaceSlugTest()
        {
            // Setup
            var ns = new Namespace
            {
                BaseUrl = new Uri( "https://shendrick.net" ),
                Name = "xforever 1313",
                Slug = null
            };

            var expectedNs = ns with
            {
                Slug = "xforever-1313",
                Id = 1
            };

            // Act
            int id = this.Uut.NamespaceManager.ConfigureNamespace( ns );
            Namespace nsById = this.Uut.NamespaceManager.GetNamespaceById( id );
            Namespace nsBySlug = this.Uut.NamespaceManager.GetNamespaceBySlug( expectedNs.Slug );

            // Check
            Assert.AreEqual( expectedNs.Id, id );
            Assert.AreEqual( expectedNs, nsById );
            Assert.AreEqual( expectedNs, nsBySlug );
        }

        [TestMethod]
        public void AddMultipleNamespaceTest()
        {
            // Setup
            var ns1 = new Namespace
            {
                BaseUrl = new Uri( "https://shendrick.net" ),
                Name = "xforever1313",
                Slug = null
            };

            var expectedNs1 = ns1 with
            {
                Slug = ns1.Name,
                Id = 1
            };

            var ns2 = new Namespace
            {
                Name = "shendrick",
                Slug = "s_hendrick"
            };

            var expectedNs2 = ns2 with
            {
                Id = 2
            };

            // Act
            int id1 = this.Uut.NamespaceManager.ConfigureNamespace( ns1 );
            int id2 = this.Uut.NamespaceManager.ConfigureNamespace( ns2 );

            Namespace nsById1 = this.Uut.NamespaceManager.GetNamespaceById( id1 );
            Namespace nsBySlug1 = this.Uut.NamespaceManager.GetNamespaceBySlug( ns1.Name );

            Namespace nsById2 = this.Uut.NamespaceManager.GetNamespaceById( id2 );
            Namespace nsBySlug2 = this.Uut.NamespaceManager.GetNamespaceBySlug( ns2.Slug );

            IEnumerable<Namespace> allNamespaes = this.Uut.NamespaceManager.GetAllNamespaces();

            // Check
            Assert.AreEqual( expectedNs1.Id, id1 );
            Assert.AreEqual( expectedNs1, nsById1 );
            Assert.AreEqual( expectedNs1, nsBySlug1 );

            Assert.AreEqual( expectedNs2.Id, id2 );
            Assert.AreEqual( expectedNs2, nsById2 );
            Assert.AreEqual( expectedNs2, nsBySlug2 );

            Assert.AreEqual( 2, allNamespaes.Count() );
            Assert.AreEqual( expectedNs1, allNamespaes.First() );
            Assert.AreEqual( expectedNs2, allNamespaes.Last() );
        }

        [TestMethod]
        public void ModifyNamespaceTest()
        {
            // Setup
            var ns = new Namespace
            {
                BaseUrl = new Uri( "https://shendrick.net" ),
                Name = "xforever1313",
                Slug = null
            };

            this.Uut.NamespaceManager.ConfigureNamespace( ns );

            ns = ns with
            {
                BaseUrl = null,
                Name = "shendrick",
                Slug = "someslug"
            };

            Namespace expectedNs = ns with { };

            // Act
            int id = this.Uut.NamespaceManager.ConfigureNamespace( ns );
            Namespace nsById = this.Uut.NamespaceManager.GetNamespaceById( ns.Id );
            Namespace nsBySlug = this.Uut.NamespaceManager.GetNamespaceBySlug( ns.Slug );
            IEnumerable<Namespace> allNamespaes = this.Uut.NamespaceManager.GetAllNamespaces();

            // Check
            Assert.AreEqual( expectedNs.Id, id );
            Assert.AreEqual( expectedNs, nsById );
            Assert.AreEqual( expectedNs, nsBySlug );

            Assert.AreEqual( 1, allNamespaes.Count() );
            Assert.AreEqual( expectedNs, allNamespaes.First() );
        }

        [TestMethod]
        public void ModifyNamespaceWithSameSlugTest()
        {
            // Setup
            var ns = new Namespace
            {
                BaseUrl = new Uri( "https://shendrick.net" ),
                Name = "xforever1313",
                Slug = "someslug"
            };

            this.Uut.NamespaceManager.ConfigureNamespace( ns );

            ns = ns with
            {
                BaseUrl = null
            };

            Namespace expectedNs = ns with { };

            // Act
            int id = this.Uut.NamespaceManager.ConfigureNamespace( ns );
            Namespace nsById = this.Uut.NamespaceManager.GetNamespaceById( ns.Id );
            Namespace nsBySlug = this.Uut.NamespaceManager.GetNamespaceBySlug( ns.Slug );
            IEnumerable<Namespace> allNamespaes = this.Uut.NamespaceManager.GetAllNamespaces();

            // Check
            Assert.AreEqual( expectedNs.Id, id );
            Assert.AreEqual( expectedNs, nsById );
            Assert.AreEqual( expectedNs, nsBySlug );

            Assert.AreEqual( 1, allNamespaes.Count() );
            Assert.AreEqual( expectedNs, allNamespaes.First() );
        }

        [TestMethod]
        public void NamespaceDoesntExistTest()
        {
            // Act / Check
            Assert.ThrowsException<NamespaceNotFoundException>(
                () => this.Uut.NamespaceManager.GetNamespaceBySlug( "hello" )
            );

            Assert.ThrowsException<NamespaceNotFoundException>(
                () => this.Uut.NamespaceManager.GetNamespaceById( 1 )
            );
        }

        [TestMethod]
        public void InvalidNamespaceThrowsOnConfigureTest()
        {
            // Setup
            var ns = new Namespace
            {
                Name = ""
            };

            // Act
            Assert.ThrowsException<ListedValidationException>(
                () => this.Uut.NamespaceManager.ConfigureNamespace( ns )
            );

            // Check
            Assert.AreEqual( 0, this.Uut.NamespaceManager.GetAllNamespaces().Count() );
        }

        [TestMethod]
        public void DuplicateSlugTest()
        {
            // Setup
            var ns1 = new Namespace
            {
                BaseUrl = new Uri( "https://shendrick.net" ),
                Name = "xforever1313",
                Slug = "xforever1313"
            };

            var ns2 = ns1 with 
            {
                Name = "shendrick",
            };

            this.Uut.NamespaceManager.ConfigureNamespace( ns1 );

            // Act 
            Assert.ThrowsException<ValidationException>(
                () => this.Uut.NamespaceManager.ConfigureNamespace( ns2 )
            );

            // Check - Make sure database did not get updated.
            Assert.AreEqual( ns1, this.Uut.NamespaceManager.GetNamespaceById( ns1.Id ) );
            Assert.AreEqual( 1, this.Uut.NamespaceManager.GetAllNamespaces().Count() );
        }
    }
}
