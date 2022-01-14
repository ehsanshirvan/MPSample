using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MPSample.Domain;
using MPSample.Domain.Common;
using MPSample.Domain.Entities;
using MPSample.Domain.Services;
using MPSample.Infrastructure.DataAccess;
using MPSample.Infrastructure.Repositories;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace MPSample.API.Extentions
{
    public static class ServiceCollectionExtention
    {
        
        public static void RegisterDbContexts(this IServiceCollection services, IConfiguration Configuration)
        {
            bool useInMemory = (bool)Configuration.GetSection("CustomSettings").GetValue(typeof(bool), "UseInMemoryDatabase");
            if (useInMemory)
                services.AddDbContext<MPDbContext>(opt => opt.UseInMemoryDatabase("MPSampleDb"));
            else
                services.AddDbContext<MPDbContext>(item => item.UseSqlServer(Configuration.GetConnectionString("conString")));

        }
        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<UnitOfWork>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<UserRepository>();
            services.AddScoped<IRepository<Transaction>, TransactionRepository>();
            services.AddScoped<TransactionRepository>();
        }
      
        public static void RegisterCommonServices(this IServiceCollection services)
        {

            services.AddScoped<DataLoadStartup>();
            services.AddScoped<TransactionService>();
        }

        
        public static void RegisterthirdParties(this IServiceCollection services)
        {

            services.AddOpenApiDocument(option =>
            {
                option.Title = "MPSample.API";
                option.Version = "1";

                option.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
               
                option.DocumentProcessors.Add(new SecurityDefinitionAppender("basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type=OpenApiSecuritySchemeType.Http,
                    Scheme = "basic",
                    In =OpenApiSecurityApiKeyLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme."
                }));



            });
        }
    }
}
