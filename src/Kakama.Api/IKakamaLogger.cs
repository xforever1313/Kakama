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

namespace Kakama.Api
{
    /// <summary>
    /// Wrapper to an actual ILogger instance.
    /// This exists so plugins don't need to take
    /// an additional logging dependency, and so we can change
    /// the back-end logging system without breaking being able to compile.
    /// </summary>
    public interface IKakamaLogger
    {
        void Debug( string message );

        void Verbose( string message );

        void Information( string message );

        void Warning( string message );

        void Error( string message );

        void Fatal( string message );
    }
}
