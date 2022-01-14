using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MPSample.Common;
using MPSample.Infrastructure.Repositories;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace MPSample.API.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext,UserRepository repo)
        {

            bool requestIsValid = _authRequest(httpContext,repo);


            return _next(httpContext);
        }

        private bool _authRequest(HttpContext httpContext,UserRepository repo)
        {

            if (httpContext.Request.Path.HasValue && httpContext.Request.Path.ToString().ToLower().Contains("swagger"))
                return true;

            var user = httpContext.Request.Headers["user"].ToString().ToLower();
            var pass = httpContext.Request.Headers["password"].ToString().ToLower();

            if (!repo.GetBy(x => x.UserName == user && x.Password == pass.ComputeSha256Hash()).Any())
            {
                httpContext.Response.StatusCode = 401;
                httpContext.Response.WriteAsync("Invalid User");
                return false;

            }
            
            if (httpContext.Request.Path.HasValue && httpContext.Request.Method == "GET")
            {
                var pathParts = httpContext.Request.Path.Value.Split("/");
                var parameter = pathParts[pathParts.Length - 1];
                if (user != parameter)
                {
                    httpContext.Response.StatusCode = 401;
                    httpContext.Response.WriteAsync("You are not authorized to access another company");
                    return false;
                }
            }
            if (httpContext.Request.Path.HasValue && httpContext.Request.Method == "POST")
            {
                var requestReader = new StreamReader(httpContext.Request.Body);
          //      var requestContent = requestReader.ReadToEnd();



            }

                



            return true;

        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
