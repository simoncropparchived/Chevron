using ApprovalTests;
using Chevron;
using NUnit.Framework;

[TestFixture]
public class HandleBarsTest
{
    [Test]
    public void Sample()
    {
        var templateContent = @"<p>Hello, my name is {{name}}. I am from {{hometown}}. I have ' +
             '{{kids.length}} kids:</p>' +
             '<ul>{{#kids}}<li>{{name}} is {{age}}</li>{{/kids}}</ul>";
        using (var handleBars = new HandleBars())
        {
            var context = new
            {
                name = "Alan",
                hometown = "Somewhere, TX",
                kids = new []
                {
                    new
                    {
                        name = "Sally",
                        age = "4"
                    }
                }
            };
            Approvals.Verify(handleBars.Transform(templateContent, context));
        }
    }

    [Test]
    public void RegisterHelperSample()
    {
        using (var handleBars = new HandleBars())
        {
            handleBars.RegisterHelper("link_to",
                @"function() {
  return new Handlebars.SafeString(""<a href='"" + this.url + ""'>"" + this.body + ""</a>"");
}");
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
            Approvals.Verify(handleBars.Transform(source, context));
        }
    }

}