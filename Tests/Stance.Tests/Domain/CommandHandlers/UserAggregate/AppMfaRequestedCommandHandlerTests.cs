﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Moq;
using NodaTime;
using Stance.Core;
using Stance.Core.Constants;
using Stance.Core.Contracts;
using Stance.Core.Contracts.Domain;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandHandlers.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandHandlers.UserAggregate
{
    public class AppMfaRequestedCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenNoUserAppearsToBeAuthenticate_ExpectFailedResultAndNoAttemptLogged()
        {
            var user = new Mock<IUser>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);

            var clock = new Mock<IClock>();

            var handler = new AppMfaRequestedCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);
            var cmd = new AppMfaRequestedCommand();

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
            user.Verify(
                x => x.ProcessPartialSuccessfulAuthenticationAttempt(
                    It.IsAny<DateTime>(), It.IsAny<AuthenticationHistoryType>()), Times.Never);
        }

        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(() =>
                {
                    ISystemUser systemUser = new UnauthenticatedUser(Guid.NewGuid(), MfaProvider.None);
                    return Maybe.From(systemUser);
                });

            var clock = new Mock<IClock>();

            var handler = new AppMfaRequestedCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);
            var cmd = new AppMfaRequestedCommand();

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var user = new Mock<IUser>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(() =>
                {
                    ISystemUser systemUser = new UnauthenticatedUser(Guid.NewGuid(), MfaProvider.None);
                    return Maybe.From(systemUser);
                });

            var clock = new Mock<IClock>();

            var handler = new AppMfaRequestedCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);
            var cmd = new AppMfaRequestedCommand();

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenUserDoesExist_ExpectSuccessfulResultAndAttemptLogged()
        {
            var user = new Mock<IUser>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(() =>
                {
                    ISystemUser systemUser = new UnauthenticatedUser(Guid.NewGuid(), MfaProvider.None);
                    return Maybe.From(systemUser);
                });

            var clock = new Mock<IClock>();

            var handler = new AppMfaRequestedCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);
            var cmd = new AppMfaRequestedCommand();

            await handler.Handle(cmd, CancellationToken.None);
            user.Verify(
                x => x.ProcessPartialSuccessfulAuthenticationAttempt(
                    It.IsAny<DateTime>(), It.IsAny<AuthenticationHistoryType>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenUserDoesNotExist_ExpectFailedResultAndNoAttemptLogged()
        {
            var user = new Mock<IUser>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe<IUser>.Nothing);

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(() =>
                {
                    ISystemUser systemUser = new UnauthenticatedUser(Guid.NewGuid(), MfaProvider.None);
                    return Maybe.From(systemUser);
                });

            var clock = new Mock<IClock>();

            var handler = new AppMfaRequestedCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);
            var cmd = new AppMfaRequestedCommand();

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
            user.Verify(
                x => x.ProcessPartialSuccessfulAuthenticationAttempt(
                    It.IsAny<DateTime>(), It.IsAny<AuthenticationHistoryType>()), Times.Never);
        }
    }
}