using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chevron;
using Nancy.Responses;

namespace Nancy.ViewEngines.Chevron
{
    /// <summary>
    /// View engine for rendering mustache views.
    /// </summary>
    public class ChevronViewEngine : IViewEngine, IDisposable
    {
        IViewLocator viewLocator;
        ThreadLocalHandlebars handlebars;

        public ChevronViewEngine()
        {
            handlebars = new ThreadLocalHandlebars();
        }

        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        public IEnumerable<string> Extensions
        {
            get { return new[] {"handlebars"}; }
        }

        /// <summary>
        /// Initialise the view engine (if necessary)
        /// </summary>
        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
            viewLocator = viewEngineStartupContext.ViewLocator;
        }


        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
        /// <param name="model">The model to be passed into the view</param>
        /// <param name="renderContext"></param>
        /// <returns>A response</returns>
        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            return new HtmlResponse
            {
                Contents = stream =>
                {
                    var templateName = viewLocationResult.Name;
                    
                    handlebars.Value.RegisterTemplate(templateName, () =>
                    {
                        using (var textReader = viewLocationResult.Contents())
                        {
                            return textReader.ReadToEnd();
                        }

                    });
                    foreach (var partial in viewLocator.GetAllCurrentlyDiscoveredViews().Where(x => x.Name.StartsWith("_")))
                    {
                        handlebars.Value.RegisterPartial(partial.Name, () =>
                        {
                            using (var textReader = partial.Contents())
                            {
                                return textReader.ReadToEnd();
                            }
                        });
                    }
                    using (var writer = new StreamWriter(stream))
                    {
                        var output = handlebars.Value.Transform(templateName, model);
                        writer.Write(output);
                    }
                }
            };
        }

        public void Dispose()
        {
            
        }
    }
}
