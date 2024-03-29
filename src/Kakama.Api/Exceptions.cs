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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kakama.Api
{
    public class NotFoundException : Exception
    {
        public NotFoundException( string message ) :
            base( message )
        {
        }
    }

    public class NamespaceNotFoundException : NotFoundException
    {
        public NamespaceNotFoundException( string message ) :
            base( message )
        {
        }
    }

    public class ProfileNotFoundException : NotFoundException
    {
        public ProfileNotFoundException( string message ) :
            base( message )
        {
        }
    }

    public class RsaKeyNotFoundException : NotFoundException
    {
        public RsaKeyNotFoundException( string message ) :
            base( message )
        {
        }
    }
}
