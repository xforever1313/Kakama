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

using Kakama.Api.Models;
using Kakama.Standard.Namespaces;

namespace Kakama.Web.Models
{
    public class ProfileModel
    {
        // ---------------- Constructor ----------------

        public ProfileModel(
            Namespace @namespace,
            Uri baseUrl,
            Profile profile,
            RsaKey rsaKey,
            IEnumerable<ProfileMetaData> profileMetaData
        )
        {
            this.Namespace = @namespace;
            this.BaseUrl = baseUrl;
            this.Profile = profile;
            this.RsaKey = rsaKey;
            this.ProfileMetaData = profileMetaData;
        }

        // ---------------- Properties ----------------

        public Namespace Namespace { get; }

        public Uri BaseUrl { get; }

        public Profile Profile { get; }

        public RsaKey RsaKey { get; }

        public IEnumerable<ProfileMetaData> ProfileMetaData { get; }
    }
}
