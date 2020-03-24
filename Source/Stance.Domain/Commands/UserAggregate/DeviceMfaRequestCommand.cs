﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MediatR;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.CommandResults.UserAggregate;

namespace Stance.Domain.Commands.UserAggregate
{
    public class DeviceMfaRequestCommand : IRequest<Result<DeviceMfaRequestCommandResult, ErrorData>>
    {
    }
}