// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Initium.Portal.Web.Pages.App
{
    [Authorize]
    public class Index : PageModel
    {
    }
}