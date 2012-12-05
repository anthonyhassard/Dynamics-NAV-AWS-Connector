using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAV.AWS.SQS
{
	public class Attribute
	{
		private readonly Queue _queue;

		private readonly Message _message;


		public Queue Queue
		{
			get { return _queue; }
		}


		public Message Message
		{
			get { return _message; }
		}


		public Attribute(Queue queue, Message message)
		{
			_queue = queue;
			_message = message;
		}


		public string Name { get; set; }


		public string Value { get; set; }
	}
}
