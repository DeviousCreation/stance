﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.CommandValidators.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandValidators.UserAggregate
{
    public class RequestAccountVerificationCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new RequestAccountVerificationCommand("email@address.com");
            var validator = new RequestAccountVerificationCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenEmailAddressIsNotValid_ExpectValidationFailure()
        {
            var cmd = new RequestAccountVerificationCommand("email.address@com");
            var validator = new RequestAccountVerificationCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.ValueMustBeAnEmailAddress) &&
                           failure.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenEmailAddressIsNull_ExpectValidationFailure()
        {
            var cmd = new RequestAccountVerificationCommand(null);
            var validator = new RequestAccountVerificationCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenEmailAddressIsEmpty_ExpectValidationFailure()
        {
            var cmd = new RequestAccountVerificationCommand(string.Empty);
            var validator = new RequestAccountVerificationCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "EmailAddress");
        }
    }
}