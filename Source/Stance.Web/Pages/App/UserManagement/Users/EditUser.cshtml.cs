// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stance.Domain.Commands.UserAggregate;
using Stance.Queries.Contracts;
using Stance.Web.Infrastructure.Constants;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.App.UserManagement.Users
{
    public class EditUser : PrgPageModel<EditUser.Model>
    {
        private readonly IMediator _mediator;
        private readonly IUserQueries _userQueries;

        public EditUser(IUserQueries userQueries, IMediator mediator)
        {
            this._userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public string LockedStatus { get; private set; }

        public string AuthenticationStatus { get; private set; }

        public DateTime WhenCreated { get; private set; }

        public string Name { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userMaybe = await this._userQueries.GetDetailsOfUserById(this.Id);
            if (userMaybe.HasNoValue)
            {
                return this.NotFound();
            }

            var user = userMaybe.Value;
            if (this.PageModel == null)
            {
                this.PageModel = new Model
                {
                    EmailAddress = user.EmailAddress,
                    IsLockable = user.IsLockable,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Id = user.UserId,
                };
            }

            this.Name = $"{user.FirstName} {user.LastName}";
            this.WhenCreated = user.WhenCreated;

            this.AuthenticationStatus = user.WhenLastAuthenticated.HasValue
                ? user.WhenLastAuthenticated.ToString()
                : "Never Authenticated";

            this.LockedStatus = user.IsLockable
                ? user.WhenLocked.HasValue ? user.WhenLocked.ToString() : "Not Locked"
                : "User is not lockable";

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result = await this._mediator.Send(new UpdateUserCoreDetailsCommand(
                this.PageModel.Id,
                this.PageModel.EmailAddress,
                this.PageModel.FirstName,
                this.PageModel.LastName,
                this.PageModel.IsLockable));

            if (result.IsSuccess)
            {
                return this.RedirectToPage(PageLocations.UserView, new { id = this.PageModel.Id });
            }

            this.PrgState = PrgState.Failed;
            return this.RedirectToPage();
        }

        public class Model
        {
            [Display(Name = "Email Address")]
            public string EmailAddress { get; set; }

            [Display(Name = "Is Lockable")]
            public bool IsLockable { get; set; }

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.EmailAddress)
                    .NotEmpty()
                    .EmailAddress();
            }
        }
    }
}