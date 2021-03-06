﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Core.Extensions
{
    public static class MediatorExtensions
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any()).ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            var tasks = domainEvents
                .Select(async domainEvent => { await mediator.Publish(domainEvent); });

            await Task.WhenAll(tasks);
        }

        public static async Task DispatchIntegrationEventsAsync(this IMediator mediator, DbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.IntegrationEvents != null && x.Entity.IntegrationEvents.Any()).ToList();

            var integrationEvents = domainEntities
                .SelectMany(x => x.Entity.IntegrationEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearIntegrationEvents());

            var tasks = integrationEvents
                .Select(async integrationEvent => { await mediator.Publish(integrationEvent); });

            await Task.WhenAll(tasks);
        }
    }
}