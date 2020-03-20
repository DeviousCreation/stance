﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Stance.Core.Constants;

namespace Stance.Web.Infrastructure.Contracts
{
    public interface IAuthenticationService
    {
        Task SignInUserPartiallyAsync(
            Guid userId,
            MfaProvider setupMfaProviders,
            string returnUrl = null);

        Task<string> SignInUserFromPartialStateAsync(Guid userId);
    }
}