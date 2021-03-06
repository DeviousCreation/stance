﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;

namespace Initium.Portal.Domain.CommandResults.UserAggregate
{
    public class InitiateAuthenticatorDeviceEnrollmentCommandResult
    {
        public InitiateAuthenticatorDeviceEnrollmentCommandResult(CredentialCreateOptions options)
        {
            this.Options = options;
        }

        public CredentialCreateOptions Options { get; }
    }
}