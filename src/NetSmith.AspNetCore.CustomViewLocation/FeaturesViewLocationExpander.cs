using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;

namespace Communicator.AspNetCore.Mvc
{
    /// <summary>
    /// Custom view locator, view in feature folder
    /// </summary>
    public class FeaturesViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            context.Values["customviewlocation"] = nameof(FeaturesViewLocationExpander);
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var viewLocationFormats = new[]
            {
                "~/Features/{1}/{0}.cshtml",
                "~/Features/_Shared/{0}.cshtml"
            };

            return viewLocationFormats;
        }
    }
}
