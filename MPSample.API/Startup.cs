using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MPSample.API.Extentions;
using MPSample.Common;
using MPSample.Domain;
using MPSample.Domain.Common;
using MPSample.Domain.Services;
using MPSample.Infrastructure.DataAccess;
using NSwag;
using NSwag.Generation.Processors.Security;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MPSample.API
{
    public class Startup
    {
        #region Properties
        public IConfiguration Configuration { get; }

        #endregion 

        #region Public Methods
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _configureServices(services);

            services.AddControllers(options =>
            {
                var jsonInputFormatter = options.InputFormatters
                    .OfType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonInputFormatter>()
                    .Single();
                jsonInputFormatter.SupportedMediaTypes.Add("application/json");
            }
            );
            services.AddMvc(x => x.EnableEndpointRouting = false);
              

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerFactory logger)
        {
            logger.AddFile("Logs/mylog-{Date}.txt");

            app.ApplyUserKeyValidation();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStaticFiles();
                app.UseDeveloperExceptionPage();
                
            }

            

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            _configureSwagger(app);

            _configureDateFormat(app);


        }

        private void _configureSwagger(IApplicationBuilder app)
        {
          
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }

        #endregion

        #region Private Methods
        private void _configureServices(IServiceCollection services)
        {
                                                       
            services.RegisterDbContexts(Configuration);
            services.RegisterRepositories();
            services.RegisterCommonServices();


            var serviceprovider = services.BuildServiceProvider();
            var startupservice = serviceprovider.GetRequiredService<DataLoadStartup>();

            services.AddControllers()
            .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.RegisterthirdParties();

            _loadPrimitiveData(startupservice);

        }

       

        private void _configureDateFormat(IApplicationBuilder app)
        {
            var defaultCulture = new CultureInfo("en-US");
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = new List<CultureInfo> { defaultCulture },
                SupportedUICultures = new List<CultureInfo> { defaultCulture }
            });
        }

        private void _loadPrimitiveData(DataLoadStartup startupservice)
        {
            startupservice.Load();
        }

        #endregion

    }
}
