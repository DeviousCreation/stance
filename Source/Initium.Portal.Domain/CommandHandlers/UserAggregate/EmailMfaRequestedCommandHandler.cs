﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.Events;
using MediatR;
using NodaTime;
using OtpNet;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class EmailMfaRequestedCommandHandler : IRequestHandler<EmailMfaRequestedCommand, ResultWithError<ErrorData>>
    {
        private readonly IClock _clock;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IUserRepository _userRepository;

        public EmailMfaRequestedCommandHandler(
            IUserRepository userRepository, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider,
            IClock clock)
        {
            this._userRepository = userRepository;
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
            this._clock = clock;
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            EmailMfaRequestedCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                return ResultWithError.Fail(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
        }

        private async Task<ResultWithError<ErrorData>> Process(CancellationToken cancellationToken)
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe = await this._userRepository.Find(currentUserMaybe.Value.UserId, cancellationToken);

            if (userMaybe.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            var totp = new Totp(user.SecurityStamp.ToByteArray());

            var token = totp.ComputeTotp();

            user.AddDomainEvent(new EmailMfaTokenGeneratedEvent(
                user.EmailAddress,
                user.Profile.FirstName,
                user.Profile.LastName,
                token));

            user.ProcessPartialSuccessfulAuthenticationAttempt(
                this._clock.GetCurrentInstant().ToDateTimeUtc(),
                AuthenticationHistoryType.EmailMfaRequested);

            return ResultWithError.Ok<ErrorData>();
        }
    }
}