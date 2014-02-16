using System;
using System.IO;
using MsieJavaScriptEngine;
using Resourcer;
using Strike;

namespace Chevron
{
    public class Handlebars : IDisposable
    {
        MsieJsEngine engine;

        public Handlebars()
        {
            engine = new MsieJsEngine(JsEngineMode.Auto);

            var handlebarsJsText = GetHandlebarsJsText();
            engine.Execute(handlebarsJsText);
        }


        /// <summary>
        /// Get the content of handlebars.js
        /// By default handlebars.js is read from an embedded resource.
        /// </summary>
        public string GetHandlebarsJsText()
        {
            var markedPath = Path.Combine(AssemblyLocation.CurrentDirectory, @"handlebars.js");
            if (File.Exists(markedPath))
            {
                return File.ReadAllText(markedPath);
            }
            return Resource.AsString("handlebars.js");
        }

        public void RegisterHelper(string name, string js)
        {
            engine.Execute(@"Handlebars.registerHelper('" + name + "', " + js + ");");
        }

        public string Transform(string source, object context)
        {
            var serializeObject = SimpleJson.SerializeObject(context);
            return TransformStringContext(source, serializeObject);
        }

        public string TransformStringContext(string templateName, string context)
        {
            return (string)engine.Evaluate(string.Format("{0}_template({1});", templateName, context));
        }

        public void RegisterTemplate(string name, string source)
        {
            var js = string.Format(
                @"var {0}_source = '{1}';
var {0}_template = Handlebars.compile({0}_source);", name, source);
            engine.Execute(js);
        }

        public void RegisterPartial(string name, string content)
        {
            var js = string.Format("Handlebars.registerPartial('{0}', '{1}');", name, content);
            engine.Execute(js);
        }

        public void Dispose()
        {
        }

    }
}