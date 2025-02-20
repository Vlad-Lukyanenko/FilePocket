using System.Diagnostics;
using FilePocket.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using tusdotnet;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;
using tusdotnet.Stores;

namespace FilePocket.WebApi.Middlewares.Upload.Tus;

public static class TusMiddlewareExtensions
{
    /// <summary>
    /// Registers the tusdotnet middleware using configured options.
    /// </summary>
    public static IApplicationBuilder UseTusDotNet(this IApplicationBuilder app)
    {
        var tusOptions = app.ApplicationServices.GetRequiredService<IOptions<TusConfigurationModel>>().Value;

        if (!tusOptions.Enabled) return app;

        // Retrieve the hosting environment to determine the web root path.
        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        
        var uploadPath = "F:\\opensource\\TusUploads";
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        app.UseTus(_ => new DefaultTusConfiguration
        {
            UrlPath = tusOptions.UrlPath,
            MaxAllowedUploadSizeInBytes = tusOptions.MaxAllowedUploadSizeMb * 1024 * 1024,
            // Events = new Events
            // {
            //     OnAuthorizeAsync = ctx =>
            //     {
            //         Debug.WriteLine("Tus Auth Request Received");
            //         var user = ctx.HttpContext.User;
            //
            //         if (user.Identity == null || !user.Identity.IsAuthenticated)
            //         {
            //             Debug.WriteLine("Unauthorized Request");
            //             ctx.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //             return Task.CompletedTask;
            //         }
            //
            //         Debug.WriteLine("Request Authorized");
            //         return Task.CompletedTask;
            //     },
            //     OnBeforeCreateAsync = ctx =>
            //     {
            //         if (!ctx.HttpContext.Request.Headers.TryGetValue("Upload-Length", out var value)) 
            //             return Task.FromResult(true);
            //         
            //         var uploadLength = long.Parse(value.ToString());
            //         if (uploadLength <= tusOptions.MaxAllowedUploadSizeMb * 1024 * 1024) 
            //             return Task.FromResult(true);
            //
            //         return Task.FromResult(false);
            //     }
            // },
            // AllowedExtensions = TusExtensions.All,
            Store = new TusDiskStore(uploadPath)
        });

        return app;
    }
}