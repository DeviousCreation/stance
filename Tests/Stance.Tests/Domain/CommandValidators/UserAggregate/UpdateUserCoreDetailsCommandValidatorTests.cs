﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.CommandValidators.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandValidators.UserAggregate
{
    public class UpdateUserCoreDetailsCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new UpdateUserCoreDetailsCommand(Guid.NewGuid(), "a@b.com", string.Empty, string.Empty, true);
            var validator = new UpdateUserCoreDetailsCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenEmailAddressIsEmpty_ExpectValidationFailure()
        {
            var cmd = new UpdateUserCoreDetailsCommand(Guid.NewGuid(), string.Empty, string.Empty, string.Empty, true);
            var validator = new UpdateUserCoreDetailsCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenEmailAddressIsNotValidEmailAddress_ExpectValidationFailure()
        {
            var cmd = new UpdateUserCoreDetailsCommand(Guid.NewGuid(), new string('*', 5),  string.Empty, string.Empty, true);
            var validator = new UpdateUserCoreDetailsCommandValidator();
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
            var cmd = new UpdateUserCoreDetailsCommand(Guid.NewGuid(), null, string.Empty, string.Empty, true);
            var validator = new UpdateUserCoreDetailsCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenUserIdIsEmpty_ExpectValidationFailure()
        {
            var cmd = new UpdateUserCoreDetailsCommand(Guid.Empty, "a@b.com", string.Empty, string.Empty, true);
            var validator = new UpdateUserCoreDetailsCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "UserId");
        }
    }
}