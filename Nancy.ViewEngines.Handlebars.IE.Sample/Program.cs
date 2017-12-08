using System;
using System.Diagnostics;
using Nancy.Hosting.Self;

public static class Program
{
    public static void Main()
    {
        using (var nancyHost = new NancyHost(new Uri("http://localhost:8888/")))
        {
            nancyHost.Start();

            Console.WriteLine("Nancy now listening - navigating to http://localhost:8888/. Press enter to stop");
            Process.Start("http://localhost:8888/");

            Console.ReadKey();
        }
    }
}