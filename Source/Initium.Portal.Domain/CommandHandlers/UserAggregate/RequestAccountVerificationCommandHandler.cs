﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Settings;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.Events.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class
        RequestAccountVerificationCommandHandler : IRequestHandler<RequestAccountVerificationCommand,
            ResultWithError<ErrorData>>
    {
        private readonly IClock _clock;
        private readonly SecuritySettings _securitySettings;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RequestAccountVerificationCommandHandler> _logger;

        public RequestAccountVerificationCommandHandler(
            IUserRepository userRepository,
            IOptions<SecuritySettings> securitySettings, IClock clock, ILogger<RequestAccountVerificationCommandHandler> logger)
        {
            this._userRepository = userRepository;
            this._clock = clock;
            this._logger = logger;
            this._securitySettings = securitySettings.Value;
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            RequestAccountVerificationCommand request,
            CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult.IsSuccess)
            {
                return result;
            }

            this._logger.LogDebug("Failed saving changes.");
            return ResultWithError.Fail(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private async Task<ResultWithError<ErrorData>> Process(
            RequestAccountVerificationCommand request,
            CancellationToken cancellationToken)
        {
            var userMaybe = await this._userRepository.FindByEmailAddress(request.EmailAddress, cancellationToken);
            if (userMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            if (user.IsVerified)
            {
                this._logger.LogDebug("User already verified.");
                return ResultWithError.Fail(new ErrorData(
                    ErrorCodes.UserIsAlreadyVerified));
            }

            var token = user.GenerateNewAccountConfirmationToken(
                this._clock.GetCurrentInstant().ToDateTimeUtc(),
                TimeSpan.FromHours(this._securitySettings.AccountVerificationTokenLifetime));
            user.AddIntegrationEvent(new AccountConfirmationTokenGeneratedIntegrationEvent(user.EmailAddress, user.Profile.FirstName, user.Profile.LastName, token.Id, token.WhenExpires));

            this._userRepository.Update(user);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}