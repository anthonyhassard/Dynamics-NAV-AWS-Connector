using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAV.AWS.SES;

namespace NAV.AWS.Tests
{
	/// <summary>
	///     Tests for the Amazon Web Service Simple <see cref="Email" /> Service library.
	/// </summary>
	[TestClass]
	public class SES
	{
		/// <summary>
		///     <see cref="Initialize" /> the test class and add the AWS key file to the TestContext properties dictionary.
		/// </summary>
		/// <remarks>
		///     For testing purposes, I am saving the access key and secret key in a file on the host machine. The first 20
		///     characters contain the access key, and the next 40 characters contain the secret key.
		/// </remarks>
		/// <param name="context">The context.</param>
		[ClassInitialize]
		public static void Initialize(TestContext context)
		{
			_localTestContext = context;
			_localTestContext.Properties.Add("AttachmentFile", @"C:\AWS\Sample.pdf");
			_localTestContext.Properties.Add("HTMLFile", @"C:\AWS\Email.html");
		}


		/// <summary>
		///     A <see langword="static" /> TestContext object for use in just this test class.
		/// </summary>
		/// <value>
		///     The test context.
		/// </value>
		private static TestContext _localTestContext;


		/// <summary>
		///     Send an HTML email to the Amazon Simple <see cref="Email" /> Service.
		/// </summary>
		[TestMethod]
		public void SendHtmlEmail()
		{
			var ses = new Email(AWS.Credentials);
			Assert.IsTrue(
				ses.Send(
					@"anthonyhassard@yahoo.com",
					@"anthonyhassard@yahoo.com",
					"Test HTML Message",
					File.ReadAllText(_localTestContext.Properties["HTMLFile"].ToString()),
					true));
			Assert.IsTrue(!string.IsNullOrWhiteSpace(ses.MessageId));
			Console.Out.WriteLine("Message AwsAssignedId: {0}", ses.MessageId);
		}


		/// <summary>
		///     Send a plain-text email to the Amazon Simple <see cref="Email" /> Service.
		/// </summary>
		[TestMethod]
		public void SendPlainTextEmail()
		{
			var ses = new Email(AWS.Credentials);
			Assert.IsTrue(
				ses.Send(
					@"anthonyhassard@yahoo.com",
					@"anthonyhassard@yahoo.com",
					"Test Message",
					"Sent from a test method in the NAV.AWS framework",
					false),
				ses.ErrorMessage);
			Assert.IsTrue(!string.IsNullOrWhiteSpace(ses.MessageId));
			Console.Out.WriteLine("Message AwsAssignedId: {0}", ses.MessageId);
		}


		/// <summary>
		///     Send an HTML email with a PDF attachment to the Amazon Simple <see cref="Email" /> Service.
		/// </summary>
		[TestMethod]
		public void SendRawEmail()
		{
			var ses = new Email(AWS.Credentials);
			ses.AddToAddress(@"anthonyhassard@yahoo.com");
			ses.FromAddress = @"anthonyhassard@yahoo.com";
			ses.AttachmentFilePath = _localTestContext.Properties["AttachmentFile"].ToString();
			ses.HTML = true;
			ses.MessageBody = File.ReadAllText(_localTestContext.Properties["HTMLFile"].ToString());
			ses.MessageSubject = "Test HTML Message (With PDF Attachment)";
			Assert.IsTrue(ses.Send());
			Assert.IsTrue(!string.IsNullOrWhiteSpace(ses.MessageId));
			Console.Out.WriteLine("Message AwsAssignedId: {0}", ses.MessageId);
		}
	}
}
