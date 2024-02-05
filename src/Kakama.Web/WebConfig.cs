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

using Kakama.Standard.Namespaces;
using SethCS.Exceptions;

namespace Kakama.Web
{
    public record class WebConfig
    {
        // ---------------- Properties ----------------

        /// <summary>
        /// Set this if the service is running not on the root
        /// of the URL.
        /// </summary>
        public string BasePath { get; init; } = "";

        /// <summary>
        /// If <see cref="Api.Models.Namespace.BaseUri"/> is set to null,
        /// this is used instead.
        /// 
        /// This should be set to your front-facing URL
        /// (e.g. https://shendrick.net).
        /// 
        /// This _is_ allowed to be null, but it means that all namespaces must have
        /// their base URL set.
        /// </summary>
        public Uri? DefaultBaseUri { get; init; } = null;

        /// <summary>
        /// If the given request has a port in
        /// the URL, should we process it?
        /// 
        /// If false, then each request will 400.
        /// </summary>
        public bool AllowPorts { get; init; } = true;

        /// <summary>
        /// If the requested URL that contains "//" this will
        /// set it to "/" instead if true.
        /// </summary>
        public bool RewriteDoubleSlashes { get; init; } = false;

        public bool NamespacesMapToUrl { get; init; } = false;

        /// <summary>
        /// The URL that prometheus metrics should be reported to.
        /// Set to null to not report metrics at all.
        /// </summary>
        public string? MetricsUrl { get; init; } = null;

        /// <summary>
        /// Where to log information or greater messages to.
        /// Leave null for no logging to files.
        /// </summary>
        public FileInfo? LogFile { get; init; } = null;

        public string? TelegramBotToken { get; init; } = null;

        public string? TelegramChatId { get; init; } = null;
    }

    internal static class WebConfigExtensions
    {
        // ---------------- Functions ----------------

        public static WebConfig FromEnvVar()
        {
            bool NotNull( string envName, out string envValue )
            {
                envValue = Environment.GetEnvironmentVariable( envName ) ?? "";
                return string.IsNullOrWhiteSpace( envValue ) == false;
            }

            var settings = new WebConfig();

            if( NotNull( "WEB_ALLOW_PORTS", out string allowPorts ) )
            {
                settings = settings with
                {
                    AllowPorts = bool.Parse( allowPorts )
                };
            }

            if( NotNull( "WEB_BASE_PATH", out string basePath ) )
            {
                settings = settings with
                {
                    BasePath = basePath
                };
            }

            if( NotNull( "WEB_BASE_URI", out string baseUrl ) )
            {
                settings = settings with
                {
                    DefaultBaseUri = new Uri( baseUrl )
                };
            }

            if( NotNull( "WEB_METRICS_URL", out string metricsUrl ) )
            {
                settings = settings with
                {
                    MetricsUrl = metricsUrl
                };
            }

            if( NotNull( "WEB_STRIP_DOUBLE_SLASH", out string stripDoubleSlash ) )
            {
                settings = settings with
                {
                    RewriteDoubleSlashes = bool.Parse( stripDoubleSlash )
                };
            }

            if( NotNull( "LOG_FILE", out string logFile ) )
            {
                settings = settings with { LogFile = new FileInfo( logFile ) };
            }

            if( NotNull( "LOG_TELEGRAM_BOT_TOKEN", out string tgBotToken ) )
            {
                settings = settings with { TelegramBotToken = tgBotToken };
            }

            if( NotNull( "LOG_TELEGRAM_CHAT_ID", out string tgChatId ) )
            {
                settings = settings with { TelegramChatId = tgChatId };
            }

            return settings;
        }

        public static void Validate( this WebConfig config )
        {
            var errors = new List<string>();

            if( config.MetricsUrl is not null )
            {
                if( config.MetricsUrl.StartsWith( '/' ) == false )
                {
                    errors.Add( $"{nameof( config.MetricsUrl )} must start with a '/'." );
                }
                else if( config.MetricsUrl.Length <= 1 )
                {
                    errors.Add( $"{nameof( config.MetricsUrl )} must be 2 or greater characters.  Got: {config.MetricsUrl}" );
                }
            }

            if( errors.Any() )
            {
                throw new ListedValidationException(
                    $"Error when validating {nameof( WebConfig )}",
                    errors
                );
            }
        }

        public static Uri GetExpectedBaseUri( this WebConfig webConfig, Namespace targetNamespace )
        {
            // Target namespace's URL has priority, return that
            // if its set.
            if( targetNamespace.BaseUri is not null )
            {
                return targetNamespace.BaseUri;
            }
            // Otherwise, fallback to the website default, assuming its specified.
            else if(
                ( targetNamespace.BaseUri is null ) &&
                ( webConfig.DefaultBaseUri is not null )
            )
            {
                return webConfig.DefaultBaseUri;
            }
            // If both are not specified, that's a configuration problem
            // the site admin needs to handle.
            else
            {
                throw new InvalidOperationException(
                    $"Base URL is not set for namespace ID {targetNamespace.Id}, and there is no default one set.{Environment.NewLine}" +
                    $"Either set the default base url in {nameof( WebConfig )}, or set the base URL on namespace {targetNamespace.Id}."
                );
            }
        }
    }
}
