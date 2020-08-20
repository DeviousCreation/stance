﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Initium.Portal.Core;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Queries;
using Initium.Portal.Queries.Entities;
using MaybeMonad;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Queries
{
    public class UserQueriesTests
    {
        [Fact]
        public async Task CheckForPresenceOfAnyUser_GivenUserDoesExist_ExpectPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);
            await context.Users.AddAsync(new User
            {
                Id = TestVariables.UserId,
            });
            await context.SaveChangesAsync();

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.CheckForPresenceOfAnyUser();
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfAnyUser_GivenUserDoesNotExist_ExpectNotPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.CheckForPresenceOfAnyUser();
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task GetProfileForCurrentUser_GivenDataIsFound_ExpectMaybeWithMappedData()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var systemUser = new Mock<ISystemUser>();
            systemUser.Setup(x => x.UserId).Returns(TestVariables.UserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe.From(systemUser.Object));
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);
            await context.Users.AddAsync(new User
            {
                Id = TestVariables.UserId,
                FirstName = "first-name",
                LastName = "last-name",
            });
            await context.SaveChangesAsync();

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.GetProfileForCurrentUser();
            Assert.True(result.HasValue);
            Assert.Equal("first-name", result.Value.FirstName);
            Assert.Equal("last-name", result.Value.LastName);
        }

        [Fact]
        public async Task GetProfileForCurrentUser_GivenDataIsNotFound_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var systemUser = new Mock<ISystemUser>();
            systemUser.Setup(x => x.UserId).Returns(TestVariables.UserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe.From(systemUser.Object));
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.GetProfileForCurrentUser();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetProfileForCurrentUser_GivenNoUserIsAuthenticated_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);

            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.GetProfileForCurrentUser();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task CheckForPresenceOfUserByEmailAddress_GivenUserDoesExist_ExpectPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);
            await context.Users.AddAsync(new User
            {
                EmailAddress = "email-address",
            });
            await context.SaveChangesAsync();
            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.CheckForPresenceOfUserByEmailAddress("email-address");
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfUserByEmailAddress_GivenUserDoesNotExist_ExpectNotPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);
            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.CheckForPresenceOfUserByEmailAddress("email-address");
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task GetDetailsOfUserById_GivenNoDataIsFound_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);
            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.GetDetailsOfUserById(TestVariables.UserId);
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetDetailsOfUserById_GivenDataIsFound_ExpectMaybeWithMappedData()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);
            await context.Users.AddAsync(new User
            {
                Id = TestVariables.UserId,
                EmailAddress = "email-address",
                FirstName = "first-name",
                LastName = "last-name",
                IsLockable = true,
                WhenCreated = TestVariables.Now.AddDays(-10),
                WhenLastAuthenticated = TestVariables.Now.AddDays(-1),
                WhenLocked = null,
                IsAdmin = false,
                WhenDisabled = TestVariables.Now,
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = new Role
                        {
                            RoleResources = new List<RoleResource>
                            {
                                new RoleResource
                                {
                                    ResourceId = TestVariables.ResourceId,
                                },
                            },
                        },
                    },
                },
            });
            await context.SaveChangesAsync();

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.GetDetailsOfUserById(TestVariables.UserId);
            Assert.True(result.HasValue);
            Assert.Equal(TestVariables.UserId, result.Value.UserId);
            Assert.Equal("email-address", result.Value.EmailAddress);
            Assert.Equal("first-name", result.Value.FirstName);
            Assert.Equal("last-name", result.Value.LastName);
            Assert.True(result.Value.IsLockable);
            Assert.Equal(TestVariables.Now.AddDays(-10), result.Value.WhenCreated);
            Assert.Equal(TestVariables.Now.AddDays(-1), result.Value.WhenLastAuthenticated);
            Assert.Null(result.Value.WhenLocked);
            Assert.False(result.Value.IsAdmin);
            Assert.Equal(TestVariables.Now, result.Value.WhenDisabled);
            var resource = Assert.Single(result.Value.Resources);
            Assert.Equal(TestVariables.ResourceId, resource);
        }

        [Fact]
        public async Task GetSystemProfileByUserId_GivenNoDataIsFound_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.GetSystemProfileByUserId(TestVariables.UserId);
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetSystemProfileByUserId_GivenDataIsFound_ExpectMaybeWithMappedData()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);
            await context.Users.AddAsync(new User
            {
                Id = TestVariables.UserId,
                EmailAddress = "email-address",
                FirstName = "first-name",
                LastName = "last-name",
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = new Role
                        {
                            RoleResources = new List<RoleResource>
                            {
                                new RoleResource
                                {
                                    Resource = new Resource
                                    {
                                        NormalizedName = "normalized-name",
                                    },
                                },
                            },
                        },
                    },
                },
            });
            await context.SaveChangesAsync();

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.GetSystemProfileByUserId(TestVariables.UserId);
            Assert.True(result.HasValue);
            Assert.Equal("email-address", result.Value.EmailAddress);
            Assert.Equal("first-name", result.Value.FirstName);
            Assert.Equal("last-name", result.Value.LastName);
            Assert.False(result.Value.IsAdmin);
            var resource = Assert.Single(result.Value.Resources);
            Assert.Equal("normalized-name", resource);
        }

        [Fact]
        public async Task CheckForPresenceOfAuthAppForCurrentUser_GivenNoUserIsAuthenticated_ExpectNotPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);
            await context.Users.AddAsync(new User
            {
                Id = TestVariables.UserId,
            });
            await context.SaveChangesAsync();

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.CheckForPresenceOfAuthAppForCurrentUser();
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfAuthAppForCurrentUser_GivenUserDoesExist_ExpectPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var systemUser = new Mock<ISystemUser>();
            systemUser.Setup(x => x.UserId).Returns(TestVariables.UserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe.From(systemUser.Object));
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.CheckForPresenceOfAuthAppForCurrentUser();
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfAuthAppForCurrentUser_GivenUserDoesNotHaveApp_ExpectNotPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var systemUser = new Mock<ISystemUser>();
            systemUser.Setup(x => x.UserId).Returns(TestVariables.UserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe.From(systemUser.Object));
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);
            await context.Users.AddAsync(new User
            {
                Id = TestVariables.UserId,
            });
            await context.SaveChangesAsync();

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.CheckForPresenceOfAuthAppForCurrentUser();
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfAuthAppForCurrentUser_GivenUserDoesApp_ExpectPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var systemUser = new Mock<ISystemUser>();
            systemUser.Setup(x => x.UserId).Returns(TestVariables.UserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe.From(systemUser.Object));
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);
            await context.Users.AddAsync(new User
            {
                Id = TestVariables.UserId,
                AuthenticatorApps = new List<AuthenticatorApp>
                {
                    new AuthenticatorApp
                    {
                        Id = TestVariables.AuthenticatorAppId,
                    },
                },
            });
            await context.SaveChangesAsync();
            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.CheckForPresenceOfAuthAppForCurrentUser();
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task GetDeviceInfoForCurrentUser_GivenNoUserIsAuthenticated_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.GetDeviceInfoForCurrentUser();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetDeviceInfoForCurrentUser_GivenUserDoesExist_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);
            var systemUser = new Mock<ISystemUser>();
            systemUser.Setup(x => x.UserId).Returns(TestVariables.UserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe.From(systemUser.Object));

            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.GetDeviceInfoForCurrentUser();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetDeviceInfoForCurrentUser_GivenUserHasNoDevices_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);
            var systemUser = new Mock<ISystemUser>();
            systemUser.Setup(x => x.UserId).Returns(TestVariables.UserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe.From(systemUser.Object));

            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);
            await context.Users.AddAsync(new User
            {
                Id = TestVariables.UserId,
            });
            await context.SaveChangesAsync();

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.GetDeviceInfoForCurrentUser();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetDeviceInfoForCurrentUser_GivenUserHasDevices_ExpectMaybeWithMappedData()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);
            var systemUser = new Mock<ISystemUser>();
            systemUser.Setup(x => x.UserId).Returns(TestVariables.UserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe.From(systemUser.Object));

            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);
            await context.Users.AddAsync(new User
            {
                Id = TestVariables.UserId,
                AuthenticatorDevices = new List<AuthenticatorDevice>
                {
                    new AuthenticatorDevice
                    {
                        Id = TestVariables.AuthenticatorDeviceId,
                        Name = "name",
                        WhenEnrolled = TestVariables.Now.AddDays(-10),
                        WhenLastUsed = TestVariables.Now.AddDays(-3),
                    },
                },
            });
            await context.SaveChangesAsync();

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context);
            var result = await userQueries.GetDeviceInfoForCurrentUser();
            Assert.True(result.HasValue);
            var device = Assert.Single(result.Value);
            Assert.NotNull(device);
            Assert.Equal(TestVariables.AuthenticatorDeviceId, device.Id);
            Assert.Equal("name", device.Name);
            Assert.Equal(TestVariables.Now.AddDays(-10), device.WhenEnrolled);
            Assert.Equal(TestVariables.Now.AddDays(-3), device.WhenLastUsed);
        }
    }
}