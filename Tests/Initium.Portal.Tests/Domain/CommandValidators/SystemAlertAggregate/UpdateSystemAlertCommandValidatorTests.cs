﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.SystemAlertAggregate;
using Initium.Portal.Domain.CommandValidators.SystemAlertAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandValidators.SystemAlertAggregate
{
    public class UpdateSystemAlertCommandValidatorTests
    {
        [Fact]
        public void Validate_GiveMessageIsEmpty_ExpectValidationFailure()
        {
            var cmd = new UpdateSystemAlertCommand(
                TestVariables.SystemAlertId,
                "name",
                string.Empty,
                SystemAlertType.Critical,
                null,
                null);
            var validator = new UpdateSystemAlertCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Message");
        }

        [Fact]
        public void Validate_GiveNameIsEmpty_ExpectValidationFailure()
        {
            var cmd = new UpdateSystemAlertCommand(
                TestVariables.SystemAlertId,
                string.Empty,
                "message",
                SystemAlertType.Critical,
                null,
                null);
            var validator = new UpdateSystemAlertCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Name");
        }

        [Fact]
        public void Validate_GivenAllRequiredPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new UpdateSystemAlertCommand(
                TestVariables.SystemAlertId,
                "name",
                "message",
                SystemAlertType.Critical,
                null,
                null);
            var validator = new UpdateSystemAlertCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenMessageIsNull_ExpectValidationFailure()
        {
            var cmd = new UpdateSystemAlertCommand(
                TestVariables.SystemAlertId,
                "name",
                null,
                SystemAlertType.Critical,
                null,
                null);
            var validator = new UpdateSystemAlertCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Message");
        }

        [Fact]
        public void Validate_GivenNameIsNull_ExpectValidationFailure()
        {
            var cmd = new UpdateSystemAlertCommand(
                TestVariables.SystemAlertId,
                null,
                "message",
                SystemAlertType.Critical,
                null,
                null);
            var validator = new UpdateSystemAlertCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Name");
        }

        [Fact]
        public void Validate_GivenSystemAlertIdIsEmpty_ExpectValidationFailure()
        {
            var cmd = new UpdateSystemAlertCommand(
                Guid.Empty,
                "name",
                "message",
                SystemAlertType.Critical,
                null,
                null);
            var validator = new UpdateSystemAlertCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "SystemAlertId");
        }
    }
}