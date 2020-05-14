﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Web.Controllers.Api.User.Models;
using Xunit;

namespace Initium.Portal.Tests.Web.Controllers.Api.User.Models
{
    public class DisableAccountRequestValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var request = new DisableAccountRequest
            {
                UserId = TestVariables.UserId,
            };
            var validator = new DisableAccountRequest.Validator();
            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
        {
            var request = new DisableAccountRequest
            {
                UserId = Guid.Empty,
            };
            var validator = new DisableAccountRequest.Validator();
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.PropertyName == "UserId");
        }
    }
}