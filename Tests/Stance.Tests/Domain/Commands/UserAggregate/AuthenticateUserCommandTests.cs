﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Stance.Domain.Commands.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.Commands.UserAggregate
{
    public class AuthenticateUserCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var command = new AuthenticateUserCommand("email-address", "password");
            Assert.Equal("email-address", command.EmailAddress);
            Assert.Equal("password", command.Password);
        }
    }
}