using System;
using System.Collections.Generic;
using System.IO;
using Amazon.SimpleEmail.Model;

namespace NAV.AWS.SES
{
	/// <summary>Email.</summary>
	public class Email
	{
		/// <summary>At least one '{0}' value must be specified.</summary>
		private const string AtLeastOneValueMustBeSpecified = "At least one '{0}' value must be specified.";


		/// <summary>The '{0}' value cannot be empty.</summary>
		private const string TheValueCannotBeEmpty = "The '{0}' value cannot be empty.";


		/// <summary>
		///     Initializes a new instance of the <see cref="NAV.AWS.SES.Email" /> class.
		/// </summary>
		/// <param name="credentials">.</param>
		public Email(Credentials credentials)
		{
			Credentials = credentials;
			Destination = new Destination();
		}


		/// <summary>The destination address class.</summary>
		/// <value>The destination.</value>
		internal Destination Destination { get; set; }


		/// <summary>Gets or sets the credentials.</summary>
		/// <value>The credentials.</value>
		public Credentials Credentials { get; set; }


		/// <summary>Gets or sets the email message subject.</summary>
		/// <value>The email message subject.</value>
		public string MessageSubject { get; set; }


		/// <summary>Determines whether an error exists for this Email object.</summary>
		/// <value>true if error exists, false if not.</value>
		public bool ErrorExists
		{
			get { return !string.IsNullOrWhiteSpace(ErrorMessage); }
		}


		/// <summary>Gets or sets a value indicating whether the email message is formatted in HTML.</summary>
		/// <value>
		///     <see langword="true" /> if html, <see langword="false" /> if plain text.
		/// </value>
		public bool HTML { get; set; }


		/// <summary>Gets or sets a list of send-to addresses.</summary>
		/// <value>A list of send-to addresses.</value>
		internal List<string> ToAddressList
		{
			get { return Destination.ToAddresses ?? (Destination.ToAddresses = new List<string>()); }
			set { Destination.ToAddresses = value; }
		}


		/// <summary>Gets or sets a list of CC addresses.</summary>
		/// <value>A list of CC addresses.</value>
		internal List<string> CcAddressList
		{
			get { return Destination.CcAddresses ?? (Destination.CcAddresses = new List<string>()); }
			set { Destination.CcAddresses = value; }
		}


		/// <summary>Gets or sets a list of BCC addresses.</summary>
		/// <value>A list of BCC addresses.</value>
		internal List<string> BccAddressList
		{
			get { return Destination.BccAddresses ?? (Destination.BccAddresses = new List<string>()); }
			set { Destination.BccAddresses = value; }
		}


		/// <summary>Gets or sets the sender's email address.</summary>
		/// <value>The sender's email address.</value>
		public string FromAddress { get; set; }


		/// <summary>Gets or sets the email message body.</summary>
		/// <value>
		///     The body of the message, formatted in <c>HTML</c> if the <see cref="NAV.AWS.SES.Email.HTML" /> property
		///     is true, otherwise formatted as plain text .
		/// </value>
		public string MessageBody { get; set; }


		/// <summary>Gets or sets a message describing the last error that occurred (blank means there were no errors).</summary>
		/// <value>A message describing the error.</value>
		public string ErrorMessage { get; set; }


		/// <summary>Gets or sets the full path to a file that will be attached to the email.</summary>
		/// <value>A string indicating where the attachment file is located.</value>
		public string AttachmentFilePath { get; set; }


		/// <summary>Gets or sets the Message ID returned from the SES response.</summary>
		/// <value>The identifier of the message.</value>
		public string MessageId { get; internal set; }


		/// <summary>
		///     Adds a BCC email <paramref name="address" /> to the message.
		/// </summary>
		/// <param name="address">The BCC email address.</param>
		public void AddBccAddress(string address)
		{
			if (BccAddressList == null)
				BccAddressList = new List<string>();
			if (!string.IsNullOrWhiteSpace(address))
				BccAddressList.Add(address);
		}


		/// <summary>Adds a carbon copy recipient to the message.</summary>
		/// <param name="address">
		///     The recipient's email <paramref name="address" /> .
		/// </param>
		public void AddCcAddress(string address)
		{
			if (CcAddressList == null)
				CcAddressList = new List<string>();
			if (!string.IsNullOrWhiteSpace(address))
				CcAddressList.Add(address);
		}


