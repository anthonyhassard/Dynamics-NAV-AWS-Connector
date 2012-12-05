using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.SQS;

namespace NAV.AWS.SQS
{
	public class Service
	{
		public Credentials Credentials { get; set; }

		public Service(Credentials credentials)
		{
			Credentials = credentials;
		}


		public string ErrorMessage { get; set; }

		/// <summary>Determines whether an error exists.</summary>
		/// <value><see langword="true"/> if error exists, false if not.</value>
		public bool ErrorExists
		{
			get { return !string.IsNullOrWhiteSpace(ErrorMessage); }
		}


		public Queue[] Queues
		{
			get
			{
				try
				{
					using (var client = new Client(Credentials))
					{
						return client.QueueList.ToArray();
					}
				}
				catch (AmazonSQSException ex)
				{
					ErrorMessage = string.Format(
						"AWS Simple Queue Service Exception\n\nError Type: {0}\n" +
						"Error Code: {1}\nRequest Id: {2}\nStatus Code: {3}\n\n{4}",
						ex.ErrorType, ex.ErrorCode, ex.RequestId, ex.StatusCode, ex);
				}
				catch (Exception ex)
				{
					ErrorMessage = ex.ToString();
				}
				return new Queue[] { };
			}
		}

		public bool QueueExists(string name)
		{
			return (Queues.Any(q => q.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));			
		}

		public bool QueueExists(Queue queue)
		{
			return (Queues.Any(q => q.Url.Equals(queue.Url, StringComparison.OrdinalIgnoreCase)));
		}

		public Queue CreateQueue(string name)
		{
			try
			{
				using (var client = new Client(Credentials))
				{
					return client.CreateQueue(name);
				}
			}
			catch (AmazonSQSException ex)
			{
				ErrorMessage = string.Format(
					"AWS Simple Queue Service Exception\n\nError Type: {0}\n" +
					"Error Code: {1}\nRequest Id: {2}\nStatus Code: {3}\n\n{4}",
					ex.ErrorType, ex.ErrorCode, ex.RequestId, ex.StatusCode, ex);				
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.ToString();
			}
			return null;
		}


		public Queue CreateQueue(string name, int visibilitySeconds)
		{
			try
			{
				using (var client = new Client(Credentials))
				{
					return client.CreateQueue(name, visibilitySeconds);
				}
			}
			catch (AmazonSQSException ex)
			{
				ErrorMessage = string.Format(
					"AWS Simple Queue Service Exception\n\nError Type: {0}\n" +
					"Error Code: {1}\nRequest Id: {2}\nStatus Code: {3}\n\n{4}",
					ex.ErrorType, ex.ErrorCode, ex.RequestId, ex.StatusCode, ex);
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.ToString();
			}
			return null;
		}


		public Queue CreateQueue(string name, int visibilitySeconds, int delaySeconds)
		{
			try
			{
				using (var client = new Client(Credentials))
				{
					return client.CreateQueue(name, visibilitySeconds, delaySeconds);
				}
			}
			catch (AmazonSQSException ex)
			{
				ErrorMessage = string.Format(
					"AWS Simple Queue Service Exception\n\nError Type: {0}\n" +
					"Error Code: {1}\nRequest Id: {2}\nStatus Code: {3}\n\n{4}",
					ex.ErrorType, ex.ErrorCode, ex.RequestId, ex.StatusCode, ex);
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.ToString();
			}
			return null;
		}

		public void SendMessage(Message message)
		{
			try
			{
				using (var client = new Client(Credentials))
				{
					client.SendMessage(message);
				}
			}
			catch (AmazonSQSException ex)
			{
				ErrorMessage = string.Format(
					"AWS Simple Queue Service Exception\n\nError Type: {0}\n" +
					"Error Code: {1}\nRequest Id: {2}\nStatus Code: {3}\n\n{4}",
					ex.ErrorType, ex.ErrorCode, ex.RequestId, ex.StatusCode, ex);				
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.ToString();
			}
		}


		public void SendMessageBatch(List<Message> messages)
		{
			try
			{
				using (var client = new Client(Credentials))
				{
					client.SendMessageBatch(messages);
				}
			}
			catch (AmazonSQSException ex)
			{
				ErrorMessage = string.Format(
					"AWS Simple Queue Service Exception\n\nError Type: {0}\n" +
					"Error Code: {1}\nRequest Id: {2}\nStatus Code: {3}\n\n{4}",
					ex.ErrorType, ex.ErrorCode, ex.RequestId, ex.StatusCode, ex);
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.ToString();
			}
		}

		public Message ReceiveMessage(Queue queue)
		{
			try
			{
			}
			catch (AmazonSQSException ex)
			{
				ErrorMessage = string.Format(
					"AWS Simple Queue Service Exception\n\nError Type: {0}\n" +
					"Error Code: {1}\nRequest Id: {2}\nStatus Code: {3}\n\n{4}",
					ex.ErrorType, ex.ErrorCode, ex.RequestId, ex.StatusCode, ex);
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.ToString();
			}
			return null;
		}

		public void DeleteMessage(Queue queue, Message message)
		{
			try
			{
			}
			catch (AmazonSQSException ex)
			{
				ErrorMessage = string.Format(
					"AWS Simple Queue Service Exception\n\nError Type: {0}\n" +
					"Error Code: {1}\nRequest Id: {2}\nStatus Code: {3}\n\n{4}",
					ex.ErrorType, ex.ErrorCode, ex.RequestId, ex.StatusCode, ex);
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.ToString();
			}			
		}
	}
}
