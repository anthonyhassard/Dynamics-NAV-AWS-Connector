using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAV.AWS.SQS
{
	public class Message
	{
		private readonly Queue _queue;

		public int DelaySeconds { get; set; }


		public Message(Queue queue)
		{
			_queue = queue;
		}


		public Queue Queue
		{
			get { return _queue; }
		}


		public string AwsAssignedId { get; set; }

		public string BatchId { get; set; }

		public string ReceiptHandle { get; set; }

		public string Body { get; set; }

		public Error Error { get; set; }

		public Attribute[] Attributes { get; set; }
	}
}


