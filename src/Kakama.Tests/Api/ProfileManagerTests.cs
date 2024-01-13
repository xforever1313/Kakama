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

        [TestMethod]
        public void AddProfilesToDifferentNamespaceTest()
        {
            // Setup
            var ns1 = new Namespace
            {
                Name = "Test Namespace 1"
            };

            this.Uut.NamespaceManager.ConfigureNamespace( ns1 );

            var ns2 = new Namespace
            {
                Name = "Test Namespace 2"
            };

            this.Uut.NamespaceManager.ConfigureNamespace( ns2 );

            const string slug = "xforever1313-slug";

            var profile1 = new Profile
            {
                Name = "xforever1313",
                NamespaceId = ns1.Id,
                Description = "Test",
                Slug = slug
            };

            // Make it the exact same profile,
            // to test that we can have duplicate profiles
            // across multiple namespaces.
            var profile2 = profile1 with
            {
                NamespaceId = ns2.Id
            };

            // Act
            int profile1Id = this.Uut.ProfileManager.ConfigureProfile( profile1 );
            int profile2Id = this.Uut.ProfileManager.ConfigureProfile( profile2 );

            // Check
            Assert.AreEqual( 
                1,
                this.Uut.ProfileManager.GetTotalNumberOfProfilesInNamespace( ns1.Id )
            );

            Assert.AreEqual( 
                1,
                this.Uut.ProfileManager.GetTotalNumberOfProfilesInNamespace( ns2.Id )
            );

            Assert.AreEqual(
                2,
                this.Uut.ProfileManager.GetNumberOfProfiles()
            );

            Assert.AreEqual(
                profile1,
                this.Uut.ProfileManager.GetProfileBySlug( ns1.Id, slug )
            );

            Assert.AreEqual(
                profile2,
                this.Uut.ProfileManager.GetProfileBySlug( ns2.Id, slug )
            );

            Assert.AreEqual(
                profile1,
                this.Uut.ProfileManager.GetProfileById( profile1Id )
            );

            Assert.AreEqual(
                profile2,
                this.Uut.ProfileManager.GetProfileById( profile2Id )
            );
        }

        [TestMethod]
        public void AddProfileMetadataTest()
        {
            // Setup
            Namespace ns = AddTestNamespace();
            var profile = new Profile
            {
                Name = "Metadata Test 1",
                NamespaceId = ns.Id
            };
            int profileId = this.Uut.ProfileManager.ConfigureProfile( profile );

            var profileMetadata1 = new ProfileMetaData
            {
                ProfileId = profileId,
                ExplictOrder = 2,
                Name = "Website",
                Value = "https://shendrick.net"
            };

            var profileMetadata2 = new ProfileMetaData
            {
                ProfileId = profileId,
                ExplictOrder = 1,
                Name = "Pronouns",
                Value = "He/Him"
            };

            // Act
            int metadataId1 = this.Uut.ProfileManager.ConfigureMetadata( profileMetadata1 );
            int metadataId2 = this.Uut.ProfileManager.ConfigureMetadata( profileMetadata2 );

            // Check
            Assert.AreEqual( 1, metadataId1 );
            Assert.AreEqual( 2, metadataId2 );
            Assert.AreEqual( metadataId1, profileMetadata1.Id );
            Assert.AreEqual( metadataId2, profileMetadata2.Id );

            IList<ProfileMetaData> allMetaData = this.Uut.ProfileManager.GetMetaData( profileId );
            Assert.AreEqual( 2, allMetaData.Count );

            // With explicit order, the least explicit order should come first.
            Assert.AreEqual( profileMetadata2, allMetaData[0] );
            Assert.AreEqual( profileMetadata1, allMetaData[1] );
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
