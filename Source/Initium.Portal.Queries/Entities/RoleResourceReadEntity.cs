﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Contracts.Queries;

namespace Initium.Portal.Queries.Entities
{
    public class RoleResourceReadEntity : ReadOnlyEntity
    {
        public Guid RoleId { get; private set; }

        public Guid ResourceId { get; private set; }

        public RoleReadEntity Role { get; private set; }

        public ResourceReadEntity Resource { get; private set; }
    }
}