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
    public sealed class NamespaceModifiyCommandTests
    {
        // ---------------- Fields ----------------

        private KakamaCliHarness? uut;

        // ---------------- Setup / Teardown ----------------

        [TestInitialize]
        public void TestSetup()
        {
            this.uut = new KakamaCliHarness( "namespacemodifytests.db", "namespacemodifycli.env" );
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
        public void ModifyAllButNoIdSpecified()
        {
            // Setup
            Kakama.Api.Models.Namespace startingNs = AddStartingNamespaceToDb();

            var args = new string[]
            {
                "namespace",
                "modify",
                // No ID specified.
                "--name=name",
                "--slug=slug",
                "--base_uri=https://shendrick.net"
            };

            // Act
            int exitCode = this.Uut.RunArgumentsWithDefaultGlobalSettings( args );

            // Check
            Assert.AreNotEqual( 0, exitCode );

            // Make sure our existing namespace was not modified.
            Kakama.Api.Models.Namespace actualNs = this.Uut.ApiHarness.NamespaceManager.GetNamespaceById( startingNs.Id );
            Assert.AreEqual( startingNs, actualNs );
        }

        [TestMethod]
        public void ModifyNameTest()
        {
            // Setup
            Kakama.Api.Models.Namespace startingNs = AddStartingNamespaceToDb();

            var args = new string[]
            {
                "namespace",
                "modify",
                $"--id={startingNs.Id}",
                "--name=New Name"
            };

            Kakama.Api.Models.Namespace expectedNs = startingNs with
            {
                Name = "New Name"
            };

            // Act
            int exitCode = this.Uut.RunArgumentsWithDefaultGlobalSettings( args );

            // Check
            Assert.AreEqual( 0, exitCode );

            Kakama.Api.Models.Namespace actualNs = this.Uut.ApiHarness.NamespaceManager.GetNamespaceById( startingNs.Id );
            Assert.AreEqual( expectedNs, actualNs );
        }

        [TestMethod]
        public void ModifySlugTest()
        {
            // Setup
            Kakama.Api.Models.Namespace startingNs = AddStartingNamespaceToDb();

            var args = new string[]
            {
                "namespace",
                "modify",
                $"--id={startingNs.Id}",
                "--slug=new-slug"
            };

            Kakama.Api.Models.Namespace expectedNs = startingNs with
            {
                Slug = "new-slug"
            };

            // Act
            int exitCode = this.Uut.RunArgumentsWithDefaultGlobalSettings( args );

            // Check
            Assert.AreEqual( 0, exitCode );

            Kakama.Api.Models.Namespace actualNs = this.Uut.ApiHarness.NamespaceManager.GetNamespaceById( startingNs.Id );
            Assert.AreEqual( expectedNs, actualNs );
        }

        [TestMethod]
        public void ClearSlugTest()
        {
            // Setup
            Kakama.Api.Models.Namespace startingNs = AddStartingNamespaceToDb();

            var args = new string[]
            {
                "namespace",
                "modify",
                $"--id={startingNs.Id}",
                "--slug=_"
            };

            Kakama.Api.Models.Namespace expectedNs = startingNs with
            {
                // Will become null, and therefore before
                // a version of the name.
                Slug = "my-namespace"
            };

            // Act
            int exitCode = this.Uut.RunArgumentsWithDefaultGlobalSettings( args );

            // Check
            Assert.AreEqual( 0, exitCode );

            Kakama.Api.Models.Namespace actualNs = this.Uut.ApiHarness.NamespaceManager.GetNamespaceById( startingNs.Id );
            Assert.AreEqual( expectedNs, actualNs );
        }

        [TestMethod]
        public void ModifyBaseUriTest()
        {
            // Setup
            Kakama.Api.Models.Namespace startingNs = AddStartingNamespaceToDb();

            var args = new string[]
            {
                "namespace",
                "modify",
                $"--id={startingNs.Id}",
                "--base_uri=https://troop53stories.shendrick.net"
            };

            Kakama.Api.Models.Namespace expectedNs = startingNs with
            {
                BaseUrl = new Uri( "https://troop53stories.shendrick.net" )
            };

            // Act
            int exitCode = this.Uut.RunArgumentsWithDefaultGlobalSettings( args );

            // Check
            Assert.AreEqual( 0, exitCode );

            Kakama.Api.Models.Namespace actualNs = this.Uut.ApiHarness.NamespaceManager.GetNamespaceById( startingNs.Id );
            Assert.AreEqual( expectedNs, actualNs );
        }

        [TestMethod]
        public void ClearBaseUriTest()
        {
            // Setup
            Kakama.Api.Models.Namespace startingNs = AddStartingNamespaceToDb();

            var args = new string[]
            {
                "namespace",
                "modify",
                $"--id={startingNs.Id}",
                "--base_uri=_"
            };

            Kakama.Api.Models.Namespace expectedNs = startingNs with
            {
                BaseUrl = null
            };

            // Act
            int exitCode = this.Uut.RunArgumentsWithDefaultGlobalSettings( args );

            // Check
            Assert.AreEqual( 0, exitCode );

            Kakama.Api.Models.Namespace actualNs = this.Uut.ApiHarness.NamespaceManager.GetNamespaceById( startingNs.Id );
            Assert.AreEqual( expectedNs, actualNs );
        }

        // ---------------- Test Helpers ----------------

        private Kakama.Api.Models.Namespace AddStartingNamespaceToDb()
        {
            var startingNs = new Kakama.Api.Models.Namespace
            {
                Id = 0,
                BaseUrl = new Uri( "https://shendrick.net" ),
                Name = "My Namespace",
                Slug = "some-slug"
            };

            int id = this.Uut.ApiHarness.NamespaceManager.ConfigureNamespace( startingNs );

            startingNs = startingNs with { Id = id };

            return startingNs;
        }
    }
}
