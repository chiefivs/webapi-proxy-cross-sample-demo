using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Web;
using StandardProducts;

AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
CreateWebHostBuilder(args).Build().Run();

static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
    File.AppendAllText("unhandled.txt", $"\n###\n{DateTime.Now}\n{e.ExceptionObject}");
}

static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().UseUrls("http://*:8000/").UseNLog();