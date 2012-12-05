using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace NAV.AWS.SES
{
	/// <summary>
	///     Send raw email via Amazon's Simple <see cref="Email" /> Service (SES).
	/// </summary>
	internal class RawEmail
	{
		/// <summary>
		///     An <see cref="Email" /> object.
		/// </summary>
		private readonly Email _email;


		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="email">
		///     An <see cref="Email" /> object.
		/// </param>
		/// <exception cref="ArgumentNullException">email.</exception>
		internal RawEmail(Email email)
		{
			if (email == null) throw new ArgumentNullException("email");
			_email = email;
		}


		/// <summary>
		///     Convert a standard .NET <see cref="MailMessage" /> to a <see cref="MemoryStream" /> . This is used to create a
		///     raw email <paramref name="message" /> containing attachments to Amazon's SES.
		/// </summary>
		/// <param name="message">.</param>
		/// <returns>System.IO.MemoryStream.</returns>
		public static MemoryStream ConvertMailMessageToMemoryStream(MailMessage message)
		{
			var fileStream = new MemoryStream();
			object mailWriter =
				typeof (SmtpClient).Assembly.GetType("System.Net.Mail.MailWriter").GetConstructor(
					BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof (Stream) }, null).Invoke(
						new object[] { fileStream });
			typeof (MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(
				message, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { mailWriter, true, true }, null);
			mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(
				mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);
			return fileStream;
		}


		/// <summary>
		///     <see cref="Send" /> this message.
		/// </summary>
		/// <returns>
		///     <see langword="true" /> if it succeeds, <see langword="false" /> if it fails.
		/// </returns>
		internal bool Send()
		{
			_email.MessageId = string.Empty;

			try
			{
				var mailMessage = new MailMessage { From = new MailAddress(_email.FromAddress) };

				foreach (string toAddress in _email.ToAddressList)
					mailMessage.To.Add(new MailAddress(toAddress));

				foreach (string ccAddress in _email.CcAddressList)
					mailMessage.CC.Add(new MailAddress(ccAddress));

				foreach (string bccAddress in _email.BccAddressList)
					mailMessage.Bcc.Add(new MailAddress(bccAddress));

				mailMessage.Subject = _email.MessageSubject;
				mailMessage.SubjectEncoding = Encoding.UTF8;
				mailMessage.AlternateViews.Add(
					_email.HTML
						? AlternateView.CreateAlternateViewFromString(_email.MessageBody, Encoding.UTF8, "text/html")
						: AlternateView.CreateAlternateViewFromString(_email.MessageBody, Encoding.UTF8, "text/plain"));

				var attachment = new Attachment(_email.AttachmentFilePath)
				{
					ContentType = new ContentType("application/octet-stream")
				};

				ContentDisposition disposition = attachment.ContentDisposition;
				disposition.DispositionType = "attachment";
				disposition.CreationDate = File.GetCreationTime(_email.AttachmentFilePath);
				disposition.ModificationDate = File.GetLastWriteTime(_email.AttachmentFilePath);
				disposition.ReadDate = File.GetLastAccessTime(_email.AttachmentFilePath);
				mailMessage.Attachments.Add(attachment);

				var rawMessage = new RawMessage();

				using (MemoryStream memoryStream = ConvertMailMessageToMemoryStream(mailMessage))
					rawMessage.WithData(memoryStream);

				var request = new SendRawEmailRequest
				{
					RawMessage = rawMessage,
					Destinations = _email.Destination.ToAddresses,
					Source = _email.FromAddress
				};

				using (var client = new Client(_email.Credentials))
				{
					_email.MessageId = client.SendRawEmail(request);
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
