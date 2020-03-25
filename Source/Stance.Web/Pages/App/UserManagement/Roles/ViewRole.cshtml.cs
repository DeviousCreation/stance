// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static.Models.Role;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.App.UserManagement.Roles
{
    public class ViewRole : NotificationPageModel
    {
        private readonly IRoleQueries _roleQueries;

        public ViewRole(IRoleQueries roleQueries)
        {
            this._roleQueries = roleQueries;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public string Name { get; private set; }

        public DetailedRoleModel Role { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var roleMaybe = await this._roleQueries.GetDetailsOfRoleById(this.Id);
            if (roleMaybe.HasNoValue)
            {
                return this.NotFound();
            }

            this.Role = roleMaybe.Value;

            this.Name = this.Role.Name;

            return this.Page();
        }
    }
}