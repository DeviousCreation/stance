﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;

namespace Initium.Portal.Domain.CommandValidators.UserAggregate
{
    public class AuthenticateUserCommandValidator : AbstractValidator<AuthenticateUserCommand>
    {
        public AuthenticateUserCommandValidator()
        {
            this.RuleFor(x => x.EmailAddress)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired)
                .EmailAddress().WithErrorCode(ValidationCodes.ValueMustBeAnEmailAddress);
            this.RuleFor(x => x.Password)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}