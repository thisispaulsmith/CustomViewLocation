using Communicator.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FeaturesViewLocationExtensions
    {
        public static void AddCustomViewLocator(this IServiceCollection services)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new FeaturesViewLocationExpander());
            });
        }
    }
}
