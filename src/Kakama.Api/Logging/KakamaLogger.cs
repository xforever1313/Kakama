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

using Kakama.Standard.Logging;
using Serilog;

namespace Kakama.Api.Logging
{
    /// <summary>
    /// Wrapper to an actual ILogger instance.
    /// This exists so plugins don't need to take
    /// an additional logging dependency, and so we can change
    /// the back-end logging system without breaking being able to compile.
    /// </summary>
    public class KakamaLogger : IKakamaLogger
    {
        // ---------------- Constructor ----------------

        public KakamaLogger( ILogger log )
        {
            this.RealLog = log;
        }

        // ---------------- Properties ----------------

        /// <summary>
        /// Reference to the backing-log object,
        /// just in case the API needs it.  This is really only
        /// meant for classes within this assembly,
        /// and not for consumers who want to write plugins.
        /// </summary>
        internal ILogger RealLog { get; }

        // ---------------- Functions ----------------

        public void Debug( string message )
        {
            this.RealLog.Debug( message );
        }

        public void Verbose( string message )
        {
            this.RealLog.Verbose( message );
        }

        public void Information( string message )
        {
            this.RealLog.Information( message );
        }

        public void Warning( string message )
        {
            this.RealLog.Warning( message );
        }

        public void Error( string message )
        {
            this.RealLog.Error( message );
        }

        public void Fatal( string message )
        {
            this.RealLog.Fatal( message );
        }
    }
}
