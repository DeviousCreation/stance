﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.SystemAlertAggregate;

namespace Initium.Portal.Domain.CommandValidators.SystemAlertAggregate
{
    public class CreateNewSystemAlertCommandValidator : AbstractValidator<CreateNewSystemAlertCommand>
    {
        public CreateNewSystemAlertCommandValidator()
        {
            this.RuleFor(x => x.Message)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}