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

using Quartz;

namespace Kakama.Api.EventScheduler
{
    public class ScheduledEventParameters
    {
        // ---------------- Constructor ---------------

        internal ScheduledEventParameters( IKakamaApi api, IJobExecutionContext context )
        {
            this.Api = api;
            this.FireTimeUtc = context.FireTimeUtc;
            this.CancellationToken = context.CancellationToken;
        }

        // ---------------- Properties ---------------

        public IKakamaApi Api { get; }

        /// <summary>
        /// The time the event actually fired.
        /// <seealso cref="IJobExecutionContext.FireTimeUtc"/>
        /// </summary>
        public DateTimeOffset FireTimeUtc { get; }

        /// <summary>
        /// Returns the cancellation token which will be cancelled when 
        /// the job cancellation has been requested.
        /// 
        /// <seealso cref="IJobExecutionContext.CancellationToken"/>
        /// </summary>
        public CancellationToken CancellationToken { get; }
    }
}
