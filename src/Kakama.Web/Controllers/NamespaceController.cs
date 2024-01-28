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

using Kakama.Api;
using Kakama.Api.Models;
using Kakama.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kakama.Web.Controllers
{
    public class NamespaceController : Controller
    {
        // ---------------- Fields ----------------

        private readonly IKakamaApi api;
        private readonly WebConfig webConfig;

        // ---------------- Constructor ----------------

        public NamespaceController( IKakamaApi api, WebConfig webConfig )
        {
            this.api = api;
            this.webConfig = webConfig;
        }

        // ---------------- Functions ----------------

        [Route( "/{namespace}" )]
        public async Task<IActionResult> Index( [FromRoute]string @namespace )
        {
            try
            {
                // If we get nothing, then we probably want
                // a list of all the namespaces, redirect to that.
                if( string.IsNullOrWhiteSpace( @namespace ) )
                {
                    Redirect( "/namespaces.html" );
                }

                Namespace? ns = await Task.Run(
                    () => this.api.NamespaceManager.TryGetNamespaceBySlug( @namespace )
                );

                if ( ns is null )
                {
                    return NotFound( "Could not find specified namespace." );
                }

                Uri targetUri = this.webConfig.GetExpectedBaseUri( ns );
                if( this.IsRequestUriCompatible( targetUri ) == false )
                {
                    // We'll treat this as a 404, since the namespace _technically_
                    // doesn't exist on this URL.
                    return NotFound( "Could not find specified namespace." );
                }

                List<Profile> profiles = await Task.Run(
                    () => this.api.ProfileManager.GetAllProfilesWithinNamespace( ns.Id )
                );

                return View( new ProfileListModel( ns, profiles ) );
            }
            catch( NotFoundException e )
            {
                return NotFound( e.Message );
            }
        }
    }
}
