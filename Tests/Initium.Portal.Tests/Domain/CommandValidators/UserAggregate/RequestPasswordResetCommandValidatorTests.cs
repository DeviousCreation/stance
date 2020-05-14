﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.CommandValidators.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandValidators.UserAggregate
{
    public class RequestPasswordResetCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new RequestPasswordResetCommand("a@b.com");
            var validator = new RequestPasswordResetCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenEmailAddressIsEmpty_ExpectValidationFailure()
        {
            var cmd = new RequestPasswordResetCommand(string.Empty);
            var validator = new RequestPasswordResetCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenEmailAddressIsNull_ExpectValidationFailure()
        {
            var cmd = new RequestPasswordResetCommand(null);
            var validator = new RequestPasswordResetCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenEmailAddressIsNotAnEmailAddress_ExpectValidationFailure()
        {
            var cmd = new RequestPasswordResetCommand("email-address");
            var validator = new RequestPasswordResetCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.ValueMustBeAnEmailAddress) &&
                           failure.PropertyName == "EmailAddress");
        }
    }
}