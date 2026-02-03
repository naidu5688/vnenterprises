using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace vnenterprises.Filters
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        private readonly int _requiredAccessType;

        public AuthorizeUserAttribute(int requiredAccessType)
        {
            _requiredAccessType = requiredAccessType;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;

            var userId = request.Cookies["UserId"];
            var accessType = request.Cookies["AccessType"];

            // ❌ Not logged in
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessType))
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            // ❌ Access type mismatch
            if (int.Parse(accessType) != _requiredAccessType)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
