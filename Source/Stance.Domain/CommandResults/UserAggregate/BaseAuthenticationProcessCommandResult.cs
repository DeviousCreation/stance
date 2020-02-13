﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Stance.Domain.CommandResults.UserAggregate
{
    public abstract class BaseAuthenticationProcessCommandResult
    {
        protected BaseAuthenticationProcessCommandResult(Guid userId, string emailAddress)
            : this(userId, emailAddress, AuthenticationState.Completed)
        {
        }

        protected BaseAuthenticationProcessCommandResult(Guid userId, string emailAddress,
            AuthenticationState authenticationStatus)
        {
            this.UserId = userId;
            this.EmailAddress = emailAddress;
            this.AuthenticationStatus = authenticationStatus;
        }

        public enum AuthenticationState
        {
            Completed,
            AwaitingMfaEmailCode,
        }

        public Guid UserId { get; }

        public string EmailAddress { get; }

        public AuthenticationState AuthenticationStatus { get; }
    }
}