﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Common.Domain.AggregatesModel.TenantAggregate;
using Initium.Portal.Infrastructure.Admin.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Infrastructure.Admin.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomizedDataEntityFramework(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddEntityFrameworkSqlServer()
                .AddDbContext<ManagementDataContext>();

            serviceCollection.AddScoped<ICoreDataContext>(provider =>
                provider.GetRequiredService<ManagementDataContext>());
            serviceCollection.AddScoped<IManagementDataContext>(provider =>
                provider.GetRequiredService<ManagementDataContext>());

            return serviceCollection;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITenantRepository, TenantRepository>();
            return serviceCollection;
        }
    }
}