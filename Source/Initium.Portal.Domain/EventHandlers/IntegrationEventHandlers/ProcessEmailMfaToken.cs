﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.Events.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Initium.Portal.Domain.EventHandlers.IntegrationEventHandlers
{
    public class ProcessEmailMfaToken : INotificationHandler<EmailMfaTokenGeneratedIntegrationEvent>
    {
        private readonly ILogger<ProcessEmailMfaToken> _logger;
        private readonly FeatureBasedTenantInfo _tenantInfo;

        public ProcessEmailMfaToken(ILogger<ProcessEmailMfaToken> logger, FeatureBasedTenantInfo tenantInfo)
        {
            this._logger = logger;
            this._tenantInfo = tenantInfo;
        }

        public Task Handle(EmailMfaTokenGeneratedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            this._logger.LogInformation($"MFA Token for Tenant {this._tenantInfo.Name} with email {notification.EmailAddress} is {notification.Token}");

            return Task.FromResult(0);
        }
    }
}