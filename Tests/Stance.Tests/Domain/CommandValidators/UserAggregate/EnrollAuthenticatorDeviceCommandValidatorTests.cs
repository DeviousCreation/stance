﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;
using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.CommandValidators.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandValidators.UserAggregate
{
    public class EnrollAuthenticatorDeviceCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new EnrollAuthenticatorDeviceCommand("name", new AuthenticatorAttestationRawResponse(), new CredentialCreateOptions());
            var validator = new EnrollAuthenticatorDeviceCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenNameIsNull_ExpectValidationFailure()
        {
            var cmd = new EnrollAuthenticatorDeviceCommand(null, new AuthenticatorAttestationRawResponse(), new CredentialCreateOptions());
            var validator = new EnrollAuthenticatorDeviceCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Name");
        }

        [Fact]
        public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
        {
            var cmd = new EnrollAuthenticatorDeviceCommand(string.Empty, new AuthenticatorAttestationRawResponse(), new CredentialCreateOptions());
            var validator = new EnrollAuthenticatorDeviceCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Name");
        }

        [Fact]
        public void Validate_GivenAuthenticatorAttestationRawResponseIsNull_ExpectValidationFailure()
        {
            var cmd = new EnrollAuthenticatorDeviceCommand("name", null, new CredentialCreateOptions());
            var validator = new EnrollAuthenticatorDeviceCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "AuthenticatorAttestationRawResponse");
        }

        [Fact]
        public void Validate_GivenCredentialCreateOptionsIsNull_ExpectValidationFailure()
        {
            var cmd = new EnrollAuthenticatorDeviceCommand("name", new AuthenticatorAttestationRawResponse(), null);
            var validator = new EnrollAuthenticatorDeviceCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "CredentialCreateOptions");
        }
    }
}