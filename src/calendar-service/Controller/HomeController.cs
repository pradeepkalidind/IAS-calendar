using Microsoft.AspNetCore.Mvc;

namespace Calendar.Service.Controller
{
    public class HomeController : NewApiController
    {
        [HttpGet("")]
        public ActionResult Home()
        {
            return Ok("IAS Calendar Service");
        }
    }
}