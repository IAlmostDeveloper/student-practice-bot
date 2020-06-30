using System;
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

		public VkEventHandler(IVkApi vkApi)
		{
			this.vkApi = vkApi;
		}

		public void MessageNew(VkResponse vkResponse)
		{
			var message = Message.FromJson(vkResponse);
			var messagesSendParams = new MessagesSendParams
			{
				RandomId = new DateTime().Millisecond,
				PeerId = message.PeerId,
				Message = message.Text
			};
			vkApi.Messages.Send(messagesSendParams);
		}
		
		public void GroupJoin(VkResponse vkResponse)
		{
			var groupJoin = VkNet.Model.GroupUpdate.GroupJoin.FromJson(vkResponse);
			var messageParams = new MessagesSendParams
			{
				PeerId = groupJoin.UserId,
				RandomId = new DateTime().Millisecond,
				Message = "Hello Человек :3"
			};
			vkApi.Messages.Send(messageParams);
		}
	}
}