		/// <summary>Adds a recipient to the message.</summary>
		/// <param name="address">
		///     The recipient's email <paramref name="address" /> .
		/// </param>
		public void AddToAddress(string address)
		{
			if (ToAddressList == null)
				ToAddressList = new List<string>();
			if (!string.IsNullOrWhiteSpace(address))
				ToAddressList.Add(address);
		}


		/// <summary>Removes a previously added BCC recipient.</summary>
		/// <param name="address">
		///     The recipient's email <paramref name="address" /> .
		/// </param>
		public void RemoveBccAddress(string address)
		{
			if (BccAddressList != null && !string.IsNullOrWhiteSpace(address))
				BccAddressList.Remove(address);
		}


		/// <summary>Removes a previously added CC recipient.</summary>
		/// <param name="address">
		///     The recipient's email <paramref name="address" /> .
		/// </param>
		public void RemoveCcAddress(string address)
		{
			if (CcAddressList != null && !string.IsNullOrWhiteSpace(address))
				CcAddressList.Remove(address);
		}


		/// <summary>Removes a previously added recipient.</summary>
		/// <param name="address">
		///     The recipient's email <paramref name="address" /> .
		/// </param>
		public void RemoveToAddress(string address)
		{
			if (ToAddressList != null && !string.IsNullOrWhiteSpace(address))
				ToAddressList.Remove(address);
		}


		/// <summary>Sends the email message to Amazon's Simple Email Service.</summary>
		/// <returns>
		///     <see langword="true" /> if the message is sent successfully, <see langword="false" /> if it fails.
		/// </returns>
		public bool Send()
		{
			if (!Validate()) return false;
			if (!string.IsNullOrWhiteSpace(AttachmentFilePath) && File.Exists(AttachmentFilePath))
			{
				var rawEmail = new RawEmail(this);
				return rawEmail.Send();
			}
			var formattedEmail = new FormattedEmail(this);
			return formattedEmail.Send();
		}


		/// <summary>Send the email message to Amazon's Simple Email Service.</summary>
		/// <param name="toAddress">
		///     A single email recipient. Use the <see cref="Email.AddToAddress" /> method for multiple recipients.
		/// </param>
		/// <returns>
		///     <see langword="true" /> if the message is sent successfully, <see langword="false" /> if it fails.
		/// </returns>
		public bool Send(string toAddress)
		{
			return Send(toAddress, string.Empty, string.Empty);
		}


		/// <summary>Send the email message to Amazon's Simple Email Service.</summary>
		/// <param name="toAddress">
		///     A single email recipient. Use the <see cref="Email.AddToAddress" /> method for multiple recipients.
		/// </param>
		/// <param name="ccAddress">
		///     A single carbon copy recipient. Use the <see cref="Email.AddCcAddress" /> method for multiple recipients.
		/// </param>
		/// <returns>
		///     <see langword="true" /> if the message is sent successfully, <see langword="false" /> if it fails.
		/// </returns>
		public bool Send(string toAddress, string ccAddress)
		{
			return Send(toAddress, ccAddress, string.Empty);
		}


		/// <summary>Send the email message to Amazon's Simple Email Service.</summary>
		/// <param name="toAddress">
		///     A single email recipient. Use the <see cref="Email.AddToAddress" /> method for multiple recipients.
		/// </param>
		/// <param name="ccAddress">
		///     A single carbon copy recipient. Use the <see cref="Email.AddCcAddress" /> method for multiple recipients.
		/// </param>
		/// <param name="bccAddress">The Bcc address.</param>
		/// <returns>
		///     <see langword="true" /> if the message is sent successfully, <see langword="false" /> if it fails.
		/// </returns>
		public bool Send(string toAddress, string ccAddress, string bccAddress)
		{
			AddToAddress(toAddress);
			AddCcAddress(ccAddress);
			AddBccAddress(bccAddress);
			return Send();
		}


		/// <summary>Send the email message to Amazon's Simple Email Service.</summary>
		/// <param name="fromAddress">The sender's email address.</param>
		/// <param name="toAddress">
		///     A single email recipient. Use the <see cref="Email.AddToAddress" /> method for multiple recipients.
		/// </param>
		/// <param name="messageSubject">The email message subject.</param>
		/// <param name="messageBody">
		///     The body of the message, formatted in <c>HTML</c> if the <see cref="NAV.AWS.SES.Email.HTML" /> property
		///     is true, otherwise formatted as plain text .
		/// </param>
		/// <param name="html">
		///     <see langword="true" /> if html, <see langword="false" /> if plain text.
		/// </param>
		/// <returns>
		///     <see langword="true" /> if the message is sent successfully, <see langword="false" /> if it fails.
		/// </returns>
		public bool Send(string fromAddress, string toAddress, string messageSubject, string messageBody, bool html)
		{
			return Send(fromAddress, toAddress, string.Empty, string.Empty, messageSubject, messageBody, html);
		}


