using Microsoft.AspNetCore.Builder;
using MPSample.API.Middlewares;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPSample.Common
{
    public static class UserValidator
    {
        public static IApplicationBuilder ApplyUserKeyValidation(this IApplicationBuilder app)
        {
            app.UseMiddleware<AuthMiddleware>();
            return app;
        }
    }

}
