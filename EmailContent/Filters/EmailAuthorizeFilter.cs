using EmailContentServices.Attributes;
using EmailContentServices.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace EmailContentServices.Filters
{
    public class EmailAuthorizeFilter : IPageFilter
    {
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            // do nothing
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            // do nothing
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var authorizeEmail = context.ActionDescriptor.EndpointMetadata.OfType<AuthorizeEmailAttribute>();

            // if there's no authorization requirement, then nothing else to do
            if (!authorizeEmail.Any()) return;

            // local requests always work
            if (context.HttpContext.Request.Host.Host.Equals("localhost")) return;

            // if you're logged in, it always works
            if (context.HttpContext.User.Identity?.IsAuthenticated ?? false) return;

            // check for a token that may have been added by ServiceProviderExtensions.GetHtmlContentAsync
            if (HasValidToken(context)) return;

            // if you made it here, it means you can't view this content
            throw new Exception("Not authorized");
        }

        private bool HasValidToken(PageHandlerExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger<EmailAuthorizeFilter>)) as ILogger<EmailAuthorizeFilter>;

            if (request.Headers.ContainsKey(ServiceProviderExtensions.EmailToken))
            {
                var token = request.Headers[ServiceProviderExtensions.EmailToken];
                var url = (request.Path + request.QueryString).Substring(1); // needs to trim leading slash
                var hash = context.HttpContext.RequestServices.BuildEmailToken(url);
                if (hash.Equals(token))
                {
                    return true;
                }
                else
                {
                    logger.LogError($"failed hash check for {request.Path}. The hash was {hash} and the token was {token}");
                    return false;
                }
            }

            return false;
        }

    }
}
