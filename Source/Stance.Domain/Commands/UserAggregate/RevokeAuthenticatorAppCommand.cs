﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MediatR;
using ResultMonad;
using Stance.Core.Domain;

namespace Stance.Domain.Commands.UserAggregate
{
    public sealed class RevokeAuthenticatorAppCommand : IRequest<ResultWithError<ErrorData>>
    {
        public RevokeAuthenticatorAppCommand(string password)
        {
            this.Password = password;
        }

        public string Password { get; }
    }
}