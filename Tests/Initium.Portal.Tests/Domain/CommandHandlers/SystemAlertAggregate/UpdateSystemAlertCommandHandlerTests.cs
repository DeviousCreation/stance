﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.SystemAlertAggregate;
using Initium.Portal.Domain.CommandHandlers.SystemAlertAggregate;
using Initium.Portal.Domain.Commands.SystemAlertAggregate;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.SystemAlertAggregate
{
    public class UpdateSystemAlertCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ResultWithError.Fail(Mock.Of<IPersistenceError>()));
            var systemAlertRepository = new Mock<ISystemAlertRepository>();
            systemAlertRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            systemAlertRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(new Mock<ISystemAlert>().Object));

            var commandHandler =
                new UpdateSystemAlertCommandHandler(systemAlertRepository.Object, Mock.Of<ILogger<UpdateSystemAlertCommandHandler>>());

            var command = new UpdateSystemAlertCommand(
                TestVariables.SystemAlertId,
                "name",
                "message",
                SystemAlertType.Critical,
                TestVariables.Now.AddDays(-1),
                TestVariables.Now.AddDays(1));

            var result = await commandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessResult()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            var systemAlertRepository = new Mock<ISystemAlertRepository>();
            systemAlertRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            systemAlertRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(new Mock<ISystemAlert>().Object));

            var commandHandler =
                new UpdateSystemAlertCommandHandler(systemAlertRepository.Object, Mock.Of<ILogger<UpdateSystemAlertCommandHandler>>());

            var command = new UpdateSystemAlertCommand(
                TestVariables.SystemAlertId,
                "name",
                "message",
                SystemAlertType.Critical,
                TestVariables.Now.AddDays(-1),
                TestVariables.Now.AddDays(1));

            var result = await commandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenCommandIsValid_ExpectUpdatesToBeCalled()
        {
            var systemAlert = new Mock<ISystemAlert>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            var systemAlertRepository = new Mock<ISystemAlertRepository>();
            systemAlertRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            systemAlertRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(systemAlert.Object));

            var commandHandler =
                new UpdateSystemAlertCommandHandler(systemAlertRepository.Object, Mock.Of<ILogger<UpdateSystemAlertCommandHandler>>());

            var command = new UpdateSystemAlertCommand(
                TestVariables.SystemAlertId,
                "name",
                "message",
                SystemAlertType.Critical,
                TestVariables.Now.AddDays(-1),
                TestVariables.Now.AddDays(1));

            await commandHandler.Handle(command, CancellationToken.None);
            systemAlertRepository.Verify(x => x.Update(It.IsAny<ISystemAlert>()), Times.Once);
            systemAlert.Verify(x => x.UpdateDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SystemAlertType>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenSystemAlertDoesNotExist_ExpectFailedResult()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            var systemAlertRepository = new Mock<ISystemAlertRepository>();
            systemAlertRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            systemAlertRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe<ISystemAlert>.Nothing);

            var commandHandler =
                new UpdateSystemAlertCommandHandler(systemAlertRepository.Object, Mock.Of<ILogger<UpdateSystemAlertCommandHandler>>());

            var command = new UpdateSystemAlertCommand(
                TestVariables.SystemAlertId,
                "name",
                "message",
                SystemAlertType.Critical,
                TestVariables.Now.AddDays(-1),
                TestVariables.Now.AddDays(1));

            var result = await commandHandler.Handle(command, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SystemAlertNotFound, result.Error.Code);
        }
    }
}