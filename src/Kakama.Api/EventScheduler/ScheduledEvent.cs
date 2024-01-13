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

namespace Kakama.Api.EventScheduler
{
    public abstract class ScheduledEvent
    {
        // ---------------- Properties ----------------

        /// <summary>
        /// The ID of the event.
        /// Set to 0 means to add the event.
        /// This will then get automatically set when
        /// the event is successfully added.
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Optional event name.  If left blank,
        /// the name will be come "Event <see cref="Id"/>".
        /// 
        /// If specified, the name will become:
        /// "Event <see cref="Id"/>: <see cref="EventName"/>"
        /// </summary>
        public virtual string EventName { get; } = "";

        /// <summary>
        /// Gets the timezone where this event is executed in.
        /// </summary>
        public virtual TimeZoneInfo TimeZone { get; } = TimeZoneInfo.Local;

        /// <summary>
        /// Cron string - when to run this event.
        /// See Quart's documentation on how to
        /// set this:
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/how-tos/crontrigger.html
        /// </summary>
        public abstract string CronString { get; }

        // ---------------- Functions ----------------

        /// <summary>
        /// What gets executed each time this event gets fired.
        /// </summary>
        public abstract Task ExecuteEvent( ScheduledEventParameters eventParams );

        internal string GetEventName()
        {
            if( string.IsNullOrEmpty( this.EventName ) )
            {
                return $"Event {this.Id}";
            }
            else
            {
                return $"Event {this.Id}: {this.EventName}";
            }
        }
    }
}
