﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Stance.Domain.AggregatesModel.RoleAggregate;
using Stance.Infrastructure;
using Stance.Infrastructure.Repositories;
using Xunit;

namespace Stance.Tests.Infrastructure.Repositories
{
    public class RoleRepositoryTests
    {
        [Fact]
        public void Add_GivenArgumentIsNotRole_ExpectArgumentException()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();

            using var context = new DataContext(options, mediator.Object);
            var roleRepository = new RoleRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => roleRepository.Add(new Mock<IRole>().Object));
            Assert.Equal("role", exception.Message);
        }

        [Fact]
        public void Add_GivenArgumentIsRole_ExpectRoleToBeAddedToTheContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();

            using var context = new DataContext(options, mediator.Object);
            var roleRepository = new RoleRepository(context);
            var role = new Role(Guid.NewGuid(), string.Empty, new List<Guid>());
            roleRepository.Add(role);
            var roleInContext = context.ChangeTracker.Entries<Role>().FirstOrDefault(x => x.Entity.Id == role.Id);
            Assert.NotNull(roleInContext);
            Assert.Equal(EntityState.Added, roleInContext.State);
        }

        [Fact]
        public void Delete_GivenArgumentIsNotRole_ExpectArgumentException()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();

            using var context = new DataContext(options, mediator.Object);
            var roleRepository = new RoleRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => roleRepository.Delete(new Mock<IRole>().Object));
            Assert.Equal("role", exception.Message);
        }

        [Fact]
        public void Delete_GivenArgumentIsRole_ExpectRoleToBeDeletedInTheContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();

            using var context = new DataContext(options, mediator.Object);
            var roleRepository = new RoleRepository(context);
            var role = new Role(Guid.NewGuid(), string.Empty, new List<Guid>());
            roleRepository.Delete(role);
            var roleInContext = context.ChangeTracker.Entries<Role>().FirstOrDefault(x => x.Entity.Id == role.Id);
            Assert.NotNull(roleInContext);
            Assert.Equal(EntityState.Deleted, roleInContext.State);
        }

        [Fact]
        public async Task Find_GivenUserDoesExist_ExpectMaybeWithUser()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();

            await using var context = new DataContext(options, mediator.Object);
            var role = new Role(Guid.NewGuid(), string.Empty, new List<Guid>());
            await context.Roles.AddAsync(role);
            await context.SaveEntitiesAsync();
            var roleRepository = new RoleRepository(context);
            var maybe = await roleRepository.Find(role.Id);
            Assert.True(maybe.HasValue);
        }

        [Fact]
        public async Task Find_GivenUserDoesNotExist_ExpectMaybeWithNoData()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();

            await using var context = new DataContext(options, mediator.Object);
            var roleRepository = new RoleRepository(context);
            var maybe = await roleRepository.Find(Guid.NewGuid());
            Assert.True(maybe.HasNoValue);
        }

        [Fact]
        public void Update_GivenArgumentIsNotRole_ExpectArgumentException()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();

            using var context = new DataContext(options, mediator.Object);
            var roleRepository = new RoleRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => roleRepository.Update(new Mock<IRole>().Object));
            Assert.Equal("role", exception.Message);
        }

        [Fact]
        public void Update_GivenArgumentIsRole_ExpectRoleToBeUpdatedInTheContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();

            using var context = new DataContext(options, mediator.Object);
            var roleRepository = new RoleRepository(context);
            var role = new Role(Guid.NewGuid(), string.Empty, new List<Guid>());
            roleRepository.Update(role);
            var roleInContext = context.ChangeTracker.Entries<Role>().FirstOrDefault(x => x.Entity.Id == role.Id);
            Assert.NotNull(roleInContext);
            Assert.Equal(EntityState.Modified, roleInContext.State);
        }
    }
}