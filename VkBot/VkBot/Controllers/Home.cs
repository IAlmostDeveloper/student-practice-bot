using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VkBot.Services;

namespace VkBot.Controllers
{
	[Route("")]
	public class Home : Controller
	{
		private readonly IConfiguration configuration;
		private readonly ErrorsStore errorsStore;

		public Home(IConfiguration configuration, ErrorsStore errorsStore)
		{
			this.configuration = configuration;
			this.errorsStore = errorsStore;
		}
		
		[HttpGet]
		public ActionResult GetConfirmCode()
		{
			var message = configuration["Config:Confirmation"];
			var errors = errorsStore.Errors
				.OrderByDescending(e => e.Key)
				.Select(error => $"\n{error.Key}\n{error.Value}\n");
			var errorMessage = string.Join(new string('=', 100), errors);
			return Ok($"{message}\n{errorMessage}");
		}

		[HttpGet("error")]
		public ActionResult Error()
		{
			throw new Exception("Some error text");
		}
	}
}