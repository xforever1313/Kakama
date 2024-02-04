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

using Kakama.Api.Logging;
using Serilog.Events;
using Serilog.Parsing;

namespace Kakama.Tests.Api.Logging
{
    [TestClass]
    public sealed class LogMessageCounterTests
    {
        // ---------------- Fields ----------------

        private ulong warningsCounted;

        private ulong errorsCounted;

        private ulong fatalsCounted;

        private LogMessageCounter? uut;

        // ---------------- Setup / Teardown ----------------

        [TestInitialize]
        public void TestSetup()
        {
            this.warningsCounted = 0;
            this.errorsCounted = 0;
            this.fatalsCounted = 0;

            this.uut = new LogMessageCounter();
            this.uut.WarningCountUpdated += Uut_WarningCountUpdated;
            this.uut.ErrorCountUpdated += Uut_ErrorCountUpdated;
            this.uut.FatalCountUpdated += Uut_FatalCountUpdated;
        }

        [TestCleanup]
        public void TestTeardown()
        {
            if( this.uut is not null )
            {
                this.uut.WarningCountUpdated -= Uut_WarningCountUpdated;
                this.uut.ErrorCountUpdated -= Uut_ErrorCountUpdated;
                this.uut.FatalCountUpdated -= Uut_FatalCountUpdated;
            }
        }

        // ---------------- Properties ----------------

        public LogMessageCounter Uut
        {
            get
            {
                Assert.IsNotNull( this.uut );
                return this.uut;
            }
        }

        // ---------------- Tests ----------------

        [TestMethod]
        public void ContructorStartsFromZeroTest()
        {
            // Check
            Assert.AreEqual( 0UL, this.Uut.WarningsSeen );
            Assert.AreEqual( 0UL, this.Uut.ErrorsSeen );
            Assert.AreEqual( 0UL, this.Uut.FatalsSeen );
        }

        [TestMethod]
        public void VerboseLogTest()
        {
            DoUnsupportedLevelTest( LogEventLevel.Verbose );
        }

        [TestMethod]
        public void DebugLogTest()
        {
            DoUnsupportedLevelTest( LogEventLevel.Debug );
        }

        [TestMethod]
        public void InformationLogTest()
        {
            DoUnsupportedLevelTest( LogEventLevel.Information );
        }

        [TestMethod]
        public void WarningOnceCountTest()
        {
            // Setup
            var logEvent = CreateLogEvent( LogEventLevel.Warning );

            // Act
            this.Uut.Emit( logEvent );

            // Check
            Assert.AreEqual( 1UL, this.Uut.WarningsSeen );
            Assert.AreEqual( 1UL, this.warningsCounted );

            Assert.AreEqual( 0UL, this.Uut.ErrorsSeen );
            Assert.AreEqual( 0UL, this.errorsCounted );

            Assert.AreEqual( 0UL, this.Uut.FatalsSeen );
            Assert.AreEqual( 0UL, this.fatalsCounted );
        }

        [TestMethod]
        public void WarningTwiceCountTest()
        {
            // Setup
            var logEvent = CreateLogEvent( LogEventLevel.Warning );

            // Act
            this.Uut.Emit( logEvent );
            this.Uut.Emit( logEvent );

            // Check
            Assert.AreEqual( 2UL, this.Uut.WarningsSeen );
            Assert.AreEqual( 2UL, this.warningsCounted );

            Assert.AreEqual( 0UL, this.Uut.ErrorsSeen );
            Assert.AreEqual( 0UL, this.errorsCounted );

            Assert.AreEqual( 0UL, this.Uut.FatalsSeen );
            Assert.AreEqual( 0UL, this.fatalsCounted );
        }

