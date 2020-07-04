using System;
using System.Collections.Generic;

namespace VkBot.Services
{
	public class ErrorsStore
	{
		public readonly Dictionary<DateTime, Exception> Errors = new Dictionary<DateTime, Exception>();
	}
}