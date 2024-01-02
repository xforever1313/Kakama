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
    public sealed class ProfileManagerTests
    {
        // ---------------- Fields ----------------

        private KakamaApiHarness? uut;

        // ---------------- Setup / Teardown ----------------

        [TestInitialize]
        public void TestSetup()
        {
            this.uut = new KakamaApiHarness( "profiletests.db" );
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
        public void AddProfileToNamespaceThatDoestExistTest()
        {
            // Setup
            var profile = new Profile
            {
                Name = "My Name",
                Description = "Test Profile",
                NamespaceId = 1
            };

            // Act
            Assert.ThrowsException<NamespaceNotFoundException>(
                () => this.Uut.ProfileManager.ConfigureProfile( profile )
            );

            // Check

            // Profile should not have been added.
            Assert.AreEqual( 0, this.Uut.ProfileManager.GetNumberOfProfiles() );
            Assert.AreEqual( 0, profile.Id );
        }

        [TestMethod]
        public void AddProfileWithNoSlugTest()
        {
            // Setup
            Namespace ns = AddTestNamespace();
            var profile = new Profile
            {
                Name = "My Profile",
                NamespaceId = ns.Id
            };

            var expectedProfile = profile with
            {
                Id = 1,
                RsaKeyId = 1,

                // Slug should auto-generate is null by default.
                Slug = "my-profile"
            };

            // Act
            int profileId = this.Uut.ProfileManager.ConfigureProfile( profile );

            // Check
            Assert.AreEqual( 1, profileId );
            Assert.IsTrue( this.Uut.ProfileManager.ProfileExists( profileId ) );
            Assert.AreEqual( 1, this.Uut.ProfileManager.GetNumberOfProfiles() );
            Assert.AreEqual( 1, this.Uut.ProfileManager.GetTotalNumberOfProfilesInNamespace( ns.Id ) );

            Assert.AreEqual( expectedProfile, profile );
            Assert.AreEqual( expectedProfile, this.Uut.ProfileManager.GetProfileById( profileId ) );
            Assert.AreEqual( expectedProfile, this.Uut.ProfileManager.GetProfileBySlug( ns.Id, expectedProfile.Slug ) );

            RsaKey rsaKey = this.Uut.ProfileManager.GetRsaKey( profileId );
            Assert.AreEqual( 1, rsaKey.Id );
            Assert.IsFalse( string.IsNullOrWhiteSpace( rsaKey.PublicKeyPem ) );
            Assert.IsFalse( string.IsNullOrWhiteSpace( rsaKey.PrivateKeyPem ) );
        }

        [TestMethod]
        public void AddProfileWithSlugTest()
        {
            // Setup
            Namespace ns = AddTestNamespace();
            var profile = new Profile
            {
                Name = "My Profile",
                Description = "Some Description",
                NamespaceId = ns.Id,
                ImageUrl = new Uri( "https://shendrick.net/static/img/me.jpg" ),
                Slug = "some-slug"
            };

            var expectedProfile = profile with
            {
                Id = 1,
                RsaKeyId = 1
            };

            // Act
            int profileId = this.Uut.ProfileManager.ConfigureProfile( profile );

            // Check
            Assert.AreEqual( 1, profileId );
            Assert.IsTrue( this.Uut.ProfileManager.ProfileExists( profileId ) );
            Assert.AreEqual( 1, this.Uut.ProfileManager.GetNumberOfProfiles() );
            Assert.AreEqual( 1, this.Uut.ProfileManager.GetTotalNumberOfProfilesInNamespace( ns.Id ) );

            Assert.AreEqual( expectedProfile, profile );
            Assert.AreEqual( expectedProfile, this.Uut.ProfileManager.GetProfileById( profileId ) );
            Assert.AreEqual( expectedProfile, this.Uut.ProfileManager.GetProfileBySlug( ns.Id, expectedProfile.Slug ) );

            RsaKey rsaKey = this.Uut.ProfileManager.GetRsaKey( profileId );
            Assert.AreEqual( 1, rsaKey.Id );
            Assert.IsFalse( string.IsNullOrWhiteSpace( rsaKey.PublicKeyPem ) );
            Assert.IsFalse( string.IsNullOrWhiteSpace( rsaKey.PrivateKeyPem ) );
        }

        [TestMethod]
        public void RekeyTest()
        {
            // Create Profile
            Namespace ns = AddTestNamespace();
            var profile = new Profile
            {
                Name = "My Profile",
                NamespaceId = ns.Id
            };

            int profileId = this.Uut.ProfileManager.ConfigureProfile( profile );

            Assert.IsTrue( this.Uut.ProfileManager.ProfileExists( profileId ) );

            // Setup
            RsaKey rsaKey = this.Uut.ProfileManager.GetRsaKey( profileId );
            Assert.AreEqual( 1, rsaKey.Id );
            string oldPrivatePem = rsaKey.PrivateKeyPem;
            string oldPublicPem = rsaKey.PublicKeyPem;

            // Act
            this.Uut.ProfileManager.RegenerateKey( profileId );
            
            // Check
            RsaKey newKey = this.Uut.ProfileManager.GetRsaKey( profileId );
            Assert.AreEqual( 1, newKey.Id );
            Assert.AreNotEqual( oldPrivatePem, newKey.PrivateKeyPem );
            Assert.AreNotEqual( oldPublicPem, newKey.PublicKeyPem );
        }

        // ---------------- Test Helpers ----------------

        /// <summary>
        /// Adds a test namespace to the database
        /// and returns it.
        /// </summary>
        private Namespace AddTestNamespace()
        {
            var ns = new Namespace
            {
                Name = "Test Namespace"
            };

            this.Uut.NamespaceManager.ConfigureNamespace( ns );

            return ns;
        }
    }
}
