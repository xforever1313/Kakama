//
// Kakama - An ActivityPub Bot Framework
// Copyright (C) 2023 Seth Hendrick
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

using Serilog;

namespace Kakama.Api
{
    public interface IKakamaApi
    {
        public ILogger Log { get; }
    }

    public class KakamaApi : IDisposable
    {
        // ---------------- Fields ----------------

        private readonly IEnumerable<FileInfo> pluginPaths;

        private List<IKakamaPlugin> plugins;

        private bool inited;
        private bool isDisposed;

        // ---------------- Constructor ----------------

        public KakamaApi( ILogger log, IEnumerable<FileInfo> pluginPaths )
        {
            this.Log = log;
            this.pluginPaths = pluginPaths;
            this.plugins = new List<IKakamaPlugin>();

            this.inited = false;
            this.isDisposed = false;
        }

        // ---------------- Properties ----------------

        public ILogger Log { get; private set; }

        // ---------------- Functions ----------------

        public void Init()
        {
            if( this.inited )
            {
                throw new InvalidOperationException( "API has already been inited!" );
            }

            LoadPlugins();

            this.inited = true;
        }

        private void LoadPlugins()
        {
            // TODO.
            foreach( FileInfo pluginPath in this.pluginPaths )
            {
            }
        }

        public void Dispose()
        {
            if( this.isDisposed )
            {
                throw new ObjectDisposedException( this.GetType().Name );
            }

            foreach( IKakamaPlugin plugin in this.plugins )
            {
                try
                {
                    plugin.Dispose();
                }
                catch( Exception e )
                {
                    this.Log.Error(
                        $"Error when tearing down plugin {plugin.Name}: {e.Message}."
                    );
                    this.Log.Debug( e.ToString() );
                }
            }

            this.isDisposed = true;
        }
    }
}
