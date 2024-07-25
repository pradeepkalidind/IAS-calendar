using System;
using Calendar.Service.Filters;
using Calendar.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Service.Controller
{
    [UserBasicAuth]
    public class VisitedUserController : NewApiController
    {
        private readonly VisitedUserService service;

        public VisitedUserController()
        {
            service = new VisitedUserService(session);
        }

        [HttpGet("User/{userId}/IsVisited")]
        public ActionResult IsVisitedUser(Guid userId)
        {
            return Ok(service.IsVisitedUser(userId));
        }

        [HttpPost("User/{userId}/MarkAsVisited")]
        public ActionResult MarkAsVisited(Guid userId)
        {
            service.MarkAsVisited(userId);
            return Ok("success");
        }
    }
}
