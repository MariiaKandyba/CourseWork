using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDesigner.Services;
using TestDesigner.Services.TestDesigner.Services;

namespace TestDesigner
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<MainWindow>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<ITestService, TestService>();
            services.AddTransient<ISerializationService, SerializationService>();

            return services.BuildServiceProvider();
        }
    }

}
