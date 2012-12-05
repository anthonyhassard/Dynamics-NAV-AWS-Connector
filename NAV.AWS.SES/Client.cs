using System;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace NAV.AWS.SES
{
	/// <summary>
	///     A simple helper class to keep SES client access streamlined.
	/// </summary>
	internal class Client : IDisposable
	{
		/// <summary>
		///     A read only field for <see langword="internal" /> access to the Amazon SES client.
		/// </summary>
		private readonly AmazonSimpleEmailService _sesClient;


		/// <summary>
		///     A read only field for <see langword="internal" /> access to AWS credentials.
		/// </summary>
		private readonly BasicAWSCredentials _awsCredentials;


		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="credentials">
		///     A <see cref="Credentials" /> object.
		/// </param>
		internal Client(Credentials credentials)
		{
			_awsCredentials = new BasicAWSCredentials(credentials.AccessKey, credentials.SecureKey);
			_sesClient = AWSClientFactory.CreateAmazonSimpleEmailServiceClient(_awsCredentials);
		}


		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (_sesClient != null)
				_sesClient.Dispose();
			if (_awsCredentials != null)
				_awsCredentials.Dispose();
		}


		/// <summary>
		///     Sends a formatted email.
		/// </summary>
		/// <param name="request">
		///     A <see cref="SendEmailRequest" /> object.
		/// </param>
		/// <returns>
		///     The unique <see cref="Message" /> ID for the email.
		/// </returns>
		internal string SendFormattedEmail(SendEmailRequest request)
		{
			SendEmailResponse response = _sesClient.SendEmail(request);
			return response != null && response.SendEmailResult != null ? response.SendEmailResult.MessageId : string.Empty;
		}


		/// <summary>
		///     Sends a raw email.
		/// </summary>
		/// <param name="request">
		///     A <see cref="SendRawEmailRequest" /> object.
		/// </param>
		/// <returns>
		///     The unique <see cref="Message" /> ID for the email.
		/// </returns>
		internal string SendRawEmail(SendRawEmailRequest request)
		{
			SendRawEmailResponse response = _sesClient.SendRawEmail(request);
			return response != null && response.SendRawEmailResult != null ? response.SendRawEmailResult.MessageId : string.Empty;
		}
	}
}
