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

using Kakama.Standard.Namespaces;

namespace Kakama.Tests.Cli.Commands.Namespaces
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
            Namespace startingNs = AddStartingNamespaceToDb();

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
            Namespace actualNs = this.Uut.ApiHarness.NamespaceManager.GetNamespaceById( startingNs.Id );
            Assert.AreEqual( startingNs, actualNs );
        }

        [TestMethod]
        public void ModifyNameTest()
        {
            // Setup
            Namespace startingNs = AddStartingNamespaceToDb();

            var args = new string[]
            {
                "namespace",
                "modify",
                $"--id={startingNs.Id}",
                "--name=New Name"
            };

            Namespace expectedNs = startingNs with
            {
                Name = "New Name"
            };

            // Act
            int exitCode = this.Uut.RunArgumentsWithDefaultGlobalSettings( args );

            // Check
            Assert.AreEqual( 0, exitCode );

            Namespace actualNs = this.Uut.ApiHarness.NamespaceManager.GetNamespaceById( startingNs.Id );
            Assert.AreEqual( expectedNs, actualNs );
        }

        [TestMethod]
        public void ModifySlugTest()
        {
            // Setup
            Namespace startingNs = AddStartingNamespaceToDb();

            var args = new string[]
            {
                "namespace",
                "modify",
                $"--id={startingNs.Id}",
                "--slug=new-slug"
            };

            Namespace expectedNs = startingNs with
            {
                Slug = "new-slug"
            };

            // Act
            int exitCode = this.Uut.RunArgumentsWithDefaultGlobalSettings( args );

            // Check
            Assert.AreEqual( 0, exitCode );

            Namespace actualNs = this.Uut.ApiHarness.NamespaceManager.GetNamespaceById( startingNs.Id );
            Assert.AreEqual( expectedNs, actualNs );
        }

        [TestMethod]
        public void ClearSlugTest()
        {
            // Setup
            Namespace startingNs = AddStartingNamespaceToDb();

            var args = new string[]
            {
                "namespace",
                "modify",
                $"--id={startingNs.Id}",
                "--slug=_"
            };

            Namespace expectedNs = startingNs with
            {
                // Will become null, and therefore before
                // a version of the name.
                Slug = "my-namespace"
            };

            // Act
            int exitCode = this.Uut.RunArgumentsWithDefaultGlobalSettings( args );

            // Check
            Assert.AreEqual( 0, exitCode );

            Namespace actualNs = this.Uut.ApiHarness.NamespaceManager.GetNamespaceById( startingNs.Id );
            Assert.AreEqual( expectedNs, actualNs );
        }

        [TestMethod]
        public void ModifyBaseUriTest()
        {
            // Setup
            Namespace startingNs = AddStartingNamespaceToDb();

            var args = new string[]
            {
                "namespace",
                "modify",
                $"--id={startingNs.Id}",
                "--base_uri=https://troop53stories.shendrick.net"
            };

            Namespace expectedNs = startingNs with
            {
                BaseUri = new Uri( "https://troop53stories.shendrick.net" )
            };

            // Act
            int exitCode = this.Uut.RunArgumentsWithDefaultGlobalSettings( args );

            // Check
            Assert.AreEqual( 0, exitCode );

            Namespace actualNs = this.Uut.ApiHarness.NamespaceManager.GetNamespaceById( startingNs.Id );
            Assert.AreEqual( expectedNs, actualNs );
        }

        [TestMethod]
        public void ClearBaseUriTest()
        {
            // Setup
            Namespace startingNs = AddStartingNamespaceToDb();

            var args = new string[]
            {
                "namespace",
                "modify",
                $"--id={startingNs.Id}",
                "--base_uri=_"
            };

            Namespace expectedNs = startingNs with
            {
                BaseUri = null
            };

            // Act
            int exitCode = this.Uut.RunArgumentsWithDefaultGlobalSettings( args );

            // Check
            Assert.AreEqual( 0, exitCode );

            Namespace actualNs = this.Uut.ApiHarness.NamespaceManager.GetNamespaceById( startingNs.Id );
            Assert.AreEqual( expectedNs, actualNs );
        }

        // ---------------- Test Helpers ----------------

        private Namespace AddStartingNamespaceToDb()
        {
            var startingNs = new Namespace
            {
                Id = 0,
                BaseUri = new Uri( "https://shendrick.net" ),
                Name = "My Namespace",
                Slug = "some-slug"
            };

            int id = this.Uut.ApiHarness.NamespaceManager.ConfigureNamespace( startingNs );

            startingNs = startingNs with { Id = id };

            return startingNs;
        }
    }
}
