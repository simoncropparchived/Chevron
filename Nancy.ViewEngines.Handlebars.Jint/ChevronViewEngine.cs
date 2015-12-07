using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chevron;
using Nancy.Responses;

namespace Nancy.ViewEngines.Handlebars
{
    /// <summary>
    /// View engine for rendering mustache views.
    /// </summary>
    public class ChevronViewEngine : IViewEngine, IDisposable
    {
        IViewLocator viewLocator;
        ThreadLocalHandlebars threadLocalHandlebars;

        public ChevronViewEngine(ThreadLocalHandlebars threadLocalHandlebars)
        {
            this.threadLocalHandlebars = threadLocalHandlebars;
        }

        public ChevronViewEngine():this(new ThreadLocalHandlebars())
        {
        }

        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        public IEnumerable<string> Extensions => new[] {"handlebars"};

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

                    var handlebars = threadLocalHandlebars.Value;
                    handlebars.RegisterTemplate(templateName, () =>
                    {
                        using (var textReader = viewLocationResult.Contents())
                        {
                            return textReader.ReadToEnd();
                        }

                    });
                    foreach (var partial in viewLocator.GetAllCurrentlyDiscoveredViews().Where(x => x.Name.StartsWith("_")))
                    {
                        var partialName = partial.Name.TrimStart('_');
                        handlebars.RegisterPartial(partialName, () =>
                        {
                            using (var textReader = partial.Contents())
                            {
                                return textReader.ReadToEnd();
                            }
                        });
                    }
                    using (var writer = new StreamWriter(stream))
                    {
                        dynamic output;
                        try
                        {
                            output = handlebars.Transform(templateName, model);
                        }
                        catch (Exception)
                        {
                            //TODO: remove this exception handling after a few versions
                            var templateContents = viewLocationResult.Contents().ReadToEnd();
                            if (templateContents.Contains("{{> _") || templateContents.Contains("{{>_"))
                            {
                                throw new Exception($"Template '{templateName}' contains and underscore prefixed partial name. This is no longer required. Search for the string '{{>_' or '{{> _' in your template and remove the '_'.");
                            }
                            throw;
                        }
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
