using System;
using System.Collections.Generic;
using System.Linq;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace NAV.AWS.SQS
{
	/// <summary>
	///     A basic helper class for providing an SQS client.
	/// </summary>
	internal sealed class Client : IDisposable
	{
		/// <summary>
		///     A read only field for <see langword="private" /> access to AWS credentials.
		/// </summary>
		private readonly BasicAWSCredentials _awsCredentials;


		/// <summary>
		///     A read only field for <see langword="private" /> access to the Amazon SQS client.
		/// </summary>
		private readonly AmazonSQS _sqsClient;


		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="credentials">
		///     A <see cref="Credentials" /> object.
		/// </param>
		internal Client(Credentials credentials)
		{
			_awsCredentials = new BasicAWSCredentials(credentials.AccessKey, credentials.SecureKey);
			_sqsClient = AWSClientFactory.CreateAmazonSQSClient(_awsCredentials);
		}


		/// <summary>
		/// </summary>
		internal List<Queue> QueueList
		{
			get
			{
				var queueList = new List<Queue>();
				ListQueuesResponse response = _sqsClient.ListQueues(new ListQueuesRequest());
				if (response.IsSetListQueuesResult())
				{
					ListQueuesResult result = response.ListQueuesResult;
					if (result.IsSetQueueUrl())
					{
						foreach (string url in result.QueueUrl)
						{
							var queue = new Queue { Url = url };
							string[] splitUrl = url.Split('/');
							if (url.EndsWith("/", StringComparison.Ordinal))
							{
								if (splitUrl.Length >= 2)
									queue.Name = splitUrl[splitUrl.Length - 2];
							}
							else
							{
								if (splitUrl.Length >= 1)
									queue.Name = splitUrl[splitUrl.Length - 1];
							}
							queueList.Add(queue);
						}
					}
				}
				return queueList;
			}
		}


		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (_sqsClient != null)
				_sqsClient.Dispose();
			if (_awsCredentials != null)
				_awsCredentials.Dispose();
		}


		/// <summary>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="visibilitySeconds"></param>
		/// <param name="delaySeconds"></param>
		/// <returns></returns>
		internal Queue CreateQueue(string name, int visibilitySeconds = 30, int delaySeconds = 0)
		{
			var request = new CreateQueueRequest
			{
				QueueName = name,
				DefaultVisibilityTimeout = Convert.ToDecimal(visibilitySeconds),
				DelaySeconds = delaySeconds
			};
			CreateQueueResponse response = _sqsClient.CreateQueue(request);
			return response.CreateQueueResult.IsSetQueueUrl()
					   ? new Queue { Name = name, Url = response.CreateQueueResult.QueueUrl }
					   : null;
		}


		/// <summary>
		/// </summary>
		/// <param name="queue"></param>
		internal void DeleteQueue(Queue queue) { }


		/// <summary>
		/// </summary>
		/// <returns></returns>
		internal Message ReceiveMessage()
		{
			return null;
		}


		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		internal void SendMessage(Message message)
		{
			var request = new SendMessageRequest
			{
				QueueUrl = message.Queue.Url,
				MessageBody = message.Body,
				DelaySeconds = message.DelaySeconds
			};
			SendMessageResponse response = _sqsClient.SendMessage(request);
			if (response.IsSetSendMessageResult())
			{
				if (response.SendMessageResult.IsSetMessageId())
					message.AwsAssignedId = response.SendMessageResult.MessageId;
			}
		}


		/// <exception cref="Exception"></exception>
		/// <exception cref="ArgumentNullException">url</exception>
		private void CheckMessageBatchRestrictions(List<Message> messages, string url)
		{
			if (url == null) throw new ArgumentNullException("url");
			if (messages.Exists(m => String.Compare(m.Queue.Url, url, StringComparison.OrdinalIgnoreCase) != 0))
			{
				throw new Exception(
					string.Format(
						"Error when sending messages to Queue '{0}': " +
						"All messages sent in a batch must reference the same URL.",
						messages[0].Queue));
			}

			if (messages.Count > 10)
				throw new Exception(
					string.Format("Error when sending messages to Queue '{0}': " +
					"No more than 10 messages can be sent in a single batch.", messages[0].Queue));
		}

		/// <exception cref="Exception" />
		internal void SendMessageBatch(List<Message> messages)
		{
			if (messages == null || messages.Count == 0)
				return;
	
			string url = messages[0].Queue.Url;

			CheckMessageBatchRestrictions(messages, url);

			int batchId = 0;
			var entries = new List<SendMessageBatchRequestEntry>();
			foreach (var message in messages)
			{
				if (string.IsNullOrWhiteSpace(message.BatchId))
					message.BatchId = Convert.ToString(batchId++);
				entries.Add(new SendMessageBatchRequestEntry
				{
					DelaySeconds = message.DelaySeconds,
					Id = message.BatchId,
					MessageBody = message.Body
				});
			}

			var request = new SendMessageBatchRequest { Entries = entries, QueueUrl = url };
			var response = _sqsClient.SendMessageBatch(request);
			if (response.IsSetSendMessageBatchResult())
			{
				if (response.SendMessageBatchResult.IsSetSendMessageBatchResultEntry())
				{
					foreach (var resultEntry in response.SendMessageBatchResult.SendMessageBatchResultEntry)
					{
						if (resultEntry.IsSetId() && resultEntry.IsSetMessageId())
						{
							messages.Single(m => m.BatchId.Equals(resultEntry.Id)).AwsAssignedId = resultEntry.MessageId;
						}
					}
				}
				else if (response.SendMessageBatchResult.IsSetBatchResultErrorEntry())
				{
					foreach (var errorEntry in response.SendMessageBatchResult.BatchResultErrorEntry)
					{
						if (errorEntry.IsSetId())
						{
							messages.Single(m => m.BatchId.Equals(errorEntry.Id)).Error = new Error
							{
								BatchId = errorEntry.Id,
								Code = errorEntry.Code,
								MessageText = errorEntry.Message,
								SenderFault = errorEntry.SenderFault,
								Service = "SQS Send Message Batch"
							};
						}
					}
				}
			}
		}
	}
}
