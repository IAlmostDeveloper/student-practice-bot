using System;
using System.Linq;
using DataBaseAccess.Data;
using DataBaseAccess.Data.Repositories;
using VkBot.Data;
using VkBot.Data.Entities;
using VkNet.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace VkBot.Services
{
    public class VkEventHandler
    {
        private readonly IVkApi vkApi;
        private readonly WordAndAnswerRepository wordAndAnswerRepository;
        private readonly QuestionAndAnswerRepository questionAndAnswerRepository;
        private const int MaxAnswersCount = 5;

        public VkEventHandler(IVkApi vkApi, 
            WordAndAnswerRepository wordAndAnswerRepository,
            QuestionAndAnswerRepository questionAndAnswerRepository)
        {
            this.vkApi = vkApi;
            this.wordAndAnswerRepository = wordAndAnswerRepository;
            this.questionAndAnswerRepository = questionAndAnswerRepository;
        }

        public void MessageNew(VkResponse vkResponse)
        {
            var message = Message.FromJson(vkResponse);
            var answersOnly = wordAndAnswerRepository.FindSeveralByPhrase(message.Text.ToLower()).ToArray();
            var answers = questionAndAnswerRepository.FindSeveralByAnswers(answersOnly).ToArray();
            switch (answers.Length)
            {
                case 0:
                    SendMessage(message.PeerId, "Я не знаю ответа на такой вопрос :(");
                    break;
                case 1:
                    SendMessage(message.PeerId, answers.First().Answer);
                    break;
                default:
                    SendMultiAnswersMessage(answers, message.PeerId);
                    break;
            }
        }

        private void SendMessage(long? peerId, string messageText, MessageKeyboard messageKeyboard = null)
        {
            var messagesSendParams = new MessagesSendParams
            {
                RandomId = new DateTime().Millisecond,
                PeerId = peerId,
                Message = messageText,
                Keyboard = messageKeyboard
            };
            vkApi.Messages.Send(messagesSendParams);
        }

        private void SendMultiAnswersMessage(QuestionAndAnswer[] answers, long? peerId)
        {
            var buttons = answers
                .Take(MaxAnswersCount)
                .Select(a => new[] {BuildTextButton(a.Question)});
            var keyboard = new MessageKeyboard
            {
                Buttons = buttons,
                Inline = true
            };
            const string messageText = "Возможно вы имели ввиду один из этих вопросов :3";
            SendMessage(peerId, messageText, keyboard);
        }
        
        private MessageKeyboardButton BuildTextButton(string label)
        {
            var action = new MessageKeyboardButtonAction
            {
                Label = label,
                Type = KeyboardButtonActionType.Text,
            };
            var button = new MessageKeyboardButton
            {
                Action = action,
                Color = KeyboardButtonColor.Primary
            };
            return button;
        }
    }
}