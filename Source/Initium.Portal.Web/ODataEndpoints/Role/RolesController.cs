﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Entities;
using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.Infrastructure.OData.Endpoints;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Initium.Portal.Web.ODataEndpoints.Role
{
    [ResourceBasedAuthorize("role-list")]
    public class RolesController : BaseODataController<RoleReadEntity, RoleFilter>
    {
        private readonly IRoleQueryService _roleQueryService;

        public RolesController(IRoleQueryService roleQueryService)
        {
            this._roleQueryService = roleQueryService;
        }

        [HttpPost]
        public override IActionResult Filtered(ODataQueryOptions<RoleReadEntity> options, [FromBody]RoleFilter filter)
        {
            if (!this.AreOptionsValid(options))
            {
                return this.BadRequest();
            }

            if (filter == null)
            {
                return this.Ok(options.ApplyTo(this._roleQueryService.QueryableEntity));
            }

            var predicate = this.GeneratePredicate(filter);
            return this.Ok(options.ApplyTo(this._roleQueryService.QueryableEntity.Where(predicate)));
        }

        [HttpPost]
        public override IActionResult FilteredExport(ODataQueryOptions<RoleReadEntity> options, [FromBody]ExportableFilter<RoleFilter> filter)
        {
            if (!this.AreOptionsValid(options))
            {
                return this.BadRequest();
            }

            IQueryable query;
            IDictionary<string, string> mappings;
            if (filter == null)
            {
                query = options.ApplyTo(this._roleQueryService.QueryableEntity);
                mappings = new Dictionary<string, string>();
            }
            else
            {
                var predicate = this.GeneratePredicate(filter.Filter);
                query = this._roleQueryService.QueryableEntity.Where(predicate);
                mappings = filter.Mappings;
            }

            return this.File(this.GenerateCsvStream(query, options, mappings), "application/csv");
        }

        protected override ExpressionStarter<RoleReadEntity> GeneratePredicate(RoleFilter filter)
        {
            var predicate = PredicateBuilder.New<RoleReadEntity>(true);
            if (filter.HasResources && !filter.HasNoResources)
            {
                predicate.And(x => x.ResourceCount > 0);
            }
            else if (filter.HasNoResources && !filter.HasResources)
            {
                predicate.And(x => x.ResourceCount < 1);
            }

            if (filter.HasUsers && !filter.HasNoUsers)
            {
                predicate.And(x => x.UserCount > 0);
            }
            else if (filter.HasNoUsers && !filter.HasUsers)
            {
                predicate.And(x => x.UserCount < 1);
            }

            return predicate;
        }
    }
}