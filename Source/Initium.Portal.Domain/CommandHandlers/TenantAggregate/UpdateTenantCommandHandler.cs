﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.TenantAggregate;
using Initium.Portal.Domain.Commands.TenantAggregate;
using Initium.Portal.Queries.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.TenantAggregate
{
    public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, ResultWithError<ErrorData>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ILogger _logger;
        private readonly ITenantQueryService _tenantQueryService;

        public UpdateTenantCommandHandler(ITenantRepository tenantRepository, ILogger<CreateTenantCommandHandler> logger, ITenantQueryService tenantQueryService)
        {
            this._tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tenantQueryService = tenantQueryService ?? throw new ArgumentNullException(nameof(tenantQueryService));
        }

        public async Task<ResultWithError<ErrorData>> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._tenantRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult)
            {
                return result;
            }

            this._logger.LogDebug("Failed saving changes.");
            return ResultWithError.Fail(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private async Task<ResultWithError<ErrorData>> Process(UpdateTenantCommand request, CancellationToken cancellationToken)
        {
            var tenantMaybe = await this._tenantRepository.Find(request.TenantId, cancellationToken);
            if (tenantMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.RoleNotFound));
            }

            var tenant = tenantMaybe.Value;

            if (!string.Equals(tenant.Identifier, request.Identifier, StringComparison.InvariantCultureIgnoreCase))
            {
                var presenceResult =
                    await this._tenantQueryService
                        .CheckForPresenceOfTenantByIdentifier(request.Identifier, cancellationToken);
                if (presenceResult.IsPresent)
                {
                    this._logger.LogDebug("Failed presence check.");
                    return ResultWithError.Fail(new ErrorData(ErrorCodes.RoleAlreadyExists));
                }
            }

            tenant.UpdateDetails(request.Identifier, tenant.Name, tenant.ConnectionString);
            tenant.SetTenantFeatures(request.Features);

            this._tenantRepository.Update(tenant);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}