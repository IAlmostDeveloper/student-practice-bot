using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VkBot.Models;
using VkNet.Abstractions;
using VkNet.Model;
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

		private readonly IVkApi vkApi;

		public CallbackController(IConfiguration configuration, IVkApi vkApi)
		{
			this.configuration = configuration;
			this.vkApi = vkApi;
		}

		[HttpPost]
		public IActionResult Callback([FromBody] Updates updates)
		{
			// Проверяем, что находится в поле "type" 
			switch (updates.Type)
			{
				// Если это уведомление для подтверждения адреса
				case "confirmation":
					// Отправляем строку для подтверждения 
					return Ok(configuration["Config:Confirmation"]);
				case "message_new":
					// Десериализация
					var msg = Message.FromJson(new VkResponse(updates.Object));

					// Отправим в ответ полученный от пользователя текст
					vkApi.Messages.Send(new MessagesSendParams
					{
						RandomId = new DateTime().Millisecond,
						PeerId = msg.PeerId,
						Message = msg.Text
					});
					break;
			}
			// Возвращаем "ok" серверу Callback API
			return Ok("ok");
		}
	}
}