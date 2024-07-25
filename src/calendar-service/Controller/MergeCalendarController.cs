using System;
using Calendar.Service.Filters;
using Calendar.Service.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Service.Controller
{
    [UserBasicAuth]
    // TODO:SQL injection
    public class MergeCalendarController : NewApiController
    {
        // did not find any usage from github search
        [HttpPost("MergeCalendar/Merge")]
        public ActionResult Merge([FromForm] MergeRequest request)
        {
            if (request.MyTravelUserId.Equals(request.MyTaxesUserId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Ok();
            }

            using (var transaction = session.BeginTransaction())
            {
                session.CreateSQLQuery(
                        string.Format(@"update MonthActivity
set [ActivityContent] = u.[ActivityContent],[FirstLocationActivityAllocationContent]=u.[FirstLocationActivityAllocationContent],[LocationPatternContent]=u.[LocationPatternContent]
from MonthActivity
inner join 
(select [ActivityContent],[FirstLocationActivityAllocationContent],[LocationPatternContent],dest.id from
  (SELECT [UserId],[Year],[Month],[ActivityContent],[FirstLocationActivityAllocationContent],[LocationPatternContent],[Id],[timestamp]
  FROM [calendar].[dbo].[MonthActivity]
  where userid='{1}') source
  inner join
  (SELECT [UserId],[Year],[Month],[Id],[timestamp]
  FROM [calendar].[dbo].[MonthActivity]
  where userid='{0}') dest
  on dest.[Year] = source.[Year] and source.[Month] = dest.[Month]  and source.[timestamp] > dest.[timestamp]) u
  on u.id = MonthActivity.id", request.MyTaxesUserId, request.MyTravelUserId))
                    .ExecuteUpdate();

                session.CreateSQLQuery(
                        string.Format(
                            @"insert into MonthActivity(userid,[Year],[Month],[ActivityContent],[FirstLocationActivityAllocationContent],[LocationPatternContent],[Id],[DayType],[DataSource],[NoTravelConfirmation])
select '{0}',source.[Year],source.[Month],source.[ActivityContent],source.[FirstLocationActivityAllocationContent],source.[LocationPatternContent],newid(),source.[DayType],source.[DataSource],source.[NoTravelConfirmation] from
  (SELECT [UserId],[Year],[Month],[ActivityContent],[FirstLocationActivityAllocationContent],[LocationPatternContent],[Id],[DayType],[DataSource],[NoTravelConfirmation]
  FROM [MonthActivity]
  where userid='{1}') source
  left outer join
  (SELECT [UserId],[Year],[Month],[Id],[timestamp]
  FROM [MonthActivity]
  where userid='{0}') dest
  on dest.[Year] = source.[Year] and source.[Month] = dest.[Month]
  where dest.Id is null
", request.MyTaxesUserId, request.MyTravelUserId))
                    .ExecuteUpdate();
                transaction.Commit();
            }

            return Ok();
        }
    }
}