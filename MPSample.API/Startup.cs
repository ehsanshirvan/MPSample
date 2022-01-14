using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MPSample.Common;
using MPSample.Domain;
using MPSample.Domain.Common;
using MPSample.Domain.Entities;
using MPSample.Domain.Services;
using MPSample.Infrastructure.DataAccess;
using MPSample.Infrastructure.Repositories;
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
                // app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MPSample.API v1"));
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
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            //app.UseSwagger();
            //app.UseSwaggerUI();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }

        #endregion

        #region Private Methods
        private void _configureServices(IServiceCollection services)
        {
            services.AddControllers();
            _configureDatabaseConnection(services);
            services.AddScoped<UnitOfWork>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<UserRepository>();
            services.AddScoped<DataLoadStartup>();
            services.AddScoped<TransactionService>();
            services.AddScoped<IRepository<Transaction>, TransactionRepository>();
            services.AddScoped<TransactionRepository>();
            var serviceprovider = services.BuildServiceProvider();
            var startupservice = serviceprovider.GetRequiredService<DataLoadStartup>();
            services.AddControllers()
            .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddOpenApiDocument(option =>
            {
                option.Title = "MPSample.API";
                option.Version = "1";

                option.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
                option.DocumentProcessors.Add(new SecurityDefinitionAppender("user", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.Basic,
                    Name = "user",
                    
                    In = OpenApiSecurityApiKeyLocation.Header,
                    //Description = "Type into the textbox: Bearer {your JWT token}."
                }));

                //option.DocumentProcessors.Add(new SecurityDefinitionAppender("password", new OpenApiSecurityScheme
                //{
                //    Type = OpenApiSecuritySchemeType.ApiKey,
                //    Name = "password",
                //    In = OpenApiSecurityApiKeyLocation.Header,
                //   // Description = "Type into the textbox: Bearer {your JWT token}."
                //}));

            });

            _loadPrimitiveData(startupservice);

        }

        private void _configureDatabaseConnection(IServiceCollection services)
        {
            bool useInMemory = (bool)Configuration.GetSection("CustomSettings").GetValue(typeof(bool), "UseInMemoryDatabase");
            if (useInMemory)
                services.AddDbContext<MPDbContext>(opt => opt.UseInMemoryDatabase("MPSampleDb"));
            else
                services.AddDbContext<MPDbContext>(item => item.UseSqlServer(Configuration.GetConnectionString("conString")));
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

    public class DataLoadStartup
    {
        
        private readonly UnitOfWork _unitOfWork;
        public DataLoadStartup(UnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            
        }

        public void Load()
        {
            var teslaUser = new User { UserName = "tesla", Password = "tesla".ComputeSha256Hash() };
            var remaUser = new User { UserName = "rema", Password = "rema".ComputeSha256Hash() };
                var mcdonaldUser = new User { UserName = "mcdonald", Password = "mcdonald".ComputeSha256Hash() };

            var currentUsers = _unitOfWork.Users.GetAll();
            if (!currentUsers.Any(x => x.UserName == teslaUser.UserName))
                _unitOfWork.Users.Add(teslaUser);

            if (!currentUsers.Any(x => x.UserName == remaUser.UserName))
                _unitOfWork.Users.Add(remaUser);

            if (!currentUsers.Any(x => x.UserName == mcdonaldUser.UserName))
                _unitOfWork.Users.Add(mcdonaldUser);

            _unitOfWork.Commit();
        }
    }
}
