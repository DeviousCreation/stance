// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stance.Domain.Commands.UserAggregate;
using Stance.Queries.Contracts;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.App.Profile
{
    [Authorize]
    public class Details : PrgPageModel<Details.Model>
    {
        private readonly IMediator _mediator;
        private readonly IUserQueries _userQueries;

        public Details(IMediator mediator, IUserQueries userQueries)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (this.PageModel == null)
            {
                var profileMaybe = await this._userQueries.GetProfileForCurrentUser();
                if (profileMaybe.HasNoValue)
                {
                    return this.NotFound();
                }

                this.PageModel = new Model
                {
                    FirstName = profileMaybe.Value.FirstName,
                    LastName = profileMaybe.Value.LastName,
                };
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result =
                await this._mediator.Send(new UpdateProfileCommand(this.PageModel.FirstName, this.PageModel.LastName));

            this.PrgState = result.IsFailure ? PrgState.Failed : PrgState.Success;

            return this.RedirectToPage();
        }

        public class Model
        {
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
        }
    }
}