﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Stance.Domain.CommandResults.UserAggregate
{
    public class AuthenticateUserCommandResult : BaseAuthenticationProcessCommandResult
    {
        public AuthenticateUserCommandResult(Guid userId, string emailAddress)
            : base(userId, emailAddress)
        {
        }

        public AuthenticateUserCommandResult(Guid userId, string emailAddress, AuthenticationState authenticationStatus)
            : base(userId, emailAddress, authenticationStatus)
        {
        }
    }
}