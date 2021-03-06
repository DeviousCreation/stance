﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using FluentValidation;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.SystemAlertAggregate;

namespace Initium.Portal.Domain.CommandValidators.SystemAlertAggregate
{
    public class UpdateSystemAlertCommandValidator : AbstractValidator<UpdateSystemAlertCommand>
    {
        public UpdateSystemAlertCommandValidator()
        {
            this.RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);

            this.RuleFor(x => x.Message)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);

            this.RuleFor(x => x.SystemAlertId)
                .Cascade(CascadeMode.Stop)
                .NotEqual(Guid.Empty).WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}