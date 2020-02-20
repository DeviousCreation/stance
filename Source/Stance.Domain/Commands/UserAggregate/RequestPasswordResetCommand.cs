﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MediatR;
using ResultMonad;
using Stance.Core.Domain;

namespace Stance.Domain.Commands.UserAggregate
{
    public sealed class RequestPasswordResetCommand : IRequest<ResultWithError<ErrorData>>
    {
        public RequestPasswordResetCommand(string emailAddress)
        {
            this.EmailAddress = emailAddress;
        }

        public string EmailAddress { get; }
    }
}