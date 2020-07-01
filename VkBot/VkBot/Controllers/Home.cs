using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace VkBot.Controllers
{
	[Route("")]
	public class Home : Controller
	{
		private readonly IConfiguration configuration;

		public Home(IConfiguration configuration)
		{
			this.configuration = configuration;
		}
		
		[HttpGet]
		public ActionResult GetConfirmCode()
		{
			return Ok(configuration["Config:Confirmation"]);
		}
	}
}