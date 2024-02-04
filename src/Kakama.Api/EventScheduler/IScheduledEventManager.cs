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
    public interface IScheduledEventManager
    {
        // ---------------- Properties ----------------

        /// <summary>
        /// Is the event manager actually enabled or not?
        /// If this returns false, all methods on this interface
        /// should be expected to throw an exception.
        /// </summary>
        bool Enabled { get; }

        // ---------------- Functions ----------------

        /// <summary>
        /// Configures the given event.
        /// If the event's <see cref="ScheduledEvent.Id"/> is zero,
        /// then the event will be added, otherwise it will be modified.
        /// 
        /// When invoking this, the passed in <see cref="ScheduledEvent.Id"/>
        /// will be modified.
        /// </summary>
        /// <returns>The ID of the event configured.</returns>
        int ConfigureEvent( ScheduledEvent e );

        /// <summary>
        /// Removes the given event, and stops
        /// any further events from happening it.  If an event is already
        /// scheduled to fire, and this is called,
        /// the event may not be cancelled.
        /// 
        /// No-op if no such event Id exists.
        /// </summary>
        void RemoveEvent( int eventId );
    }

    internal interface IDisposeableScheduledEventManager : IScheduledEventManager, IDisposable
    {
        void Start();
    }
}
