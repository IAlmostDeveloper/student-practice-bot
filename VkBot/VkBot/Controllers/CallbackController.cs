using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VkBot.Data;
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
        
        private readonly UserRepository userRepository;

        public CallbackController(IConfiguration configuration, IVkApi vkApi, UserRepository userRepository)
        {
            this.configuration = configuration;
            this.vkApi = vkApi;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public IActionResult Callback([FromBody] Updates updates)
        {
            // Проверяем, что находится в поле "type" 
            Console.WriteLine(updates.Type);
            switch (updates.Type)
            {
                // Если это уведомление для подтверждения адреса
                case "confirmation":
                    // Отправляем строку для подтверждения 
                    return Ok(configuration["Config:Confirmation"]);
                case "message_new":
                    // Десериализация
                    var msg = Message.FromJson(new VkResponse(updates.Object));
                    
                    var response = userRepository.FindByFirstName(msg.Text);
                    
                    vkApi.Messages.Send(new MessagesSendParams
                    {
                        RandomId = new DateTime().Millisecond,
                        PeerId = msg.PeerId,
                        Message = response != null ? response.LastName : "No such student"
                    });
                    break;
            }

            // Возвращаем "ok" серверу Callback API
            return Ok("ok");
        }
    }
}