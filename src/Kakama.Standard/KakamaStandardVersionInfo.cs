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

namespace Kakama.Standard
{
    /// <summary>
    /// Provides the version information of the Kakama Standard.
    /// </summary>
    public static class KakamaStandardVersionInfo
    {
        // ---------------- Constructor ----------------

        static KakamaStandardVersionInfo()
        {
            Version = typeof( KakamaStandardVersionInfo ).Assembly.GetName()?.Version?.ToString( 3 ) ?? "Unknown Version";
        }

        public static string Version { get; }
    }
}
