using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MPSample.Common;
using MPSample.Domain;
using MPSample.Domain.Common;
using MPSample.Domain.Entities;
using MPSample.Domain.Services;
using MPSample.Infrastructure.DataAccess;
using MPSample.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using static MPSample.API.WholeAppSettings;

namespace MPSample.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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

        private void _configureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddDbContext<MPDbContext>(opt => opt.UseInMemoryDatabase("MPSampleDb"));
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

            services.Configure<Logging>(Configuration.GetSection("Logging"));
            services.Configure<CustomSettings>(Configuration.GetSection("CustomSettings"));


            bool useInMemory =  (bool)Configuration.GetSection("CustomSettings").GetValue(typeof(bool), "UseInMemoryDatabase");

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
            startupservice.Handle();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            _configureDateFormat(app);

        }
    }

    public class DataLoadStartup
    {
        
        private readonly UnitOfWork _unitOfWork;
        public DataLoadStartup(UnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            
        }

        public void Handle()
        {

            var itemsToAdd = new List<User> {
                new User { UserName = "tesla",Password = "tesla".ComputeSha256Hash()},
                new User { UserName = "rema",Password = "rema".ComputeSha256Hash()},
                new User { UserName = "mcdonald",Password = "mcdonald".ComputeSha256Hash()}
            };
            _unitOfWork.Users.AddRange(itemsToAdd);
            _unitOfWork.Commit();
        }
    }
}
