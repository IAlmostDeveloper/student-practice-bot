using System;
using System.Linq;
using DataBaseAccess.Data;
using VkBot.Data;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace VkBot.Services
{
    public class VkEventHandler
    {
        private readonly IVkApi vkApi;
        private readonly QuestionAndAnswerRepository questionAndAnswerRepository;

        public VkEventHandler(IVkApi vkApi, QuestionAndAnswerRepository questionAndAnswerRepository)
        {
            this.vkApi = vkApi;
            this.questionAndAnswerRepository = questionAndAnswerRepository;
        }

        public void MessageNew(VkResponse vkResponse)
        {
            var message = Message.FromJson(vkResponse);
            var response = questionAndAnswerRepository.FindByQuestion(message.Text);
            var messagesSendParams = new MessagesSendParams
            {
                RandomId = new DateTime().Millisecond,
                PeerId = message.PeerId,
                Message = response != null ? response.Answer : "Ваш вопрос не найден в моей БД"
            };
            vkApi.Messages.Send(messagesSendParams);
        }

        public void GroupJoin(VkResponse vkResponse)
        {
            var groupJoin = VkNet.Model.GroupUpdate.GroupJoin.FromJson(vkResponse);
            var userName = "Человек";
            if (groupJoin.UserId != null)
            {
                var user = vkApi.Users.Get(new[] {groupJoin.UserId.Value}).FirstOrDefault();
                userName = user != null ? user.FirstName : userName;
            }

            var messageParams = new MessagesSendParams
            {
                PeerId = groupJoin.UserId,
                RandomId = new DateTime().Millisecond,
                Message = $"Hello {userName} :3"
            };
            vkApi.Messages.Send(messageParams);
        }
    }
}