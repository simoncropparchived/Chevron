using Nancy;
using Nancy.ViewEngines.Handlebars;

public class MainModule : NancyModule
{
    public MainModule()
    {
        Get["/"] = x =>
        {
            var version = typeof (ChevronViewEngine).Assembly.GetName().Version;
            var model = new
            {
                ViewEngineName = "Chevron",
                ViewEngineVersion = version.ToString(3)
            };
            return View["Index.handlebars", model];
        };
    }
}