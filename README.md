![Icon](https://raw.github.com/SimonCropp/Chevron/master/Icons/package_icon.png)

Wraps [Handlebars](http://handlebarsjs.com/) to make it usable from .net.

## Nuget

Nuget package http://nuget.org/packages/Chevron 

To Install from the Nuget Package Manager Console 
    
    PM> Install-Package Chevron

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

using (var handleBars = new HandleBars())
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
using (var handleBars = new HandleBars())
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
using (var handleBars = new HandleBars())
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

## The Internals

TODO

## Icon 

<a href="http://thenounproject.com/term/mustache/19592/" target="_blank">Mustache</a> designed by <a href="http://thenounproject.com/Mattebrooks/" target="_blank">Matt Brooks</a> from The Noun Project
