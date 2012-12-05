using System;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace NAV.AWS.SES
{
	/// <summary>Send formatted email via Amazon's Simple Email Service (SES).</summary>
	internal class FormattedEmail
	{
		/// <summary>An Email object.</summary>
		private readonly Email _email;


		/// <summary>Constructor.</summary>
		/// <exception cref="ArgumentNullException">email.</exception>
		/// <param name="email">The Email object.</param>
		internal FormattedEmail(Email email)
		{
			if (email == null) throw new ArgumentNullException("email");
			_email = email;
		}


		/// <summary>
		///     <see cref="Send" /> the email message to Amazon's Simple Email Service.
		/// </summary>
		/// <returns>true if it succeeds, false if it fails.</returns>
		internal bool Send()
		{
			_email.MessageId = string.Empty;

			try
			{
				var message = new Message
				{
					Body =
						_email.HTML
							? new Body().WithHtml(new Content(_email.MessageBody)) : new Body().WithText(new Content(_email.MessageBody)),
					Subject = new Content(_email.MessageSubject)
				};
				var request = new SendEmailRequest(_email.FromAddress, _email.Destination, message);
				using (var client = new Client(_email.Credentials))
				{
					_email.MessageId = client.SendFormattedEmail(request);
				}

				return !_email.ErrorExists;
			}
			catch (AmazonSimpleEmailServiceException ex)
			{
				return _email.SetErrorMessage(
					string.Format(
						"AWS Simple Email Service Exception\n\nError Type: {0}\n" +
						"Error Code: {1}\nRequest Id: {2}\nStatus Code: {3}\n\n{4}",
						ex.ErrorType, ex.ErrorCode, ex.RequestId, ex.StatusCode, ex));
			}
			catch (AmazonClientException ex)
			{
				return _email.SetErrorMessage(ex.ToString());
			}
			catch (Exception ex)
			{
				return _email.SetErrorMessage(ex.ToString());
			}
		}
	}
}

//TODO: Add error handling & logging
//TODO: Add attachments & raw message handling
