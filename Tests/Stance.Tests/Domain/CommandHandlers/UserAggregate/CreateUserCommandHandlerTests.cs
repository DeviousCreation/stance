﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Moq;
using NodaTime;
using Stance.Core.Contracts.Domain;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandHandlers.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Stance.Queries.Contracts;
using Stance.Queries.Models;
using Xunit;

namespace Stance.Tests.Domain.CommandHandlers.UserAggregate
{
    public class CreateUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var clock = new Mock<IClock>();
            var userQueries = new Mock<IUserQueries>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            userQueries.Setup(x => x.CheckForPresenceOfUserByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new StatusCheckModel(true));

            var handler = new CreateUserCommandHandler(userRepository.Object, clock.Object, userQueries.Object);
            var cmd = new CreateUserCommand(new string('*', 5), new string('*', 6), new string('*', 7), false);
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var clock = new Mock<IClock>();
            var userQueries = new Mock<IUserQueries>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            userQueries.Setup(x => x.CheckForPresenceOfUserByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new StatusCheckModel(false));

            var handler = new CreateUserCommandHandler(userRepository.Object, clock.Object, userQueries.Object);
            var cmd = new CreateUserCommand(new string('*', 5), new string('*', 6), new string('*', 7), false);
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenUserInSystem_ExpectFailedResult()
        {
            var clock = new Mock<IClock>();
            var userQueries = new Mock<IUserQueries>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            userQueries.Setup(x => x.CheckForPresenceOfUserByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new StatusCheckModel(true));

            var handler = new CreateUserCommandHandler(userRepository.Object, clock.Object, userQueries.Object);
            var cmd = new CreateUserCommand(new string('*', 5), new string('*', 6), new string('*', 7), false);
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserAlreadyExists, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenUserNotInSystem_ExpectUserToBeAdded()
        {
            var clock = new Mock<IClock>();
            var userQueries = new Mock<IUserQueries>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            userQueries.Setup(x => x.CheckForPresenceOfUserByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new StatusCheckModel(false));

            var handler = new CreateUserCommandHandler(userRepository.Object, clock.Object, userQueries.Object);
            var cmd = new CreateUserCommand(new string('*', 5), new string('*', 6), new string('*', 7), false);
            await handler.Handle(cmd, CancellationToken.None);

            userRepository.Verify(x => x.Add(It.IsAny<IUser>()), Times.Once);
        }
    }
}