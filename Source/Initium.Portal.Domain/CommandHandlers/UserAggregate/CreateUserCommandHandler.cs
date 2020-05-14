﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Settings;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.Events;
using Initium.Portal.Queries.Contracts.Static;
using MediatR;
using Microsoft.Extensions.Options;
using NodaTime;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<CreateUserCommandResult, ErrorData>>
    {
        private readonly IClock _clock;
        private readonly IUserQueries _userQueries;
        private readonly IUserRepository _userRepository;
        private readonly SecuritySettings _securitySettings;

        public CreateUserCommandHandler(IUserRepository userRepository, IClock clock, IUserQueries userQueries, IOptions<SecuritySettings> securitySettings)
        {
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this._userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
            this._securitySettings = securitySettings.Value;
        }

        public async Task<Result<CreateUserCommandResult, ErrorData>> Handle(
            CreateUserCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                return Result.Fail<CreateUserCommandResult, ErrorData>(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
        }

        private static string GenerateRandomPassword()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var data = new byte[256];
                rng.GetBytes(data);
                return BCrypt.Net.BCrypt.HashPassword(Convert.ToBase64String(data));
            }
        }

        private async Task<Result<CreateUserCommandResult, ErrorData>> Process(
            CreateUserCommand request)
        {
            var statusCheck =
                await this._userQueries.CheckForPresenceOfUserByEmailAddress(request.EmailAddress);
            if (statusCheck.IsPresent)
            {
                return Result.Fail<CreateUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.UserAlreadyExists));
            }

            var whenHappened = this._clock.GetCurrentInstant().ToDateTimeUtc();
            var user = new User(Guid.NewGuid(), request.EmailAddress, GenerateRandomPassword(), request.IsLockable,
                whenHappened, request.FirstName, request.LastName, request.Roles, request.IsAdmin);
            var token = user.GenerateNewAccountConfirmationToken(whenHappened, TimeSpan.FromMinutes(this._securitySettings.AccountVerificationTokenLifetime));
            user.AddDomainEvent(new AccountConfirmationTokenGeneratedEvent(user.EmailAddress, user.Profile.FirstName, user.Profile.LastName, token));
            this._userRepository.Add(user);
            return Result.Ok<CreateUserCommandResult, ErrorData>(new CreateUserCommandResult(user.Id));
        }
    }
}