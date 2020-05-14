﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Queries.Contracts.Static;
using MediatR;
using NodaTime;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public sealed class
        CreateInitialUserCommandHandler : IRequestHandler<CreateInitialUserCommand, ResultWithError<ErrorData>>
    {
        private readonly IClock _clock;
        private readonly IUserQueries _userQueries;
        private readonly IUserRepository _userRepository;

        public CreateInitialUserCommandHandler(IUserRepository userRepository, IClock clock, IUserQueries userQueries)
        {
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this._userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            CreateInitialUserCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                return ResultWithError.Fail(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
        }

        private async Task<ResultWithError<ErrorData>> Process(
            CreateInitialUserCommand request)
        {
            var statusCheck = await this._userQueries.CheckForPresenceOfAnyUser();

            if (statusCheck.IsPresent)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.SystemIsAlreadySetup));
            }

            var whenHappened = this._clock.GetCurrentInstant().ToDateTimeUtc();

            var user = new User(Guid.NewGuid(), request.EmailAddress, BCrypt.Net.BCrypt.HashPassword(request.Password),
                true, whenHappened,
                request.FirstName, request.LastName, new List<Guid>(), true);

            user.VerifyAccount(whenHappened);

            this._userRepository.Add(user);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}