// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.Constants;
using Stance.Web.Infrastructure.Contracts;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.Auth
{
    [Authorize(AuthenticationSchemes = "login-partial")]
    public class ValidateEmailMfaCode : PrgPageModel<ValidateEmailMfaCode.Model>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMediator _mediator;

        public ValidateEmailMfaCode(IMediator mediator, IAuthenticationService authenticationService)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._authenticationService =
                authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task<IActionResult> OnPost()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result =
                await this._mediator.Send(new ValidateEmailMfaCodeAgainstCurrentUserCommand(this.PageModel.Code));
            if (result.IsSuccess)
            {
                var returnUrl = await this._authenticationService.SignInUserFromPartialStateAsync(result.Value.UserId);

                if (string.IsNullOrEmpty(returnUrl))
                {
                    return this.RedirectToPage(PageLocations.AppDashboard);
                }

                return this.LocalRedirect(returnUrl);
            }

            this.PrgState = PrgState.InError;
            return this.RedirectToPage();
        }

        public class Model
        {
            public string Code { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.Code).NotEmpty();
            }
        }
    }
}