using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using MsieJavaScriptEngine;
using Resourcer;
using Strike;

namespace Chevron
{
    public class Handlebars : IDisposable
    {
        MsieJsEngine engine;

        List<string> registeredTemplates = new List<string>();
        List<string> registeredPartials = new List<string>(); 
        List<string> registeredHelpers = new List<string>(); 

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
            RegisterHelper(name, () => js);
        }

        public void RegisterHelper(string name, Func<string> js)
        {
            if (!registeredHelpers.Contains(name))
            {
                registeredHelpers.Add(name);
                var code = string.Format(@"Handlebars.registerHelper('{0}', {1});", name, js());
                engine.Execute(code);
            }
        }

        public string Transform(string templateName, object context)
        {
            string serializeObject;
            if (context == null)
            {
                serializeObject = "{}";
            }
            else
            {
                serializeObject = SimpleJson.SerializeObject(context);
            }
            return TransformStringContext(templateName, serializeObject);
        }

        public string TransformStringContext(string templateName, string context)
        {
            templateName = templateName.ToLowerInvariant();
            return (string)engine.Evaluate(string.Format("{0}_template({1});", templateName, context));
        }

        public void RegisterTemplate(string name, string source)
        {
            RegisterTemplate(name, ()=>source);
        }

        public void RegisterTemplate(string name, Func<string> content)
        {
            name = name.ToLowerInvariant();
            if (!registeredTemplates.Contains(name))
            {
                VariableNameValidator.ValidateSuffix(name);
                registeredTemplates.Add(name);
                var templateContent = content();
                templateContent = SanitizeContent(templateContent);
                var js = string.Format(
                    @"var {0}_source = '{1}';
var {0}_template = Handlebars.compile({0}_source);", name, templateContent);
                engine.Execute(js);
            }
        }

        static string SanitizeContent(string templateContent)
        {
            var stringReader = new StringReader(templateContent);
            var stringBuilder = new StringBuilder();
            string line;
            while ((line = stringReader.ReadLine()) != null)
            {
                var trim = line.Trim();
                if (trim.StartsWith("{{") && trim.EndsWith("}}"))
                {
                    stringBuilder.Append(trim);
                    continue;
                }
                stringBuilder.AppendLine(line);
            }
            return HttpUtility.JavaScriptStringEncode(stringBuilder.ToString());
        }

        public void RegisterPartial(string name, string content)
        {
            RegisterPartial(name, () => content);
        }

        public void RegisterPartial(string name, Func<string> content)
        {
            name = name.ToLowerInvariant();
            if (!registeredPartials.Contains(name))
            {
                VariableNameValidator.ValidateSuffix(name);
                registeredPartials.Add(name);
                var templateContent = content();
                templateContent = SanitizeContent(templateContent);
                var js = string.Format("Handlebars.registerPartial('{0}', '{1}');", name, templateContent);
                engine.Execute(js);
            }
        }

        public void Dispose()
        {
        }

    }
}