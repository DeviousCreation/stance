﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Settings;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandHandlers.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.Events.IntegrationEvents;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NodaTime;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.UserAggregate
{
    public class RequestAccountVerificationCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(Guid.NewGuid(), "first-name", "last-name"));
            user.Setup(x => x.GenerateNewAccountConfirmationToken(It.IsAny<DateTime>(), It.IsAny<TimeSpan>()))
                .Returns(new SecurityTokenMapping(
                    TestVariables.SecurityTokenMappingId,
                    SecurityTokenPurpose.AccountConfirmation,
                    TestVariables.Now,
                    TestVariables.Now.AddDays(1)));

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ResultWithError.Fail(Mock.Of<IPersistenceError>()));
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var handler = new RequestAccountVerificationCommandHandler(userRepository.Object, securitySettings.Object, Mock.Of<IClock>(), Mock.Of<ILogger<RequestAccountVerificationCommandHandler>>());
            var cmd = new RequestAccountVerificationCommand("email-address");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(Guid.NewGuid(), "first-name", "last-name"));
            user.Setup(x => x.GenerateNewAccountConfirmationToken(It.IsAny<DateTime>(), It.IsAny<TimeSpan>()))
                .Returns(new SecurityTokenMapping(
                    TestVariables.SecurityTokenMappingId,
                    SecurityTokenPurpose.AccountConfirmation,
                    TestVariables.Now,
                    TestVariables.Now.AddDays(1)));

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var handler = new RequestAccountVerificationCommandHandler(userRepository.Object, securitySettings.Object, Mock.Of<IClock>(), Mock.Of<ILogger<RequestAccountVerificationCommandHandler>>());
            var cmd = new RequestAccountVerificationCommand("email-address");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsSuccess);
        }

        [Fact]

        public async Task Handle_GivenUserDoesNotExist_ExpectFailedResult()
        {
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe<IUser>.Nothing);

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var handler = new RequestAccountVerificationCommandHandler(userRepository.Object, securitySettings.Object, Mock.Of<IClock>(), Mock.Of<ILogger<RequestAccountVerificationCommandHandler>>());
            var cmd = new RequestAccountVerificationCommand("email-address");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]

        public async Task Handle_GivenUserIsAlreadyVerified_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(Guid.NewGuid(), "first-name", "last-name"));
            user.Setup(x => x.IsVerified).Returns(true);
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var handler = new RequestAccountVerificationCommandHandler(userRepository.Object, securitySettings.Object, Mock.Of<IClock>(), Mock.Of<ILogger<RequestAccountVerificationCommandHandler>>());
            var cmd = new RequestAccountVerificationCommand("email-address");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserIsAlreadyVerified, result.Error.Code);
        }

        [Fact]

        public async Task Handle_GivenUserIsNotVerified_ExpectNewTokenGeneratedAndDomainEventRaised()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(Guid.NewGuid(), "first-name", "last-name"));
            user.Setup(x => x.GenerateNewAccountConfirmationToken(It.IsAny<DateTime>(), It.IsAny<TimeSpan>()))
                .Returns(new SecurityTokenMapping(
                    TestVariables.SecurityTokenMappingId,
                    SecurityTokenPurpose.AccountConfirmation,
                    TestVariables.Now,
                    TestVariables.Now.AddDays(1)));

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var handler = new RequestAccountVerificationCommandHandler(userRepository.Object, securitySettings.Object, Mock.Of<IClock>(), Mock.Of<ILogger<RequestAccountVerificationCommandHandler>>());
            var cmd = new RequestAccountVerificationCommand("email-address");

            await handler.Handle(cmd, CancellationToken.None);

            user.Verify(x => x.GenerateNewAccountConfirmationToken(It.IsAny<DateTime>(), It.IsAny<TimeSpan>()), Times.Once);
            userRepository.Verify(x => x.Update(It.IsAny<IUser>()), Times.Once);
            user.Verify(x => x.AddIntegrationEvent(It.IsAny<AccountConfirmationTokenGeneratedIntegrationEvent>()));
        }
    }
}