using System;
using System.Linq;
using DataBaseAccess.Data.Repositories;
using Newtonsoft.Json;
using VkBot.Data.Entities;
using VkNet.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
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
        private const int MaxMessageButtonLength = 40;

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
            var answers = GetAnswers(message);
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

        private QuestionAndAnswer[] GetAnswers(Message message)
        {
            QuestionAndAnswer[] answers;
            if (message.Payload != null)
            {
                var question = (string) JsonConvert.DeserializeObject<dynamic>(message.Payload).Question;
                var answer = questionAndAnswerRepository.FindByQuestion(question);
                const string unknownAnswer = "Мне не удалось найти этот ответ.\nЧто-то пошло не так :(";
                answers = new[] {answer ?? new QuestionAndAnswer {Answer = unknownAnswer}};
            }
            else
            {
                var answer = questionAndAnswerRepository.FindByQuestion(message.Text);
                if (answer != null)
                    answers = new[] {answer};
                else 
                {
                    var answersOnly = wordAndAnswerRepository.FindSeveralByPhrase(message.Text.ToLower()).ToArray();
                    answers = questionAndAnswerRepository.FindSeveralByAnswers(answersOnly).ToArray();
                }
            }

            return answers;
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
                .Select(answer =>
                {
                    var label = answer.Question;
                    if (label.Length > MaxMessageButtonLength)
                    {
                        var labelParts = answer.Question.Take(MaxMessageButtonLength - 3);
                        label = string.Join("", labelParts) + "...";
                    }
                    return new[] {BuildTextButton(label, ("Question", answer.Question))};
                });
            var keyboard = new MessageKeyboard
            {
                Buttons = buttons,
                Inline = true
            };
            const string messageText = "Возможно вы имели ввиду один из этих вопросов :3";
            SendMessage(peerId, messageText, keyboard);
        }
        
        private MessageKeyboardButton BuildTextButton(string label, (string, string) actionValue = default)
        {
            var payload = $"{{\"{actionValue.Item1}\":\"{actionValue.Item2}\"}}";
            var action = new MessageKeyboardButtonAction
            {
                Label = label,
                Type = KeyboardButtonActionType.Text,
                Payload = actionValue == default ? null : payload
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