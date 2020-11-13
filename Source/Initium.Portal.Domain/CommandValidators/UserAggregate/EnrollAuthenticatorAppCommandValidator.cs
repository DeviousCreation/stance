﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;

namespace Initium.Portal.Domain.CommandValidators.UserAggregate
{
    public class EnrollAuthenticatorAppCommandValidator : AbstractValidator<EnrollAuthenticatorAppCommand>
    {
        public EnrollAuthenticatorAppCommandValidator()
        {
            this.RuleFor(x => x.Key)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);

            this.RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}