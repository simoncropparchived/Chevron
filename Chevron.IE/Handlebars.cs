using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Resourcer;

namespace Chevron
{

    public class Handlebars : IDisposable
    {
#if (IE)
        MsieJavaScriptEngine.MsieJsEngine engine;

        public Handlebars(MsieJavaScriptEngine.MsieJsEngine engine)
        {
            this.engine = engine;
            var handlebarsJsText = GetHandlebarsJsText();
            engine.Execute(handlebarsJsText);
        }

        public Handlebars()
            : this(new MsieJavaScriptEngine.MsieJsEngine(MsieJavaScriptEngine.JsEngineMode.Auto))
        {
        }
#endif
#if (Jint)
        Jint.Engine engine;

        public Handlebars(Jint.Engine engine)
        {
            this.engine = engine;
            var handlebarsJsText = GetHandlebarsJsText();
            engine.Execute(handlebarsJsText);
        }

        public Handlebars()
            : this(new Jint.Engine())
        {
        }
#endif
#if (V8)
        Microsoft.ClearScript.V8.V8ScriptEngine engine;

        public Handlebars(Microsoft.ClearScript.V8.V8ScriptEngine engine)
        {
            this.engine = engine;
            var handlebarsJsText = GetHandlebarsJsText();
            engine.Execute(handlebarsJsText);
        }

        public Handlebars()
            : this(new Microsoft.ClearScript.V8.V8ScriptEngine())
        {
        }
#endif

        List<string> registeredTemplates = new List<string>();
        List<string> registeredPartials = new List<string>(); 
        List<string> registeredHelpers = new List<string>(); 


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


#if(Jint)

        public string Transform(string templateName, object context)
        {
            return TransformStringContext(templateName,  context);
        }
        public string TransformStringContext(string templateName, object o)
        {
            templateName = templateName.ToLowerInvariant();

            CheckTemplate(templateName);
            return engine.Invoke("chevronTemplate_" + templateName, o).AsString();
        }
#else
        
        public string Transform(string templateName, object context)
        {
            var serializeObject = SerializeObject(context);
            return TransformStringContext(templateName, serializeObject, context);
        }

        public string SerializeObject(object context)
        {
            if (context == null)
            {
                return "{}";
            }
            return IE.SimpleJson.SerializeObject(context);
        }

        public string TransformStringContext(string templateName, string context, object o)
        {
            templateName = templateName.ToLowerInvariant();

            CheckTemplate(templateName);
            var expression = string.Format("chevronTemplate_{0}({1});", templateName, context);
            return (string)engine.Evaluate(expression);
        }

#endif
        void CheckTemplate(string templateName)
        {
            if (!registeredTemplates.Contains(templateName))
            {
                throw new Exception(string.Format("Could not find a template named '{0}'.", templateName));
            }
        }

        public void RegisterTemplate(string name, string source)
        {
            RegisterTemplate(name, ()=>source);
        }

        public void RegisterTemplate(string templateName, Func<string> content)
        {
            templateName = templateName.ToLowerInvariant();
            if (!registeredTemplates.Contains(templateName))
            {
                VariableNameValidator.ValidateSuffix(templateName);
                registeredTemplates.Add(templateName);
                var templateContent = content();
                templateContent = SanitizeContent(templateContent);
                var code = string.Format(
                    @"var {0}_source = '{1}';
var chevronTemplate_{0} = Handlebars.compile({0}_source);", templateName, templateContent);
                engine.Execute(code);
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
                if (IsHelper(trim))
                {
                    stringBuilder.Append(trim);
                    continue;
                }
                if (stringReader.Peek() == -1)
                {
                    stringBuilder.Append(line);
                }
                else
                {
                    stringBuilder.AppendLine(line);
                }
            }

            var result = stringBuilder.ToString();
            return HttpUtility.JavaScriptStringEncode(result);
        }

        static bool IsHelper(string trim)
        {
            return (trim.StartsWith("{{#") || trim.StartsWith("{{/")) && trim.EndsWith("}}");
        }

        public void RegisterPartial(string partialName, string content)
        {
            RegisterPartial(partialName, () => content);
        }

        public void RegisterPartial(string partialName, Func<string> content)
        {
            if (!registeredPartials.Contains(partialName))
            {
                registeredPartials.Add(partialName);
                var templateContent = content();
                templateContent = SanitizeContent(templateContent);
                var code = string.Format("Handlebars.registerPartial('{0}', '{1}');", partialName, templateContent);
                engine.Execute(code);
            }
        }

        public void Dispose()
        {
        }

    }
}