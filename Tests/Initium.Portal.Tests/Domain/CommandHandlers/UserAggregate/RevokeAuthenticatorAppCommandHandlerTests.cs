﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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
    public class RevokeAuthenticatorAppCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenNoUserAppearsToBeAuthenticate_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>
            {
                new AuthenticatorApp(TestVariables.AuthenticatorAppId, "key", TestVariables.Now),
            });
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

            var handler = new RevokeAuthenticatorAppCommandHandler(
                userRepository.Object, Mock.Of<IClock>(), currentAuthenticatedUserProvider.Object, Mock.Of<ILogger<RevokeAuthenticatorAppCommandHandler>>());
            var cmd = new RevokeAuthenticatorAppCommand("password");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>
            {
                new AuthenticatorApp(TestVariables.AuthenticatorAppId, "key", TestVariables.Now),
            });
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();

            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ResultWithError.Fail(Mock.Of<IPersistenceError>()));
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new RevokeAuthenticatorAppCommandHandler(
                userRepository.Object, Mock.Of<IClock>(), currentAuthenticatedUserProvider.Object, Mock.Of<ILogger<RevokeAuthenticatorAppCommandHandler>>());
            var cmd = new RevokeAuthenticatorAppCommand("password");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>
            {
                new AuthenticatorApp(TestVariables.AuthenticatorAppId, "key", TestVariables.Now),
            });
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new RevokeAuthenticatorAppCommandHandler(
                userRepository.Object, Mock.Of<IClock>(), currentAuthenticatedUserProvider.Object, Mock.Of<ILogger<RevokeAuthenticatorAppCommandHandler>>());
            var cmd = new RevokeAuthenticatorAppCommand("password");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenUserDoesExist_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>
            {
                new AuthenticatorApp(TestVariables.AuthenticatorAppId, "key", TestVariables.Now),
            });
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe<IUser>.Nothing);

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new RevokeAuthenticatorAppCommandHandler(
                userRepository.Object, Mock.Of<IClock>(), currentAuthenticatedUserProvider.Object, Mock.Of<ILogger<RevokeAuthenticatorAppCommandHandler>>());
            var cmd = new RevokeAuthenticatorAppCommand("password");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenUserHasAppEnrolled_ExpectSuccessfulResultAndRevokeAttempted()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>
            {
                new AuthenticatorApp(TestVariables.AuthenticatorAppId, "key", TestVariables.Now),
            });
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new RevokeAuthenticatorAppCommandHandler(
                userRepository.Object, Mock.Of<IClock>(), currentAuthenticatedUserProvider.Object, Mock.Of<ILogger<RevokeAuthenticatorAppCommandHandler>>());
            var cmd = new RevokeAuthenticatorAppCommand("password");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsSuccess);
            user.Verify(x => x.RevokeAuthenticatorApp(It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenUserHasNoAppEnrolled_ExpectFailedResultAndNoRevokeAttempted()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>());
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new RevokeAuthenticatorAppCommandHandler(
                userRepository.Object, Mock.Of<IClock>(), currentAuthenticatedUserProvider.Object, Mock.Of<ILogger<RevokeAuthenticatorAppCommandHandler>>());
            var cmd = new RevokeAuthenticatorAppCommand("password");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.NoAuthenticatorAppEnrolled, result.Error.Code);
            user.Verify(x => x.RevokeAuthenticatorApp(It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task Handle_GivenPasswordIsNotCorrect_ExpectFailedResultAndNoRevokeAttempted()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new RevokeAuthenticatorAppCommandHandler(
                userRepository.Object, Mock.Of<IClock>(), currentAuthenticatedUserProvider.Object, Mock.Of<ILogger<RevokeAuthenticatorAppCommandHandler>>());
            var cmd = new RevokeAuthenticatorAppCommand("wrong-password");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.PasswordNotCorrect, result.Error.Code);
            user.Verify(x => x.RevokeAuthenticatorApp(It.IsAny<DateTime>()), Times.Never);
        }
    }
}