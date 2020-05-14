﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Initium.Portal.Queries.Static.Models;
using Initium.Portal.Queries.Static.Models.User;
using MaybeMonad;

namespace Initium.Portal.Queries.Contracts.Static
{
    public interface IUserQueries
    {
        Task<StatusCheckModel> CheckForPresenceOfAnyUser();

        Task<Maybe<ProfileModel>> GetProfileForCurrentUser();

        Task<StatusCheckModel> CheckForPresenceOfUserByEmailAddress(string emailAddress);

        Task<Maybe<DetailedUserModel>> GetDetailsOfUserById(Guid userId);

        Task<Maybe<SystemProfileModel>> GetSystemProfileByUserId(Guid userId);

        Task<StatusCheckModel> CheckForPresenceOfAuthAppForCurrentUser();

        Task<Maybe<List<DeviceInfo>>> GetDeviceInfoForCurrentUser();
    }
}