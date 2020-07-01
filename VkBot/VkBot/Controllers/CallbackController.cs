using System;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using VkBot.Models;
using VkBot.Services;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace VkBot.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CallbackController : ControllerBase
	{
		/// <summary>
		/// Конфигурация приложения
		/// </summary>
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
			var vkResponse = new VkResponse(eventDto.Object);
			switch (eventDto.Type)
			{
				case "confirmation":
					return Ok(configuration["Config:Confirmation"]);
				case "message_new":
					eventHandler.MessageNew(vkResponse);
					break;
				case "group_join":
					eventHandler.GroupJoin(vkResponse);
					break;
			}
			return Ok("ok");
		}
	}
}