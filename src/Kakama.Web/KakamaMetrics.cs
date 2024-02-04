﻿//
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
using Prometheus;

namespace Kakama.Web
{
    public class KakamaMetrics : IDisposable
    {
        // ---------------- Fields ----------------

        private readonly LogMessageCounter logCounter;

        private readonly Counter warningCounter;

        private readonly Counter errorCounter;

        private readonly Counter fatalCounter;

        // ---------------- Constructor ----------------

        public KakamaMetrics( LogMessageCounter logCounter )
        {
            this.logCounter = logCounter;
            this.logCounter.WarningCountUpdated += LogCounter_WarningCountUpdated;
            this.logCounter.ErrorCountUpdated += LogCounter_ErrorCountUpdated;
            this.logCounter.FatalCountUpdated += LogCounter_FatalCountUpdated;

            this.warningCounter = Metrics.CreateCounter(
                "kakama_warnings_Logged",
                "The number of warning messsages logged since the process started"
            );

            this.errorCounter = Metrics.CreateCounter(
                "kakama_errors_Logged",
                "The number of error messsages logged since the process started"
            );

            this.fatalCounter = Metrics.CreateCounter(
                "kakama_fatals_Logged",
                "The number of fatal messsages logged since the process started"
            );
        }

        // ---------------- Functions ----------------

        public void Dispose()
        {
            this.logCounter.WarningCountUpdated -= LogCounter_WarningCountUpdated;
            this.logCounter.ErrorCountUpdated -= LogCounter_ErrorCountUpdated;
            this.logCounter.FatalCountUpdated -= LogCounter_FatalCountUpdated;
        }

        private void LogCounter_WarningCountUpdated( ulong newCount )
        {
            this.warningCounter.IncTo( newCount );
        }

        private void LogCounter_ErrorCountUpdated( ulong newCount )
        {
            this.errorCounter.IncTo( newCount );
        }

        private void LogCounter_FatalCountUpdated( ulong newCount )
        {
            this.fatalCounter.IncTo( newCount );
        }
    }
}
