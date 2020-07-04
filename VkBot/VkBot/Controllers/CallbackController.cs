using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VkBot.Models;
using VkBot.Services;
using VkNet.Utils;

namespace VkBot.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CallbackController : ControllerBase
	{
		private readonly IConfiguration configuration;

		private readonly VkEventHandler eventHandler;

		public CallbackController(IConfiguration configuration, VkEventHandler eventHandler)
		{
			this.configuration = configuration;
			this.eventHandler = eventHandler;
		}

		[HttpPost]
		public IActionResult Callback([FromBody] EventDto eventDto)
		{
			Console.WriteLine(eventDto.Type);
			var vkResponse = new VkResponse(eventDto.Object);
			switch (eventDto.Type)
			{
				case "confirmation":
					return Ok(configuration["Config:Confirmation"]);
				case "message_new":
					eventHandler.MessageNew(vkResponse);
					break;
			}
			return Ok("ok");
		}
	}
}