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
    /// <summary>
    /// Class that is used if we don't want scheduled events to fire
    /// (such as running the command line).
    /// </summary>
    internal sealed class DisabledScheduledEventManager : IDisposeableScheduledEventManager
    {
        // ---------------- Constructor ----------------

        public DisabledScheduledEventManager()
        {
        }

        // ---------------- Properties ----------------

        public bool Enabled => false;

        // ---------------- Functions ----------------

        public void Start() => throw GetException();

        public void Dispose()
        {
        }

        public int ConfigureEvent( ScheduledEvent e ) => throw GetException();

        /// <remarks>
        /// Method does nothing.  The interface says do a no-op if
        /// the event ID doesn't exist, and no events exis
        /// </remarks>
        public void RemoveEvent( int eventId )
        {
        }

        private static InvalidOperationException GetException() =>
            new InvalidOperationException( "Scheduled event manager is disabled, can not configure any events.  This can happen if running in the command line." );
    }
}
