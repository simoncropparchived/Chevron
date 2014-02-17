![Icon](https://raw.github.com/SimonCropp/Chevron/master/Icons/package_icon.png)

Wraps [HandlebarsJS](http://handlebarsjs.com/) to make it usable from .net.

## Nuget

### Core

Nuget package http://nuget.org/packages/Chevron 

    PM> Install-Package Chevron

### NancyFX integration

Nuget package http://nuget.org/packages/Nancy.ViewEngines.Chevron

    PM> Install-Package Nancy.ViewEngines.Chevron

## Usage

### Rendering a template

#### Input
```
var source = @"<p>Hello, my name is {{name}}. I am from {{hometown}}. I have ' +
        '{{kids.length}} kids:</p>' +
        '<ul>{{#kids}}<li>{{name}} is {{age}}</li>{{/kids}}</ul>";

var context = new
{
    name = "Alan",
    hometown = "Somewhere, TX",
    kids = new[]
        {
            new
            {
                name = "Sally",
                age = "4"
            }
        }
};

using (var handleBars = new Handlebars())
{
    handleBars.RegisterTemplate("myTemplate", source);
    Approvals.Verify(handleBars.Transform("myTemplate", context));
}
```

#### Output
```
<p>Hello, my name is Alan. I am from Somewhere, TX. I have 1 kids:</p>
<ul>
	<li>Sally is 4</li>
</ul>
```

### Register Helpers

#### Input
```
var source = "<ul>{{#posts}}<li>{{link_to}}</li>{{/posts}}</ul>";
var context = new
{
    posts = new[]
        {
            new
            {
                url = "/hello-world",
                body = "Hello World!"
            }
        }
};
using (var handleBars = new Handlebars())
{
    handleBars.RegisterHelper("link_to",
        @"function() {
return new Handlebars.SafeString(""<a href='"" + this.url + ""'>"" + this.body + ""</a>"");
}");
    handleBars.RegisterTemplate("myTemplate", source);
    Approvals.Verify(handleBars.Transform("myTemplate", context));
}
```
#### Output
```
<ul>
	<li>
		<a href='/hello-world'>Hello World!</a>
	</li>
</ul>
```

### Register Partials

#### Input
```
var source = "<ul>{{#people}}<li>{{> link}}</li>{{/people}}</ul>";
var context = new
{
    people = new[]
        {
            new
            {
                name = "Alan",
                id = 1
            },
            new
            {
                name = "Yehuda",
                id = 2
            }
        }
};
using (var handleBars = new Handlebars())
{
    handleBars.RegisterPartial("link",@"<a href=""/people/{{id}}"">{{name}}</a>");
    handleBars.RegisterTemplate("myTemplate", source);
    Approvals.Verify(handleBars.Transform("myTemplate", context));
}
```
#### Output
```
<ul>
	<li><a href="/people/1">Alan</a></li>
	<li><a href="/people/2">Yehuda</a></li>
</ul>
```

## HandlebarsJS

The binary ships with a resource merged version of [HandlebarsJS](http://handlebarsjs.com/). Also see the [License]( https://github.com/wycats/handlebars.js/blob/master/LICENSE).

### Current merged version

The current version included in the library is v1.3.0. If you feel a newer version should be included at any point in time please raise an issue.

### Running a custom version

If you want to run a custom version of HandlebaseJS simply place the custom `handlebars.js` in the current running directory and that file will be used instead of the merged version. 

## MSIE JavaScript Engine for .NET

The binary ships with an ILMerged copy of the [MSIE JavaScript Engine](https://github.com/Taritsyn/MsieJavaScriptEngine/). It also gives ceridt to various other libraries for its' inspiration [MSIE JavaScript Engine Credits](https://github.com/Taritsyn/MsieJavaScriptEngine#credits). Also see the [License](http://github.com/Taritsyn/MsieJavaScriptEngine/blob/master/LICENSE.md).

## Icon 

<a href="http://thenounproject.com/term/mustache/19592/" target="_blank">Mustache</a> designed by <a href="http://thenounproject.com/Mattebrooks/" target="_blank">Matt Brooks</a> from The Noun Project
