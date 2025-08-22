using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Vulyk.Web.Controllers;

namespace Vulyk.Web.Filters
{
    public class DenyAuthenticatedAttribute : Attribute, IPageFilter
    {
        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {

        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult(nameof(ChatController.Index), "Chat", new {area = ""});
            }
        }

        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {

        }
    }
}
