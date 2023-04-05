using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Volunteer.Api.Filters;

public class NullFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var nullParams = filterContext.ActionArguments.Where(p => p.Value == null)
            .Select(p => p.Key)
            .ToList();
        
        if (nullParams.Any())
        {
            filterContext.Result = new BadRequestResult();
        }
    }

}