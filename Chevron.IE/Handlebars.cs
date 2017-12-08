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
            Guard.AgainstNull(engine, "engine");
            this.engine = engine;
            var handlebarsJsText = GetHandlebarsJsText();
            engine.Execute(handlebarsJsText);
        }

        public Handlebars()
            : this(new MsieJavaScriptEngine.MsieJsEngine(new MsieJavaScriptEngine.JsEngineSettings
            {
                EngineMode = MsieJavaScriptEngine.JsEngineMode.Auto
            }))
        {
        }
#endif
#if (Jint)
        Jint.Engine engine;

        public Handlebars(Jint.Engine engine)
        {
            Guard.AgainstNull(engine, "engine");
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
            Guard.AgainstNull(engine, "engine");
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
            var markedPath = Path.Combine(AssemblyLocation.CurrentDirectory, "handlebars.js");
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
            Guard.AgainstNull(js, "js");
            Guard.AgainstNullAndEmpty(name, "name");
            if (!registeredHelpers.Contains(name))
            {
                registeredHelpers.Add(name);
                var code = $@"Handlebars.registerHelper('{name}', {js()});";
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
            Guard.AgainstNullAndEmpty(templateName, "templateName");
            templateName = templateName.ToLowerInvariant();

            CheckTemplate(templateName);
            return engine.Invoke("chevronTemplate_" + templateName, o).AsString();
        }
#else

        public string Transform(string templateName, object context)
        {
            Guard.AgainstNullAndEmpty(templateName, "templateName");
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
            Guard.AgainstNullAndEmpty(templateName, "templateName");
            Guard.AgainstNull(context, "context");
            templateName = templateName.ToLowerInvariant();

            CheckTemplate(templateName);
            var expression = $"chevronTemplate_{templateName}({context});";
            return (string)engine.Evaluate(expression);
        }

#endif
        void CheckTemplate(string templateName)
        {
            if (!registeredTemplates.Contains(templateName))
            {
                throw new Exception($"Could not find a template named '{templateName}'.");
            }
        }

        public void RegisterTemplate(string templateName, string source)
        {
            RegisterTemplate(templateName, () => source);
        }

        public void RegisterTemplate(string templateName, Func<string> content)
        {
            Guard.AgainstNullAndEmpty(templateName, "templateName");
            if (char.IsNumber(templateName[0]))
            {
                throw new ArgumentException("'templateName' cannot start with a number.","templateName");
            }
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
            if (templateContent.EndsWith("\n"))
            {
                stringBuilder.AppendLine();
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
            Guard.AgainstNullAndEmpty(content, "content");
            RegisterPartial(partialName, () => content);
        }

        public void RegisterPartial(string partialName, Func<string> content)
        {
            Guard.AgainstNullAndEmpty(partialName, "partialName");
            Guard.AgainstNull(content, "content");
            if (!registeredPartials.Contains(partialName))
            {
                registeredPartials.Add(partialName);
                var templateContent = content();
                templateContent = SanitizeContent(templateContent);
                var code = $"Handlebars.registerPartial('{partialName}', '{templateContent}');";
                engine.Execute(code);
            }
        }

        public void Dispose()
        {
        }
    }
}