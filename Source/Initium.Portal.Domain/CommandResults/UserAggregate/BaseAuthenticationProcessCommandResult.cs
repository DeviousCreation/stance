﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Domain.CommandResults.UserAggregate
{
    public abstract class BaseAuthenticationProcessCommandResult
    {
        protected BaseAuthenticationProcessCommandResult(
            Guid userId, AuthenticationState authenticationStatus)
        {
            this.UserId = userId;
            this.AuthenticationStatus = authenticationStatus;
        }

        public enum AuthenticationState
        {
            Unknown,
            Completed,
            AwaitingMfaEmailCode,
            AwaitingMfaAppCode,
            AwaitingMfaDeviceCode,
        }

        public Guid UserId { get; }

        public AuthenticationState AuthenticationStatus { get; }
    }
}