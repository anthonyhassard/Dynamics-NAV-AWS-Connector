using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAV.AWS.SQS;

namespace NAV.AWS.Tests
{
	/// <summary>Tests for the Amazon Simple Queue Service.</summary>
	[TestClass]
	public class SQS
	{
		/// <summary>A <see langword="static" /> TestContext object for use in just this test class.</summary>
		private static TestContext _localTestContext;


		/// <summary><see cref="Initialize" /> the test class and add the AWS key file to the TestContext properties dictionary.</summary>
		/// <param name="context">The context.</param>
		[ClassInitialize]
		public static void Initialize(TestContext context)
		{
			_localTestContext = context;
			_localTestContext.Properties.Add("QueueName", "Queue01");
		}


		/// <summary>Test the creation of a <see cref="NAV.AWS.SQS.Queue"/> object.</summary>
		[TestMethod]
		public void CreateQueue()
		{
			var sqs = new NAV.AWS.SQS.Service(AWS.Credentials);
			var queue = sqs.CreateQueue(_localTestContext.Properties["QueueName"].ToString());
			Assert.IsTrue(!sqs.ErrorExists, sqs.ErrorMessage);
			Assert.IsTrue(queue.Name.Equals(_localTestContext.Properties["QueueName"].ToString(), StringComparison.OrdinalIgnoreCase));
			Assert.IsTrue(!string.IsNullOrWhiteSpace(queue.Url));
			Assert.IsTrue(sqs.QueueExists(queue));
			Assert.IsTrue(!sqs.ErrorExists, sqs.ErrorMessage);
			Console.Out.WriteLine("Queue: {0}, {1}", queue.Name, queue.Url);
			_localTestContext.Properties.Add("Queue", queue);
		}

		/// <summary>Sends the message.</summary>
		[TestMethod]
		public void SendMessage()
		{
			var sqs = new NAV.AWS.SQS.Service(AWS.Credentials);
			var queue = _localTestContext.Properties["Queue"] as NAV.AWS.SQS.Queue;
			Assert.IsNotNull(queue);
			var message = new Message(queue) { Body = "Test Message", DelaySeconds = 0 };
			sqs.SendMessage(message);
			Assert.IsTrue(!string.IsNullOrWhiteSpace(message.AwsAssignedId));
			Console.Out.WriteLine("Message: {0}", message.AwsAssignedId);
		}
	}
}
