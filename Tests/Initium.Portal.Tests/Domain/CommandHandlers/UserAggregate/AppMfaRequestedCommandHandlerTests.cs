﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Authentication;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandHandlers.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.UserAggregate
{
    public class AppMfaRequestedCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenNoUserAppearsToBeAuthenticate_ExpectFailedResultAndNoAttemptLogged()
        {
            var user = new Mock<IUser>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);

            var handler = new AppMfaRequestedCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<IClock>(), Mock.Of<ILogger<AppMfaRequestedCommandHandler>>());
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
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ResultWithError.Fail(Mock.Of<IPersistenceError>()));
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

            var handler = new AppMfaRequestedCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<IClock>(), Mock.Of<ILogger<AppMfaRequestedCommandHandler>>());
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
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
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

            var handler = new AppMfaRequestedCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<IClock>(), Mock.Of<ILogger<AppMfaRequestedCommandHandler>>());
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
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
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

            var handler = new AppMfaRequestedCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<IClock>(), Mock.Of<ILogger<AppMfaRequestedCommandHandler>>());
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
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
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

            var handler = new AppMfaRequestedCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<IClock>(), Mock.Of<ILogger<AppMfaRequestedCommandHandler>>());
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