﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilePocket.WebApi.Controllers
{
    [Authorize]
    public class BaseController : ControllerBase
    {
        public Guid UserId => Guid.Parse(User.FindFirst("uid")!.Value);
    }
}
