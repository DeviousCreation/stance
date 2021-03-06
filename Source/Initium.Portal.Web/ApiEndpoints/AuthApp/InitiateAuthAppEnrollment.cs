﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Initium.Portal.Core.Authentication;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.ApiEndpoints.AuthApp
{
    public class InitiateAuthAppEnrollment : BaseAsyncEndpoint
        .WithoutRequest
        .WithResponse<InitiateAuthAppEnrollment.EndpointResponse>
    {
        [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded",
        Justification = "This is the fix template used by totp apps")]
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IMediator _mediator;
        private readonly UrlEncoder _urlEncoder;
        private readonly FeatureBasedTenantInfo _tenantInfo;

        public InitiateAuthAppEnrollment(ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, IMediator mediator, UrlEncoder urlEncoder, FeatureBasedTenantInfo tenantInfo)
        {
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
            this._mediator = mediator;
            this._urlEncoder = urlEncoder;
            this._tenantInfo = tenantInfo;
        }

        [HttpPost("api/auth-app/initiate-enrollment", Name = "InitiateAuthAppEnrollmentEndpoint")]
        [ValidateAntiForgeryToken]
        public override async Task<ActionResult<EndpointResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                return this.Ok(new EndpointResponse());
            }

            if (!(currentUserMaybe.Value is AuthenticatedUser user))
            {
                return this.Ok(new EndpointResponse());
            }

            var result = await this._mediator.Send(new InitiateAuthenticatorAppEnrollmentCommand(), cancellationToken);
            if (result.IsFailure)
            {
                return this.Ok(new EndpointResponse());
            }

            var formattedSharedKey = FormatAuthenticatorAppKey(result.Value.SharedKey);
            var authenticatorUri = string.Format(
                AuthenticatorUriFormat,
                this._urlEncoder.Encode(this._tenantInfo.Name),
                this._urlEncoder.Encode(user.EmailAddress),
                result.Value.SharedKey);
            return this.Ok(new EndpointResponse(result.Value.SharedKey, formattedSharedKey,
                authenticatorUri));
        }

        private static string FormatAuthenticatorAppKey(string base32Key)
        {
            var result = new StringBuilder();
            var currentPosition = 0;
            while (currentPosition + 4 < base32Key.Length)
            {
                result.Append(base32Key.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }

            if (currentPosition < base32Key.Length)
            {
                result.Append(base32Key.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        public class EndpointResponse : BasicEndpointResponse
        {
            public EndpointResponse()
                : base(false)
            {
            }

            public EndpointResponse(string sharedKey, string formattedSharedKey, string authenticatorUri)
                : base(true)
            {
                this.SharedKey = sharedKey;
                this.FormattedSharedKey = formattedSharedKey;
                this.AuthenticatorUri = authenticatorUri;
            }

            public string SharedKey { get; }

            public string FormattedSharedKey { get; }

            public string AuthenticatorUri { get; }
        }
    }
}