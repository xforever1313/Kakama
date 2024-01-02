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

using Kakama.Api.DatabaseEngines;
using Serilog;

namespace Kakama.Api
{
    public interface IKakamaApi
    {
        // ---------------- Properties ----------------

        ILogger Log { get; }

        NamespaceManager NamespaceManager { get; }

        ProfileManager ProfileManager { get; }

        string Version { get; }

        // ---------------- Functions ----------------

        TDbConnection CreateDatabaseConnection<TDbConnection>()
            where TDbConnection : BaseDatabaseConnection, new();
    }

    public class KakamaApi : IKakamaApi, IDisposable
    {
        // ---------------- Fields ----------------

        protected readonly KakamaSettings settings;

        private readonly IEnumerable<FileInfo> pluginPaths;

        private readonly IDatabaseEngine dbEngine;

        private readonly List<IKakamaPlugin> plugins;

        private bool inited;
        private bool isDisposed;

        // ---------------- Constructor ----------------

        public KakamaApi( KakamaSettings settings, ILogger log ) :
            this( settings, log, Array.Empty<FileInfo>() )
        {

        }

        public KakamaApi( KakamaSettings settings, ILogger log, IEnumerable<FileInfo> pluginPaths )
        {
            this.settings = settings;
            this.pluginPaths = pluginPaths;
            this.plugins = new List<IKakamaPlugin>();

            if( settings.DatabaseEngine == DatabaseEngine.Sqlite )
            {
                this.dbEngine = new SqliteDatabaseEngine(
                    this.settings.SqliteDatabaseLocation,
                    this.settings.SqlitePoolConnection
                );
            }
            else
            {
                throw new NotSupportedException(
                    $"The given database engine is not supported: {settings.DatabaseEngine}"
                );
            }

            this.Log = log;
            this.NamespaceManager = new NamespaceManager( this );
            this.ProfileManager = new ProfileManager( this );
            this.Version = GetType().Assembly.GetName().Version?.ToString( 3 ) ?? "Unknown Version";

            this.inited = false;
            this.isDisposed = false;
        }

        // ---------------- Properties ----------------

        public ILogger Log { get; }

        public NamespaceManager NamespaceManager { get; }

        public ProfileManager ProfileManager { get; }
        public string Version { get; }

        // ---------------- Functions ----------------

        public void Init()
        {
            if( this.inited )
            {
                throw new InvalidOperationException( "API has already been inited!" );
            }

            using( KakamaDatabaseConnection db = this.CreateKakamaDatabaseConnection() )
            {
                db.EnsureCreated();
            }

            LoadPlugins();

            this.inited = true;
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

        public TDbConnection CreateDatabaseConnection<TDbConnection>()
            where TDbConnection : BaseDatabaseConnection, new()
        {
            TDbConnection connection = new TDbConnection();
            connection.Init( this.dbEngine, this.Log );

            return connection;
        }

        private void LoadPlugins()
        {
            // TODO.
            foreach( FileInfo pluginPath in this.pluginPaths )
            {
            }
        }
    }

    internal static class IKakamaApiExtensions
    {
        internal static KakamaDatabaseConnection CreateKakamaDatabaseConnection( this IKakamaApi api )
        {
            return api.CreateDatabaseConnection<KakamaDatabaseConnection>();
        }
    }
}
