namespace Chevron.Mvc
{
    using System;
    using System.Web.Mvc;

    using Chevron;

    public class ChevronViewEngine : IViewEngine
    {
        ThreadLocalHandlebars threadLocalHandlebars;

        public ChevronViewEngine(ThreadLocalHandlebars threadLocalHandlebars)
        {
            this.threadLocalHandlebars = threadLocalHandlebars;
        }

        public ChevronViewEngine() : this(new ThreadLocalHandlebars())
        {
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            throw new NotImplementedException();
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return new ViewEngineResult(new HandlebarsView(this.threadLocalHandlebars, controllerContext, viewName), this);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            // WTF
        }
    }
}
