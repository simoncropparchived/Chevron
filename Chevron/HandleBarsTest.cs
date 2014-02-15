using System.Collections.Generic;
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

            var model = new Person
            {
                name = "Alan",
                hometown = "Somewhere, TX",
                kids = new List<Person>
                {
                    new Person
                    {
                        name = "Sally", 
                        age = "4"
                    }
                }
            };
            ApprovalTests.Approvals.Verify(handleBars.Transform(templateContent, model));
        }
    }

    public class Person
    {
        public string name;
        public string hometown;
        public List<Person> kids;
        public string age;
    }
}