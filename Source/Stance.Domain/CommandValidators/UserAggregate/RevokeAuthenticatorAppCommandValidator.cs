﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation;
using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandValidators.UserAggregate
{
    public sealed class RevokeAuthenticatorAppCommandValidator : AbstractValidator<RevokeAuthenticatorAppCommand>
    {
        public RevokeAuthenticatorAppCommandValidator()
        {
            this.RuleFor(x => x.Password)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}