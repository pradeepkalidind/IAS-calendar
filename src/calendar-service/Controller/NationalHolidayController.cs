using Calendar.Service.Filters;
using Calendar.Service.Requests;
using Calendar.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Service.Controller
{
    [UserBasicAuth]
    public class NationalHolidayController : NewApiController
    {
        private readonly INationalHolidayService service;

        public NationalHolidayController()
        {
            service = new NationalHolidayService();
        }

        [HttpPost("NationalHoliday/AvailableCountries")]
        public ActionResult AvailableCountries()
        {
            var result = service.GetAvailableCountries();
            return Ok(result);
        }

        [HttpPost("NationalHoliday/GetNationalHolidaysByCountry")]
        public ActionResult GetNationalHolidaysByCountry([FromForm] GetNationalHolidaysRequest request, [FromQuery] string culture = null)
        {
            return Ok(service.GetNameOfNationalHolidaysByCountry(request.Country, culture));
        }
    }
}
