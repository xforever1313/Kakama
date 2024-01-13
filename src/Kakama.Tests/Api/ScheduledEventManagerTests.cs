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
using Kakama.Api.EventScheduler;
using Moq;
using Serilog;

namespace Kakama.Tests.Api
{
    [TestClass]
    [DoNotParallelize]
    public sealed class ScheduledEventManagerTests
    {
        // ---------------- Fields ----------------

        private KakamaApiHarness? api;

        private IScheduledEventManager? uut;

        // ---------------- Setup / Teardown ----------------

        [TestInitialize]
        public void TestSetup()
        {
            this.api = new KakamaApiHarness( "scheduleTest" );
            this.api.PerformTestSetup();

            this.uut = this.api.EventManager;
        }

        [TestCleanup]
        public void TestTeardown()
        {
            this.api?.PerformTestTearDown();
        }

        // ---------------- Properties ----------------

        private IScheduledEventManager Uut
        {
            get
            {
                Assert.IsNotNull( this.uut );
                return this.uut;
            }
        }

        // ---------------- Tests ----------------

        [TestMethod]
        public void DoSingleEventOneIterationTest()
        {
            // Setup
            using var countdownEvent = new CountdownEvent( 1 );

            var testEvent1 = new TestEvent( countdownEvent );
            
            // Act
            this.Uut.ConfigureEvent( testEvent1 );

            // Check
            WaitOnCountdownEvent( countdownEvent );
        }

        [TestMethod]
        public void DoSingleEventThreeIterationTest()
        {
            // Setup
            using var countdownEvent = new CountdownEvent( 3 );

            var testEvent1 = new TestEvent( countdownEvent );
            
            // Act
            this.Uut.ConfigureEvent( testEvent1 );

            // Check
            WaitOnCountdownEvent( countdownEvent );
        }

        [TestMethod]
        public void DoTwoEventsThreeIterationTest()
        {
            // Setup
            using var countdownEvent1 = new CountdownEvent( 3 );
            using var countdownEvent2 = new CountdownEvent( 3 );

            var testEvent1 = new TestEvent( countdownEvent1 );
            var testEvent2 = new TestEvent( countdownEvent2 );
            
            // Act
            this.Uut.ConfigureEvent( testEvent1 );
            this.Uut.ConfigureEvent( testEvent2 );

            // Check
            WaitOnCountdownEvent( countdownEvent1 );
            WaitOnCountdownEvent( countdownEvent2 );
        }

        // ---------------- Test Helpers ----------------

        private static void WaitOnCountdownEvent( CountdownEvent e )
        {
            Assert.IsTrue( e.Wait( TimeSpan.FromSeconds( 10 ) ) );
        }

        // ---------------- Helper Classes ----------------

        private sealed class TestEvent : ScheduledEvent
        {
            // ---------------- Fields ----------------

            private readonly CountdownEvent countdownEvent;

            // ---------------- Constructor ----------------

            public TestEvent( CountdownEvent countdownEvent )
            {
                this.countdownEvent = countdownEvent;
            }

            // ---------------- Properties ----------------

            public override string CronString => "* * * * * ?";

            public override string EventName => "Test Event";

            // ---------------- Functions ----------------

            public override async Task ExecuteEvent( ScheduledEventParameters eventParams )
            {
                await Task.Run(
                    () =>
                    {
                        if( this.countdownEvent.CurrentCount > 0 )
                        {
                            this.countdownEvent.Signal();
                        }
                    }
                );
            }
        }
    }
}
