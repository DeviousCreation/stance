﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Initium.Portal.Domain.AggregatesModel.TenantAggregate;
using Initium.Portal.Infrastructure;
using Initium.Portal.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Infrastructure.Repositories
{
    public class TenantRepositoryTests
    {
        [Fact]
        public void Add_GivenArgumentIsNotTenantType_ExpectException()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();
            var tenantInfo = new Mock<ITenantInfo>();

            using var context = new DataContext(options, mediator.Object, tenantInfo.Object);
            var repository = new TenantRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => repository.Add(new Mock<ITenant>().Object));
            Assert.Equal("tenant", exception.Message);
        }

        [Fact]
        public void Add_GivenArgumentIsTenantType_ExpectReturnedTenantToBeIdenticalAsArgument()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();
            var tenantInfo = new Mock<ITenantInfo>();

            using var context = new DataContext(options, mediator.Object, tenantInfo.Object);
            var repository = new TenantRepository(context);

            var tenant = new Tenant(TestVariables.TenantId, "identifier", "name", "connection-string");
            var returned = repository.Add(tenant);
            Assert.NotNull(returned);
            Assert.Equal(tenant, returned);
        }

        [Fact]
        public void Add_GivenArgumentIsTenantType_ExpectTenantToBeAddedToContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();
            var tenantInfo = new Mock<ITenantInfo>();

            using var context = new DataContext(options, mediator.Object, tenantInfo.Object);
            var repository = new TenantRepository(context);

            var tenant = new Tenant(TestVariables.TenantId, "identifier", "name", "connection-string");
            repository.Add(tenant);
            var inContext = context.ChangeTracker.Entries<Tenant>()
                .FirstOrDefault(x => x.Entity.Id == TestVariables.TenantId);
            Assert.NotNull(inContext);
            Assert.Equal(EntityState.Added, inContext.State);
        }

        [Fact]
        public void Update_GivenArgumentIsNotTenant_ExpectArgumentException()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();
            var tenantInfo = new Mock<ITenantInfo>();

            using var context = new DataContext(options, mediator.Object, tenantInfo.Object);
            var repository = new TenantRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => repository.Update(new Mock<ITenant>().Object));
            Assert.Equal("tenant", exception.Message);
        }

        [Fact]
        public void Update_GivenArgumentIsTenant_ExpectTenantToBeUpdatedInTheContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();
            var tenantInfo = new Mock<ITenantInfo>();

            using var context = new DataContext(options, mediator.Object, tenantInfo.Object);
            var repository = new TenantRepository(context);
            var tenant = new Tenant(TestVariables.TenantId, "identifier", "name", "connection-string");
            repository.Update(tenant);
            var inContext = context.ChangeTracker.Entries<Tenant>()
                .FirstOrDefault(x => x.Entity.Id == TestVariables.TenantId);
            Assert.NotNull(inContext);
            Assert.Equal(EntityState.Modified, inContext.State);
        }

        [Fact]
        public async Task Find_GivenSystemAlertDoesExist_ExpectMaybeWithData()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();
            var tenantInfo = new Mock<ITenantInfo>();

            await using var context = new DataContext(options, mediator.Object, tenantInfo.Object);
            await context.Tenants.AddAsync(
                new Tenant(TestVariables.TenantId, "identifier", "name", "connection-string"));
            await context.SaveChangesAsync();
            var repository = new TenantRepository(context);
            var maybe = await repository.Find(TestVariables.TenantId);
            Assert.True(maybe.HasValue);
        }

        [Fact]
        public async Task Find_GivenSystemAlertDoesNotExist_ExpectMaybeWithNoValue()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();
            var tenantInfo = new Mock<ITenantInfo>();

            await using var context = new DataContext(options, mediator.Object, tenantInfo.Object);
            var repository = new TenantRepository(context);
            var maybe = await repository.Find(Guid.Empty);
            Assert.True(maybe.HasNoValue);
        }
    }
}