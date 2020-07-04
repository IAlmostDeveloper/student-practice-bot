using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VkBot.Services;

namespace VkBot.Controllers
{
	[ApiController]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class ErrorController : ControllerBase
	{
		private readonly ErrorsStore errorsStore;

		public ErrorController(ErrorsStore errorsStore)
		{
			this.errorsStore = errorsStore;
		}
		
		[Route("/handleError")]
		public IActionResult HandleError()
		{
			var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
			errorsStore.Errors.Add(DateTime.Now, context.Error);
			return Problem();
		}
	}
}