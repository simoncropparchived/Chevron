namespace Chevron.Mvc
{
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;

    using Chevron;

    public class HandlebarsView : IView
    {
        private readonly ThreadLocalHandlebars threadLocalHandlebars;

        private readonly ControllerContext controllerContext;

        private readonly string viewName;

        public HandlebarsView(ThreadLocalHandlebars threadLocalHandlebars, ControllerContext controllerContext, string viewName)
        {
            this.threadLocalHandlebars = threadLocalHandlebars;
            this.controllerContext = controllerContext;
            this.viewName = viewName;
        }

        public HandlebarsView(ControllerContext controllerContext, string viewName) :this(new ThreadLocalHandlebars(), controllerContext, viewName)
        {
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            var handlebars = this.threadLocalHandlebars.Value;
            var path = this.controllerContext.HttpContext.Server.MapPath("~/Templates/");
            Directory.GetFiles(path, "*.handlebars", SearchOption.AllDirectories).ToList().ForEach(
                (file) =>
                    {
                        var name = file.Replace(path, "");
                        name = name.Substring(0, name.LastIndexOf("."));
                        handlebars.RegisterPartial(name.Replace("\\", "/"), () => File.ReadAllText(file));
                        handlebars.RegisterTemplate(name.Replace("\\", "_").Replace("-", "_").Replace(".", "_"), () => File.ReadAllText(file));
                    });

            //handlebars.Execute(File.ReadAllText(this.controllerContext.HttpContext.Server.MapPath("~/Templates/underscore.js")));

            var output = handlebars.Transform(viewName, viewContext.ViewData.Model);
            writer.Write(output);
        }
    }
}