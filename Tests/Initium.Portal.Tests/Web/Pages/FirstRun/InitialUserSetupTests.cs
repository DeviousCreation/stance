﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Pages.FirstRun;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.FirstRun
{
    public class InitialUserSetupTests
    {
        [Fact]
        public async Task OnGet_GivenNoUserInSystem_ExpectPageResult()
        {
            var userQueries = new Mock<IUserQueryService>();
            userQueries.Setup(x => x.CheckForPresenceOfAnyUser())
                .ReturnsAsync(new StatusCheckModel(false));
            var mediator = new Mock<IMediator>();

            var page = new InitialUserSetup(userQueries.Object, mediator.Object);
            var tempDataDictionary = new Mock<ITempDataDictionary>();
            page.TempData = tempDataDictionary.Object;

            var result = await page.OnGet();
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnGet_GivenUserInSystem_ExpectNotFoundResult()
        {
            var userQueries = new Mock<IUserQueryService>();
            userQueries.Setup(x => x.CheckForPresenceOfAnyUser())
                .ReturnsAsync(new StatusCheckModel(true));
            var mediator = new Mock<IMediator>();

            var page = new InitialUserSetup(userQueries.Object, mediator.Object);

            var result = await page.OnGet();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnPost_GivenCommandFailure_ExpectRedirectToPageResultToSamePageAndPrgStateSet()
        {
            var userQueries = new Mock<IUserQueryService>();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail<CreateUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.SavingChanges)));

            var page = new InitialUserSetup(userQueries.Object, mediator.Object)
            {
                PageModel = new InitialUserSetup.Model(),
            };

            var result = await page.OnPost();
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Failed, page.PrgState);
        }

        [Fact]
        public async Task OnPost_GivenCommandSuccess_ExpectRedirectToPageResult()
        {
            var userQueries = new Mock<IUserQueryService>();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok<CreateUserCommandResult, ErrorData>(new CreateUserCommandResult(TestVariables.UserId)));

            var page = new InitialUserSetup(userQueries.Object, mediator.Object)
            {
                PageModel = new InitialUserSetup.Model(),
            };

            var result = await page.OnPost();
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(CorePageLocations.FirstRunSetupCompleted, redirectToPageResult.PageName);
        }

        [Fact]
        public async Task OnPost_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var userQueries = new Mock<IUserQueryService>();
            var mediator = new Mock<IMediator>();

            var page = new InitialUserSetup(userQueries.Object, mediator.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPost();
            Assert.IsType<RedirectToPageResult>(result);
        }

        public class Validator
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var model = new InitialUserSetup.Model
                {
                    EmailAddress = "a@b.com",
                    FirstName = "first-name",
                    LastName = "last-name",
                };
                var validator = new InitialUserSetup.Validator();
                var result = validator.Validate(model);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenEmailAddressIsEmpty_ExpectValidationFailure()
            {
                var model = new InitialUserSetup.Model
                {
                    EmailAddress = string.Empty,
                    FirstName = "first-name",
                    LastName = "last-name",
                };
                var validator = new InitialUserSetup.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }

            [Fact]
            public void Validate_GivenEmailAddressIsNull_ExpectValidationFailure()
            {
                var model = new InitialUserSetup.Model
                {
                    EmailAddress = null,
                    FirstName = "first-name",
                    LastName = "last-name",
                };
                var validator = new InitialUserSetup.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }

            [Fact]
            public void Validate_GivenEmailAddressIsNotValidEmailAddress_ExpectValidationFailure()
            {
                var model = new InitialUserSetup.Model
                {
                    EmailAddress = "email-address",
                    FirstName = "first-name",
                    LastName = "last-name",
                };
                var validator = new InitialUserSetup.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }

            [Fact]
            public void Validate_GivenFirstNameIsEmpty_ExpectValidationFailure()
            {
                var model = new InitialUserSetup.Model
                {
                    EmailAddress = string.Empty,
                    FirstName = string.Empty,
                    LastName = "last-name",
                };
                var validator = new InitialUserSetup.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "FirstName");
            }

            [Fact]
            public void Validate_GivenFirstNameIsNull_ExpectValidationFailure()
            {
                var model = new InitialUserSetup.Model
                {
                    EmailAddress = null,
                    FirstName = null,
                    LastName = "last-name",
                };
                var validator = new InitialUserSetup.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "FirstName");
            }

            [Fact]
            public void Validate_GivenLastNameIsEmpty_ExpectValidationFailure()
            {
                var model = new InitialUserSetup.Model
                {
                    EmailAddress = string.Empty,
                    FirstName = "first-name",
                    LastName = string.Empty,
                };
                var validator = new InitialUserSetup.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "LastName");
            }

            [Fact]
            public void Validate_GivenLastNameIsNull_ExpectValidationFailure()
            {
                var model = new InitialUserSetup.Model
                {
                    EmailAddress = null,
                    FirstName = "first-name",
                    LastName = null,
                };
                var validator = new InitialUserSetup.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "LastName");
            }
        }
    }
}