using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.SQS.Model;

namespace NAV.AWS.SQS
{
	public class Queue
	{

		public string Name { get; set; }

		public string Url { get; set; }


		public Queue() { }


		public Message[] Messages { get; set; }

		public override string ToString()
		{
			return string.Format("Name: {0}, URL: {1}", Name, Url);
		}
	}
}
