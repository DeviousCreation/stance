﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.CommandValidators.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandValidators.UserAggregate
{
    public class ChangePasswordCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new ChangePasswordCommand("current-password", "new-password");
            var validator = new ChangePasswordCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenAllCurrentPasswordIsNull_ExpectValidationFailure()
        {
            var cmd = new ChangePasswordCommand(null, "new-password");
            var validator = new ChangePasswordCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "CurrentPassword");
        }

        [Fact]
        public void Validate_GivenAllCurrentPasswordIsEmpty_ExpectValidationFailure()
        {
            var cmd = new ChangePasswordCommand(string.Empty, "new-password");
            var validator = new ChangePasswordCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "CurrentPassword");
        }

        [Fact]
        public void Validate_GivenAllNewPasswordIsNull_ExpectValidationFailure()
        {
            var cmd = new ChangePasswordCommand("current-password", null);
            var validator = new ChangePasswordCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "NewPassword");
        }

        [Fact]
        public void Validate_GivenAllNewPasswordIsEmpty_ExpectValidationFailure()
        {
            var cmd = new ChangePasswordCommand("current-password", string.Empty);
            var validator = new ChangePasswordCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "NewPassword");
        }
    }
}