﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MaybeMonad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static.Models.Role;
using Stance.Web.Pages.App.UserManagement.Roles;
using Xunit;

namespace Stance.Tests.Web.Pages.App.UserManagement.Roles
{
    public class ViewRoleTests
    {
        [Fact]
        public async Task OnGetAsync_GivenRoleIsNotInSystem_ExpectNotFoundResultReturned()
        {
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.GetDetailsOfRoleById(It.IsAny<Guid>()))
                .ReturnsAsync(() => Maybe<DetailedRoleModel>.Nothing);

            var page = new ViewRole(roleQueries.Object);

            var result = await page.OnGetAsync();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_GivenRoleIsInSystem_ExpectDataToBeSetAndPageResultReturned()
        {
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.GetDetailsOfRoleById(It.IsAny<Guid>()))
                .ReturnsAsync(() => Maybe.From(new DetailedRoleModel(Guid.Empty, string.Empty, new List<Guid>())));

            var page = new ViewRole(roleQueries.Object);

            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.NotNull(page.Role);
        }
    }
}