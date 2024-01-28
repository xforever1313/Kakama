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
using KristofferStrube.ActivityStreams;
using Microsoft.AspNetCore.Mvc;

namespace Kakama.Web.Controllers
{
    public class ProfileController : Controller
    {
        // ---------------- Fields ----------------

        private readonly IKakamaApi api;

        // ---------------- Constructor ----------------

        public ProfileController( IKakamaApi api )
        {
            this.api = api;
        }

        // ---------------- Functions ----------------

#if false
        [Route( "/{namespace}/{profile}" )]
        public IActionResult Index( [FromRoute]string @namespace, string profile )
        {
            try
            {

            }
            catch( NotFoundException e )
            {
                return NotFound( e.Message );
            }
        }
#endif

        [Route( "/{namespace}/{profile}/profile.json" )]
        public async Task<IActionResult> ProfileJson( [FromRoute] string @namespace, string profile )
        {
            try
            {
                ProfileModel profileModel = await GetProfileModel( @namespace, profile );

                this.HttpContext.Response.ContentType = "application/activity+json";

                Service service = profileModel.Profile.ToActivityPubJson(
                    profileModel.Namespace,
                    profileModel.RsaKey,
                    profileModel.ProfileMetaData,
                    "https://shendrick.net"
                );

                return Json( service );
            }
            catch( NotFoundException e )
            {
                return NotFound( e.Message );
            }
        }

        private async Task<ProfileModel> GetProfileModel( string namespaceSlug, string profileSlug )
        {
            Namespace ns = await Task.Run(
                () => this.api.NamespaceManager.GetNamespaceBySlug( namespaceSlug )
            );

            Api.Models.Profile profileObj = await Task.Run(
                () => this.api.ProfileManager.GetProfileBySlug( ns.Id, profileSlug )
            );

            IEnumerable<ProfileMetaData> metaData = await Task.Run(
                () => this.api.ProfileManager.GetMetaData( profileObj.Id )
            );

            RsaKey rsaKey = await Task.Run(
                () => this.api.ProfileManager.GetRsaKey( profileObj.Id )
            );

            return new ProfileModel( ns, profileObj, rsaKey, metaData );
        }
    }
}
