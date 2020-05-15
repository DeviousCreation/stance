﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.CommandValidators.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandValidators.UserAggregate
{
    public class MarkAllUnreadNotificationsAsViewedCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new MarkAllUnreadNotificationsAsViewedCommand(TestVariables.UserId);
            var validator = new MarkAllUnreadNotificationsAsViewedCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenUserIdIsEmpty_ExpectFail()
        {
            var cmd = new MarkAllUnreadNotificationsAsViewedCommand(Guid.Empty);
            var validator = new MarkAllUnreadNotificationsAsViewedCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors, x => x.ErrorCode == ValidationCodes.FieldIsRequired && x.PropertyName == "UserId");
        }
    }
}