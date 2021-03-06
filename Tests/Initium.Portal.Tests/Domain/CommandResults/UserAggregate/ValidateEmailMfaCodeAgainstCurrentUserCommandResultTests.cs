﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Domain.CommandResults.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandResults.UserAggregate
{
    public class ValidateEmailMfaCodeAgainstCurrentUserCommandResultTests
    {
        [Fact]
        public void AuthenticatedConstructor_GiveValidArguments_PropertiesAreSet()
        {
            var commandResult = new ValidateEmailMfaCodeAgainstCurrentUserCommandResult(TestVariables.UserId);

            Assert.Equal(TestVariables.UserId, commandResult.UserId);
            Assert.Equal(BaseAuthenticationProcessCommandResult.AuthenticationState.Completed, commandResult.AuthenticationStatus);
        }

        [Fact]
        public void PartiallyAuthenticatedConstructor_GiveValidArguments_PropertiesAreSet()
        {
            var commandResult = new ValidateEmailMfaCodeAgainstCurrentUserCommandResult(TestVariables.UserId, BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaEmailCode);
            Assert.Equal(TestVariables.UserId, commandResult.UserId);
            Assert.Equal(BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaEmailCode, commandResult.AuthenticationStatus);
        }
    }
}