        [TestMethod]
        public void ErrorOnceCountTest()
        {
            // Setup
            var logEvent = CreateLogEvent( LogEventLevel.Error );

            // Act
            this.Uut.Emit( logEvent );

            // Check
            Assert.AreEqual( 0UL, this.Uut.WarningsSeen );
            Assert.AreEqual( 0UL, this.warningsCounted );

            Assert.AreEqual( 1UL, this.Uut.ErrorsSeen );
            Assert.AreEqual( 1UL, this.errorsCounted );

            Assert.AreEqual( 0UL, this.Uut.FatalsSeen );
            Assert.AreEqual( 0UL, this.fatalsCounted );
        }

        [TestMethod]
        public void ErrorTwiceCountTest()
        {
            // Setup
            var logEvent = CreateLogEvent( LogEventLevel.Error );

            // Act
            this.Uut.Emit( logEvent );
            this.Uut.Emit( logEvent );

            // Check
            Assert.AreEqual( 0UL, this.Uut.WarningsSeen );
            Assert.AreEqual( 0UL, this.warningsCounted );

            Assert.AreEqual( 2UL, this.Uut.ErrorsSeen );
            Assert.AreEqual( 2UL, this.errorsCounted );

            Assert.AreEqual( 0UL, this.Uut.FatalsSeen );
            Assert.AreEqual( 0UL, this.fatalsCounted );
        }

        [TestMethod]
        public void FatalOnceCountTest()
        {
            // Setup
            var logEvent = CreateLogEvent( LogEventLevel.Fatal );

            // Act
            this.Uut.Emit( logEvent );

            // Check
            Assert.AreEqual( 0UL, this.Uut.WarningsSeen );
            Assert.AreEqual( 0UL, this.warningsCounted );

            Assert.AreEqual( 0UL, this.Uut.ErrorsSeen );
            Assert.AreEqual( 0UL, this.errorsCounted );

            Assert.AreEqual( 1UL, this.Uut.FatalsSeen );
            Assert.AreEqual( 1UL, this.fatalsCounted );
        }

        [TestMethod]
        public void FatalTwiceCountTest()
        {
            // Setup
            var logEvent = CreateLogEvent( LogEventLevel.Fatal );

            // Act
            this.Uut.Emit( logEvent );
            this.Uut.Emit( logEvent );

            // Check
            Assert.AreEqual( 0UL, this.Uut.WarningsSeen );
            Assert.AreEqual( 0UL, this.warningsCounted );

            Assert.AreEqual( 0UL, this.Uut.ErrorsSeen );
            Assert.AreEqual( 0UL, this.errorsCounted );

            Assert.AreEqual( 2UL, this.Uut.FatalsSeen );
            Assert.AreEqual( 2UL, this.fatalsCounted );
        }

        // ---------------- Test Helpers ----------------

        private static LogEvent CreateLogEvent( LogEventLevel level )
        {
            return new LogEvent(
                DateTimeOffset.Now,
                level,
                null,
                new MessageTemplate( Array.Empty<MessageTemplateToken>() ),
                Array.Empty<LogEventProperty>()
            );
        }

        /// <summary>
        /// Ensures that if we do a log level we don't support,
        /// no counts get incremented.
        /// </summary>
        private void DoUnsupportedLevelTest( LogEventLevel level )
        {
            // Setup
            var logEvent = CreateLogEvent( level );

            // Act
            this.Uut.Emit( logEvent );

            // Check
            Assert.AreEqual( 0UL, this.Uut.WarningsSeen );
            Assert.AreEqual( 0UL, this.warningsCounted );

            Assert.AreEqual( 0UL, this.Uut.ErrorsSeen );
            Assert.AreEqual( 0UL, this.errorsCounted );

            Assert.AreEqual( 0UL, this.Uut.FatalsSeen );
            Assert.AreEqual( 0UL, this.fatalsCounted );
        }

        private void Uut_WarningCountUpdated( ulong newCount )
        {
            this.warningsCounted = newCount;
        }

        private void Uut_ErrorCountUpdated( ulong newCount )
        {
            this.errorsCounted = newCount;
        }

        private void Uut_FatalCountUpdated( ulong newCount )
        {
            this.fatalsCounted = newCount;
        }
    }
}
