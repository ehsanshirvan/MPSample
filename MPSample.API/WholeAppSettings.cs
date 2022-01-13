using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static MPSample.API.WholeAppSettings;

namespace MPSample.API
{
    public class WholeAppSettings
    {
        public class AppSettings
        {
            public Logging Logging { get; set; }
            public string AllowedHosts { get; set; }
            public CustomSettings CustomSettings { get; set; }
        }

        public class CustomSettings
        {
            public bool UseInMemoryDatabase { get; set; }
        }

        public class Logging
        {
            public LogLevel LogLevel { get; set; }
        }

        public class LogLevel
        {
            public string Default { get; set; }

            public string Warning { get; set; }

            public string Error { get; set; }
        }
    }


    public class AppSettingModel : PageModel
    {
        private readonly CustomSettings _options;

        public AppSettingModel(IOptions<CustomSettings> options)
        {
            _options = options.Value;
        }

        public  bool UseInMemoryMode()
        {
            return _options.UseInMemoryDatabase;
        }
    }

}
