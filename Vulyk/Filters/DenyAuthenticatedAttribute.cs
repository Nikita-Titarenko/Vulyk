using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Vulyk.Controllers;

namespace Vulyk.Filters
{
    public class DenyAuthenticatedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult(nameof(ChatController.Index), "Chat", null);
            }
        }
    }
}
