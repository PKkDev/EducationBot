using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EducationBot.Telegram.Controllers
{
    /// <summary>
    /// accept service error on prod
    /// </summary>
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger;

        /// <summary>
        /// initialization
        /// </summary>
        /// <param name="logger"></param>
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// generate error response from service
        /// </summary>
        /// <returns></returns>
        [Route("error")]
        public HttpErrorMessage AcceptAPIError()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            string message = exception?.Message ?? "Exception occurred";

            _logger.LogError(message);

            Response.StatusCode = 500;
            var response = new HttpErrorMessage(message);
            return response;
        }
    }

    /// <summary>
    /// response object
    /// </summary>
    public class HttpErrorMessage
    {
        /// <summary>
        /// message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="message"></param>
        public HttpErrorMessage(string message) => Message = message;
    }
}
