﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation;

namespace Initium.Portal.Web.Controllers.Api.AuthApp.Models
{
    public class RevokeAuthAppRequest
    {
        public string Password { get; set; }

        public class Validator : AbstractValidator<RevokeAuthAppRequest>
        {
            public Validator()
            {
                this.RuleFor(x => x.Password)
                    .NotEmpty();
            }
        }
    }
}