using Nancy;
using Nancy.ViewEngines.Chevron;

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
            return View["Index.handebars", model];
        };

    }
}