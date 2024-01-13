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

using Quartz;
using Quartz.Logging;
using Serilog.Extensions.Logging;

namespace Kakama.Api.EventScheduler
{
    /// <summary>
    /// Manages timed events.
    /// </summary>
    public class ScheduledEventManager : IDisposable
    {
        // ---------------- Fields ----------------

        private const string apiJobKey = "api";

        private const string eventInfoKey = "eventInfo";

        private readonly IKakamaApi api;

        private readonly Dictionary<int, ITrigger> events;

        private readonly IScheduler taskScheduler;

        private readonly IJobDetail job;

        // ---------------- Constructor ----------------

        public ScheduledEventManager( IKakamaApi api )
        {
            this.api = api;
            this.events = new Dictionary<int, ITrigger>();

            var msLogger = new SerilogLoggerFactory( this.api.Log );
            LogContext.SetCurrentLogProvider( msLogger );

            this.taskScheduler = SchedulerBuilder.Create()
                .WithName( $"{nameof( KakamaApi )} scheduler" )
                .WithInterruptJobsOnShutdown( true )
                .UseDedicatedThreadPool()
                .BuildScheduler()
                .Result;

            var jobData = new JobDataMap
            {
                [apiJobKey] = this.api
            };

            this.job = JobBuilder.Create<KakamaJob>()
                .WithIdentity( "Kakama Event" )
                .StoreDurably()
                .SetJobData( jobData )
                .Build();

            this.taskScheduler.AddJob( this.job, false );
        }

        // ---------------- Properties ----------------

        // ---------------- Functions ----------------

        public void Start()
        {
            this.taskScheduler.Start();
        }

        public void Dispose()
        {
            this.taskScheduler.DeleteJob( this.job.Key );
            this.taskScheduler.Shutdown();
        }

        public int ConfigureEvent( ScheduledEvent e )
        {
            ITrigger CreateTrigger( string eventName )
            {
                var jobData = new JobDataMap
                {
                    [eventInfoKey] = e
                };

                ITrigger trigger = TriggerBuilder.Create()
                    .WithCronSchedule(
                        e.CronString,
                        ( CronScheduleBuilder cronBuilder ) =>
                        {
                            cronBuilder.InTimeZone( e.TimeZone );
                            cronBuilder.WithMisfireHandlingInstructionFireAndProceed();
                        }
                    )
                    .WithIdentity( eventName )
                    .UsingJobData( jobData )
                    .ForJob( this.job.Key )
                    .StartNow()
                    .Build();

                return trigger;
            }

            if( e.Id == 0 )
            {
                e.Id = this.events.Count + 1;

                string eventName = e.GetEventName();

                ITrigger trigger = CreateTrigger( eventName );
                this.events[e.Id] = trigger;
                this.taskScheduler.ScheduleJob( trigger );
            }
            else if( this.events.ContainsKey( e.Id ) == false )
            {
                throw new ArgumentException(
                    $"Can not find event with ID: {e.Id}",
                    nameof( e )
                );
            }
            else
            {
                string eventName = e.GetEventName();

                ITrigger trigger = this.events[e.Id];

                TriggerKey key = trigger.Key;
                trigger = CreateTrigger( eventName );
                this.taskScheduler.RescheduleJob( key, trigger );
            }

            return e.Id;
        }

        public void RemoveEvent( int eventId )
        {
            if( this.events.ContainsKey( eventId ) == false )
            {
                // If an event doesn't exist, just let it go.
                return;
            }

            ITrigger trigger = this.events[eventId];
            this.taskScheduler.UnscheduleJob( trigger.Key );
            this.events.Remove( eventId );
        }

        private class KakamaJob : IJob
        {
            public Task Execute( IJobExecutionContext context )
            {
                object apiObj = context.MergedJobDataMap.Get( apiJobKey );
                if( apiObj is not IKakamaApi api )
                {
                    throw new InvalidCastException(
                        $"Could not cast job data type to {nameof( IKakamaApi )}."
                    );
                }

                object eventInfoObj = context.MergedJobDataMap.Get( eventInfoKey );
                if( eventInfoObj is not ScheduledEvent e )
                {
                    throw new InvalidCastException(
                        $"Could not cast job data type to {nameof( ScheduledEvent )}."
                    );
                }

                var eventParams = new ScheduledEventParameters( api, context );
                return e.ExecuteEvent( eventParams );
            }
        }
    }
}
