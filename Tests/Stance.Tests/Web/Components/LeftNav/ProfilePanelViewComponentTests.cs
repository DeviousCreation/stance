﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using MaybeMonad;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using Stance.Core;
using Stance.Core.Contracts;
using Stance.Web.Components.ProfilePanel;
using Xunit;

namespace Stance.Tests.Web.Components.LeftNav
{
    public class ProfilePanelViewComponentTests
    {
        [Fact]
        public void Invoke_GivenUserIsAuthenticated_ExpectViewWithPopulatedModel()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(
                Maybe.From(
                    new AuthenticatedUser(Guid.Empty, new string('*', 3), new string('*', 4), new string('*', 5))));

            var httpContext = new DefaultHttpContext();
            var viewContext = new ViewContext { HttpContext = httpContext };
            var viewComponentContext = new ViewComponentContext { ViewContext = viewContext };

            var component = new ProfilePanelViewComponent(currentAuthenticatedUserProvider.Object)
            {
                ViewComponentContext = viewComponentContext,
            };

            var result = component.Invoke();
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            var model = Assert.IsType<ProfilePanelViewModel>(viewResult.ViewData.Model);
            Assert.Equal(new string('*', 3), model.EmailAddress);
            Assert.Equal($"{new string('*', 4)} {new string('*', 5)}", model.Name);
        }

        [Fact]
        public void Invoke_GivenUserIsNotAuthenticated_ExpectViewWithEmptyModel()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(
                Maybe<AuthenticatedUser>.Nothing);

            var httpContext = new DefaultHttpContext();
            var viewContext = new ViewContext { HttpContext = httpContext };
            var viewComponentContext = new ViewComponentContext { ViewContext = viewContext };

            var component = new ProfilePanelViewComponent(currentAuthenticatedUserProvider.Object)
            {
                ViewComponentContext = viewComponentContext,
            };

            var result = component.Invoke();
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            var model = Assert.IsType<ProfilePanelViewModel>(viewResult.ViewData.Model);
            Assert.Equal(string.Empty, model.EmailAddress);
            Assert.Equal(string.Empty, model.Name);
        }
    }
}