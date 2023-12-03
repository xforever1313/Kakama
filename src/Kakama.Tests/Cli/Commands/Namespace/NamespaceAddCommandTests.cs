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

namespace Kakama.Tests.Cli.Commands.Namespace
{
    [TestClass]
    [DoNotParallelize] // <- Makes sure we don't share the .db file on multiple threads.
    public sealed class NamespaceAddCommandTests
    {
        // ---------------- Fields ----------------

        private KakamaCliHarness? uut;

        // ---------------- Setup / Teardown ----------------

        [TestInitialize]
        public void TestSetup()
        {
            this.uut = new KakamaCliHarness( "namespaceclitests.db", "namespaceaddcli.env" );
            this.uut.PerformTestSetup();
        }

        [TestCleanup]
        public void TestTeardown()
        {
            this.uut?.PerformTestTearDown();
        }

        // ---------------- Properties ----------------

        public KakamaCliHarness Uut
        {
            get
            {
                Assert.IsNotNull( this.uut );
                return this.uut;
            }
        }

        // ---------------- Tests ----------------

        [TestMethod]
        public void AddWithOptionalsNotSpecified()
        {
            // Setup
            const string expectedName = "My Name";
            var args = new string[]
            {
                "namespace",
                "add",
                $"--name={expectedName}"
            };

            var expectedNs = new Kakama.Api.Models.Namespace
            {
                Id = 1,
                BaseUrl = null,
                Name = expectedName,
                Slug = "my-name"
            };

            // Act
            int exitCode = this.Uut.RunArgumentsWithDefaultGlobalSettings( args );
            Assert.AreEqual( 0, exitCode );

            // Check
            IEnumerable<Kakama.Api.Models.Namespace> allNamespaces = this.Uut.ApiHarness.NamespaceManager.GetAllNamespaces();
            Assert.AreEqual( 1, allNamespaces.Count() );

            Assert.AreEqual( expectedNs, allNamespaces.First() );
        }

        [TestMethod]
        public void AddWithOptionalsSpecified()
        {
            // Setup
            const string expectedName = "My Name";
            const string expectedSlug = "my-slug";
            Uri expectedBaseUri = new Uri( "https://shendrick.net" );

            var args = new string[]
            {
                "namespace",
                "add",
                $"--name={expectedName}",
                $"--slug={expectedSlug}",
                $"--base_uri={expectedBaseUri}"
            };

            var expectedNs = new Kakama.Api.Models.Namespace
            {
                Id = 1,
                BaseUrl = expectedBaseUri,
                Name = expectedName,
                Slug = expectedSlug
            };

            // Act
            int exitCode = this.Uut.RunArgumentsWithDefaultGlobalSettings( args );
            Assert.AreEqual( 0, exitCode );

            // Check
            IEnumerable<Kakama.Api.Models.Namespace> allNamespaces = this.Uut.ApiHarness.NamespaceManager.GetAllNamespaces();
            Assert.AreEqual( 1, allNamespaces.Count() );

            Assert.AreEqual( expectedNs, allNamespaces.First() );
        }
    }
}
