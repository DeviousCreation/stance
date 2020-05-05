﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Stance.Web.Pages.External
{
    public class Error : PageModel
    {
        public void OnGet()
        {
            this.Response.StatusCode = 200;
        }
    }
}