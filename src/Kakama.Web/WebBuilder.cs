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

using dotenv.net;
using Kakama.Api;
using Microsoft.AspNetCore.HttpOverrides;
using Mono.Options;
using Serilog;
using Serilog.Sinks.Telegram;
using Serilog.Sinks.Telegram.Alternative;

namespace Kakama.Web
{
    public sealed class WebBuilder
    {
        // ---------------- Fields ----------------

        private readonly string[] args;

        /// <remarks>
        /// This is null until <see cref="Run"/> is called.
        /// </remarks>
        private Serilog.ILogger? log;

        // ---------------- Constructor ----------------

        public WebBuilder( string[] args )
        {
            this.args = args;
        }

        // ---------------- Functions ----------------

        public int Run()
        {
            bool showHelp = false;
            bool showVersion = false;
            bool showLicense = false;
            bool showCredits = false;
            string envFile = string.Empty;

            var options = new OptionSet
            {
                {
                    "h|help",
                    "Shows thie mesage and exits.",
                    v => showHelp = ( v is not null )
                },
                {
                    "version",
                    "Shows the version and exits.",
                    v => showVersion = ( v is not null )
                },
                {
                    "print_license",
                    "Prints the software license and exits.",
                    v => showLicense = ( v is not null )
                },
                {
                    "print_credits",
                    "Prints the third-party notices and credits.",
                    v => showCredits = ( v is not null )
                },
                {
                    "env=",
                    "The .env file that contains the environment variable settings.",
                    v => envFile = v
                }
            };

            try
            {
                options.Parse( args );

                if( showHelp )
                {
                    options.WriteOptionDescriptions( Console.Out );
                    return 0;
                }
                else if( showVersion )
                {
                    PrintVersion();
                    return 0;
                }
                else if( showLicense )
                {
                    PrintLicense();
                    return 0;
                }
                else if( showCredits )
                {
                    PrintCredits();
                    return 0;
                }

                options.Parse( args );

                if( string.IsNullOrWhiteSpace( envFile ) == false )
                {
                    Console.WriteLine( $"Using .env file located at '{envFile}'" );
                    DotEnv.Load( new DotEnvOptions( envFilePaths: new string[] { envFile } ) );
                }

                RunInternal();

                this.log?.Information( "Application Exiting" );
                return 0;
            }
            catch( Exception e )
            {
                this.log?.Fatal( "FATAL ERROR:" + Environment.NewLine + e );
                return -1;
            }
        }

        private void RunInternal()
        {
            WebConfig webConfig = WebConfigExtensions.FromEnvVar();
            this.log = CreateLog( webConfig );

            KakamaSettings settings = KakamaSettingsExtensions.FromEnvVar();

            using var api = new KakamaApi( settings, this.log, Array.Empty<FileInfo>() );

            WebApplicationBuilder builder = WebApplication.CreateBuilder( args );
            builder.Services.AddSingleton<IKakamaApi>( api );

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Host.UseSerilog( this.log );

            WebApplication app = builder.Build();
            if( string.IsNullOrWhiteSpace( webConfig.BasePath ) == false )
            {
                app.Use(
                    ( HttpContext context, RequestDelegate next ) =>
                    {
                        context.Request.PathBase = webConfig.BasePath;
                        return next( context );
                    }
                );
            }

            if( webConfig.RewriteDoubleSlashes )
            {
                app.Use( ( context, next ) =>
                {
                    string? value = context.Request.Path.Value;
                    if( ( value is not null ) && value.StartsWith( "//" ) )
                    {
                        context.Request.Path = new PathString( value.Replace( "//", "/" ) );
                    }
                    return next();
                } );
            }

            app.UseForwardedHeaders(
                new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                }
            );

            if( webConfig.AllowPorts == false )
            {
                app.Use(
                    ( HttpContext context, RequestDelegate next ) =>
                    {
                        int? port = context.Request.Host.Port;
                        if( port is not null )
                        {
                            // Kill the connection,
                            // and stop all processing.
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            context.Connection.RequestClose();
                            return Task.CompletedTask;
                        }

                        return next( context );
                    }
                );
            }

            app.UseHostFiltering();

            // Configure the HTTP request pipeline.
            if( !app.Environment.IsDevelopment() )
            {
                app.UseExceptionHandler( "/Home/Error" );
            }

            app.UseRouting();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }

        private Serilog.ILogger CreateLog( WebConfig webConfig )
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console( Serilog.Events.LogEventLevel.Information );

            bool useFileLogger = false;
            bool useTelegramLogger = false;

            FileInfo? logFile = webConfig.LogFile;
            if( logFile is not null )
            {
                useFileLogger = true;
                logger.WriteTo.File(
                    logFile.FullName,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                    retainedFileCountLimit: 10,
                    fileSizeLimitBytes: 512 * 1000 * 1000, // 512 MB
                    shared: false
                );
            }

            string? telegramBotToken = webConfig.TelegramBotToken;
            string? telegramChatId = webConfig.TelegramChatId;
            if(
                ( string.IsNullOrWhiteSpace( telegramBotToken ) == false ) &&
                ( string.IsNullOrWhiteSpace( telegramChatId ) == false )
            )
            {
                useTelegramLogger = true;
                var telegramOptions = new TelegramSinkOptions(
                    botToken: telegramBotToken,
                    chatId: telegramChatId,
                    dateFormat: "dd.MM.yyyy HH:mm:sszzz",
                    applicationName: $"{nameof(Kakama )}",
                    failureCallback: this.OnTelegramFailure
                );
                logger.WriteTo.Telegram(
                    telegramOptions,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning
                );
            }

            Serilog.ILogger log = logger.CreateLogger();
            log.Information( $"Using File Logging: {useFileLogger}." );
            log.Information( $"Using Telegram Logging: {useTelegramLogger}." );

            return log;
        }

        private void OnTelegramFailure( Exception e )
        {
            this.log?.Warning( $"Telegram message did not send:{Environment.NewLine}{e}" );
        }

        private static void PrintCredits()
        {
            Console.WriteLine( Resources.GetCredits() );
        }

        private static void PrintLicense()
        {
            Console.WriteLine( Resources.GetLicense() );
        }

        private static void PrintVersion()
        {
            Console.WriteLine( GetVersion() );
        }

        private static string GetVersion()
        {
            return typeof( WebBuilder ).Assembly.GetName().Version?.ToString( 3 ) ?? "Unknown Version";
        }
    }
}
