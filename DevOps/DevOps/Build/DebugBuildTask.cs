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


using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Frosting;

namespace DevOps.Build
{
    [TaskName( "build" )]
    public sealed class DebugBuildTask : DevopsTask
    {
        // ---------------- Functions ----------------

        public override void Run( BuildContext context )
        {
            var buildOptions = new DotNetBuildSettings
            {
            };

            context.DotNetBuild( context.Solution.ToString(), buildOptions );
        }
    }
}
