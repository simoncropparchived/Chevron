using System;
using System.IO;
using MsieJavaScriptEngine;
using Resourcer;
using Strike;

namespace Chevron
{
    public class HandleBars : IDisposable
    {
        MsieJsEngine engine;

        public HandleBars()
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

        public string TransformStringContext(string source, string context)
        {
            var dataPart = string.Format(@"var data ={0};", context);
            var sourcePart = string.Format(@"var source = '{0}';", source);

            engine.Execute(sourcePart + @"var template = Handlebars.compile(source);
" +
                           dataPart);

            return (string) engine.Evaluate("template(data)");
        }

        public void Dispose()
        {
        }
    }
}