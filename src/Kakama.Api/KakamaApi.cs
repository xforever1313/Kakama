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

using Kakama.Api.DatabaseEngines;
using Kakama.Api.EventScheduler;
using Kakama.Api.Logging;
using Kakama.Api.Namespaces;
using Kakama.Standard;
using Kakama.Standard.Logging;
using Kakama.Standard.Namespaces;
using Serilog;

namespace Kakama.Api
{
    public interface IKakamaApi
    {
        // ---------------- Properties ----------------

        IKakamaLogger Log { get; }

        INamespaceManager NamespaceManager { get; }

        ProfileManager ProfileManager { get; }

        IScheduledEventManager EventManager { get; }

        IDateTimeFactory DateTimeFactory { get; }

        string Version { get; }

        // ---------------- Functions ----------------

        TDbConnection CreateDatabaseConnection<TDbConnection>()
            where TDbConnection : BaseDatabaseConnection, new();
    }

    public class KakamaApi : IKakamaApi, IDisposable
    {
        // ---------------- Fields ----------------

        protected readonly KakamaSettings settings;

        protected readonly bool runScheduledEvents;

        private readonly KakamaLogger log;

        private readonly IDisposeableScheduledEventManager eventManager;

        private readonly IEnumerable<FileInfo> pluginPaths;

        private readonly IDatabaseEngine dbEngine;

        private readonly List<IKakamaPlugin> plugins;

        private bool inited;
        private bool isDisposed;

        // ---------------- Constructor ----------------

        public KakamaApi( KakamaSettings settings, ILogger log, bool runScheduledEvents ) :
            this( settings, log, runScheduledEvents, Array.Empty<FileInfo>() )
        {

        }

        public KakamaApi(
            KakamaSettings settings,
            ILogger log,
            bool runScheduledEvents,
            IEnumerable<FileInfo> pluginPaths
        ) : this( settings, log, runScheduledEvents, pluginPaths, new DateTimeFactory() )
        {
        }

        public KakamaApi(
            KakamaSettings settings,
            ILogger log,
            bool runScheduledEvents,
            IEnumerable<FileInfo> pluginPaths,
            IDateTimeFactory dateTimeFactory
        )
        {
            this.settings = settings;
            this.runScheduledEvents = runScheduledEvents;
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

            this.log = new KakamaLogger( log );
            this.NamespaceManager = new NamespaceManager( this );
            this.ProfileManager = new ProfileManager( this );

            if( this.runScheduledEvents )
            {
                this.eventManager = new ScheduledEventManager( this, this.log.RealLog );
            }
            else
            {
                this.eventManager = new DisabledScheduledEventManager();
            }

            this.DateTimeFactory = dateTimeFactory;
            this.Version = GetType().Assembly.GetName().Version?.ToString( 3 ) ?? "Unknown Version";

            this.inited = false;
            this.isDisposed = false;
        }

        // ---------------- Properties ----------------

        public IKakamaLogger Log => this.log;

        public INamespaceManager NamespaceManager { get; }

        public ProfileManager ProfileManager { get; }

        public IScheduledEventManager EventManager => this.eventManager;

        public IDateTimeFactory DateTimeFactory { get; }

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

            if( this.eventManager.Enabled )
            {
                this.eventManager.Start();
            }
            else
            {
                this.Log.Debug( "Scheduled events flagged to not run, not starting event scheduler." );
            }

            this.inited = true;
        }

        public void Dispose()
        {
            if( this.isDisposed )
            {
                throw new ObjectDisposedException( this.GetType().Name );
            }

            this.eventManager.Dispose();

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
