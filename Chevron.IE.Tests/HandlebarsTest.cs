using System;
using ApprovalTests;
using Chevron;
using NUnit.Framework;

[TestFixture]
public class HandlebarsTest
{
    [Test]
    public void Sample()
    {
        var source = @"
<p>Hello, my name is {{name}}. I am from {{hometown}}. I have {{kids.length}} kids:</p>
<ul>
    {{#kids}}
    <li>{{name}} is {{age}}</li>
    {{/kids}}
</ul>";

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
            handleBars.RegisterTemplate("Index", source);
            Approvals.Verify(handleBars.Transform("Index", context));
        }
    }

    [Test]
    public void NewLineInTemplate()
    {
        var source = "AA\r\nBB";

        using (var handleBars = new Handlebars())
        {
            handleBars.RegisterTemplate("Index", source);
            Approvals.Verify(handleBars.Transform("Index", null));
        }
    }

    [Test]
    public void NumberInTemplateName()
    {
        var source = "AA\r\nBB";

        using (var handleBars = new Handlebars())
        {
            handleBars.RegisterTemplate("Index1", source);
            Approvals.Verify(handleBars.Transform("Index1", null));
        }
    }

    [Test]
    public void NumberAtStartOfTemplateName()
    {
        using (var handleBars = new Handlebars())
        {
            var argumentException = Assert.Throws<ArgumentException>(() => { handleBars.RegisterTemplate("1Index", "AA"); });
            Assert.AreEqual("'templateName' cannot start with a number.\r\nParameter name: templateName", argumentException.Message);
        }
    }

    [Test]
    public void RegisterHelperSample()
    {
        var helperJs =
            @"function() {
return new Handlebars.SafeString(""<a href='"" + this.url + ""'>"" + this.body + ""</a>"");
}";
        var source = @"
<ul>
{{#posts}}
    <li>{{link_to}}</li>
{{/posts}}
</ul>";
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
            handleBars.RegisterHelper("link_to", helperJs);
            handleBars.RegisterTemplate("myTemplate", source);
            Approvals.Verify(handleBars.Transform("myTemplate", context));
        }
    }

    [Test]
    public void CaseInsensitive()
    {
        var source = "Foo";
        using (var handleBars = new Handlebars())
        {
            handleBars.RegisterTemplate("mytemplate", source);
            Approvals.Verify(handleBars.Transform("myTemplate", null));
        }
    }

    [Test]
    public void RegisterPartialsSample()
    {
        var partial = @"<a href=""/people/{{id}}"">{{name}}</a>";

        var source = @"
<ul>
{{#people}}
    <li>{{> link}}</li>
{{/people}}
</ul>";

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
                    name = "John",
                    id = 2
                }
            }
        };
        using (var handleBars = new Handlebars())
        {
            handleBars.RegisterPartial("link", partial);
            handleBars.RegisterTemplate("myTemplate", source);
            Approvals.Verify(handleBars.Transform("myTemplate", context));
        }
    }
    [Test]
    public void PartialWithNewline()
    {
        var partial = "thepartial\r\n";

        var source = @"<li>{{> partial}}</li>";
        using (var handleBars = new Handlebars())
        {
            handleBars.RegisterPartial("partial", partial);
            handleBars.RegisterTemplate("myTemplate", source);
            Approvals.Verify(handleBars.Transform("myTemplate", new object()));
        }
    }
    [Test]
    [Ignore]
    public void MissingPartial()
    {
        var partial = @"<a href=""/people/{{id}}"">{{name}}</a>";

        var source = "{{> partial}}";

        using (var handleBars = new Handlebars())
        {
          //  handleBars.RegisterPartial("link", partial);
            handleBars.RegisterTemplate("myTemplate", source);
            Approvals.Verify(handleBars.Transform("myTemplate", null));
        }
    }

}