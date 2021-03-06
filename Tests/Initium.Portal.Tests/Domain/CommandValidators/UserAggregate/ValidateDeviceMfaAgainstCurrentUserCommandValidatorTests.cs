﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.CommandValidators.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandValidators.UserAggregate
{
    public class ValidateDeviceMfaAgainstCurrentUserCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new ValidateDeviceMfaAgainstCurrentUserCommand(new AuthenticatorAssertionRawResponse(), new AssertionOptions());
            var validator = new ValidateDeviceMfaAgainstCurrentUserCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenAuthenticatorAssertionRawResponseIsNull_ExpectValidationFailure()
        {
            var cmd = new ValidateDeviceMfaAgainstCurrentUserCommand(null, new AssertionOptions());
            var validator = new ValidateDeviceMfaAgainstCurrentUserCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "AuthenticatorAssertionRawResponse");
        }

        [Fact]
        public void Validate_GivenAssertionOptionsIsNull_ExpectValidationFailure()
        {
            var cmd = new ValidateDeviceMfaAgainstCurrentUserCommand(new AuthenticatorAssertionRawResponse(), null);
            var validator = new ValidateDeviceMfaAgainstCurrentUserCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "AssertionOptions");
        }
    }
}