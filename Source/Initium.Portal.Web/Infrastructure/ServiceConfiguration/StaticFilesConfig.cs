﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using CompressedStaticFiles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class StaticFilesConfig
    {
        public static IApplicationBuilder UseCustomizedStaticFiles(this IApplicationBuilder app)
        {
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";

            app.UseCompressedStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = provider,
            });

            return app;
        }
    }
}