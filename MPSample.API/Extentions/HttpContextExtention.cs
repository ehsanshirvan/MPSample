using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
namespace MPSample.API.Extentions
{
    public static class HttpContextExtention
    {
        public static string GetRawRequestBodyString(this HttpContext httpContext, Encoding encoding)
        {
            try
            {
                var body = "";
                if (httpContext.Request.ContentLength == null || !(httpContext.Request.ContentLength > 0))
                    return body;

                httpContext.Request.EnableBuffering();
                httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(httpContext.Request.Body, encoding, true, 1024, true))
                {
                    body = reader.ReadToEndAsync().Result;
                }
                httpContext.Request.Body.Position = 0;
                return body;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static T GetRequestBodyContent<T>(this HttpContext httpContext, Encoding encoding)
        {
            try
            {
                var body = "";
                if (httpContext.Request.ContentLength == null || !(httpContext.Request.ContentLength > 0))
                    return default(T);

                httpContext.Request.EnableBuffering();
                httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(httpContext.Request.Body, encoding, true, 1024, true))
                {
                    body = reader.ReadToEndAsync().Result;
                }
                httpContext.Request.Body.Position = 0;
                return JsonConvert.DeserializeObject<T>(body);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

    }
}