		/// <summary>Send the email message to Amazon's Simple Email Service.</summary>
		/// <param name="fromAddress">The sender's email address.</param>
		/// <param name="toAddress">
		///     A single email recipient. Use the <see cref="Email.AddToAddress" /> method for multiple recipients.
		/// </param>
		/// <param name="ccAddress">
		///     A single carbon copy recipient. Use the <see cref="Email.AddCcAddress" /> method for multiple recipients.
		/// </param>
		/// <param name="messageSubject">The email message subject.</param>
		/// <param name="messageBody">
		///     The body of the message, formatted in <c>HTML</c> if the <see cref="NAV.AWS.SES.Email.HTML" /> property
		///     is true, otherwise formatted as plain text .
		/// </param>
		/// <param name="html">
		///     <see langword="true" /> if html, <see langword="false" /> if plain text.
		/// </param>
		/// <returns>
		///     <see langword="true" /> if the message is sent successfully, <see langword="false" /> if it fails.
		/// </returns>
		public bool Send(
			string fromAddress, string toAddress, string ccAddress, string messageSubject, string messageBody,
			bool html)
		{
			return Send(fromAddress, toAddress, ccAddress, string.Empty, messageSubject, messageBody, html);
		}


		/// <summary>Send the email message to Amazon's Simple Email Service.</summary>
		/// <param name="fromAddress">The sender's email address.</param>
		/// <param name="toAddress">
		///     A single email recipient. Use the <see cref="Email.AddToAddress" /> method for multiple recipients.
		/// </param>
		/// <param name="ccAddress">
		///     A single carbon copy recipient. Use the <see cref="Email.AddCcAddress" /> method for multiple
		///     recipients.
		/// </param>
		/// <param name="bccAddress">The Bcc address.</param>
		/// <param name="messageSubject">The email message subject.</param>
		/// <param name="messageBody">
		///     The body of the message, formatted in <c>HTML</c> if the <see cref="NAV.AWS.SES.Email.HTML" />
		///     property is true, otherwise formatted as plain text .
		/// </param>
		/// <param name="html">
		///     <see langword="true" /> if html, <see langword="false" /> if plain text.
		/// </param>
		/// <returns>
		///     <see langword="true" /> if the message is sent successfully, <see langword="false" /> if it fails.
		/// </returns>
		public bool Send(
			string fromAddress, string toAddress, string ccAddress, string bccAddress, string messageSubject,
			string messageBody, bool html)
		{
			FromAddress = fromAddress;
			AddToAddress(toAddress);
			AddCcAddress(ccAddress);
			AddBccAddress(bccAddress);
			MessageSubject = messageSubject;
			MessageBody = messageBody;
			HTML = html;
			return Send();
		}


		/// <summary>
		///     Sets an error <paramref name="message" /> and returns false to cascade the return value from the calling method.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <returns>Always returns false, which can be used to cascade the return value from the calling method .</returns>
		internal bool SetErrorMessage(string message)
		{
			ErrorMessage = message;
			return false;
		}


		/// <summary>Validates the email message contains the minimum amount of data to succeed.</summary>
		/// <returns>
		///     <see langword="true" /> if the message contains the minimum amount of data needed to send successfully,
		///     <see
		///         langword="false" />
		///     if it does not.
		/// </returns>
		internal bool Validate()
		{
			if (string.IsNullOrWhiteSpace(MessageBody))
				return SetErrorMessage(string.Format(TheValueCannotBeEmpty, "Message Body"));
			if (string.IsNullOrWhiteSpace(FromAddress))
				return SetErrorMessage(string.Format(TheValueCannotBeEmpty, "Send From Address"));
			if (ToAddressList.TrueForAll(string.IsNullOrWhiteSpace))
				return SetErrorMessage(string.Format(AtLeastOneValueMustBeSpecified, "Send To Address"));
			return true;
		}
	}
}
