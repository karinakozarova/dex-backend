/*
* Digital Excellence Copyright (C) 2020 Brend Smits
* 
* This program is free software: you can redistribute it and/or modify 
* it under the terms of the GNU Lesser General Public License as published 
* by the Free Software Foundation version 3 of the License.
* 
* This program is distributed in the hope that it will be useful, 
* but WITHOUT ANY WARRANTY; without even the implied warranty 
* of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
* See the GNU Lesser General Public License for more details.
* 
* You can find a copy of the GNU Lesser General Public License 
* along with this program, in the LICENSE.md file in the root project directory.
* If not, see https://www.gnu.org/licenses/lgpl-3.0.txt
*/

using Configuration;
using IdentityServer.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{

    /// <summary>
    ///     Startup file for Identity Server
    /// </summary>
    public class Startup
    {

        /// <summary>
        ///     Startup constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Config = configuration.GetSection("App")
                                  .Get<Config>();
            Configuration = configuration;
            Environment = environment;
        }

        /// <summary>
        ///     Configuration for Identity server
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        ///     Config for Identity server
        /// </summary>
        public Config Config { get; }

        public IWebHostEnvironment Environment { get; }

        /// <summary>
        ///     Configure services for the identity server
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // configures the OpenIdConnect handlers to persist the state parameter into the server-side IDistributedCache.
            services.AddOidcStateDataFormatterCache();

            services.AddControllersWithViews();

            IIdentityServerBuilder builder = services.AddIdentityServer(options =>
                                                     {
                                                         options.Events.RaiseErrorEvents = true;
                                                         options.Events.RaiseInformationEvents =
                                                             true;
                                                         options.Events.RaiseFailureEvents = true;
                                                         options.Events.RaiseSuccessEvents = true;
                                                     })
                                                     .AddTestUsers(TestUsers.Users);

            // in-memory, code config
            builder.AddInMemoryIdentityResources(IdentityConfig.Ids);
            builder.AddInMemoryApiResources(IdentityConfig.Apis);
            builder.AddInMemoryClients(IdentityConfig.Clients(Config));
            builder.AddTestUsers(TestUsers.Users);

            // services.AddAuthentication()
            //     .AddOpenIdConnect("FHICT", "Fontys FHICT", options =>
            //     {
            //         options.ClientId = "";
            //         options.ClientSecret = "";
            //         options.Authority = "";
            //         // ...
            //     });

            if(Environment.IsDevelopment())
            {
                //TODO: Have some sort of certificate on the production servers
                // not recommended for production - you need to store your key material somewhere secure
                builder.AddDeveloperSigningCredential();
            }
            services.AddCors(options =>
            {
                options.AddPolicy("dex-api",
                                  policy =>
                                  {
                                      policy.AllowAnyOrigin()
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                                  });
            });
        }

        /// <summary>
        ///     Configure the application
        /// </summary>
        /// <param name="app"></param>
        public void Configure(IApplicationBuilder app)
        {
            if(Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseCors("dex-api");
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
        }

    }

}