using System;
using System.IO;
using System.Linq;
using System.Security;
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
		///     The field use by the <see cref="NAV.AWS.Tests.SES.Credentials" /> property.
		/// </summary>
		private static Credentials _credentials;


		/// <summary>
		///     A <see langword="static" /> TestContext object for use throughout the test class.
		/// </summary>
		/// <value>
		///     The test context.
		/// </value>
		public static TestContext TestContext { get; set; }


		/// <summary>
		///     A <see langword="static" /> read-only <see cref="Credentials" /> object providing easy access to the AWS keys
		///     for all tests in the class.
		/// </summary>
		/// <value>
		///     The credentials.
		/// </value>
		public static Credentials Credentials
		{
			get
			{
				if (_credentials == null)
				{
					string accessKey = string.Empty;
					using (var secretKey = new SecureString())
					{
						using (var keyFile = new BinaryReader(File.OpenRead(TestContext.Properties["KeyFile"].ToString())))
						{
							accessKey = keyFile.ReadChars(20).Aggregate(accessKey, (current, c) => current + c);
							foreach (char c in keyFile.ReadChars(40))
								secretKey.AppendChar(c);
						}
						_credentials = new Credentials(accessKey, secretKey.Copy());
					}
				}
				return _credentials;
			}
		}


		/// <summary>
		///     Clean up and dispose objects after the tests are finished.
		/// </summary>
		[ClassCleanup]
		public static void Cleanup()
		{
			if (_credentials != null)
				_credentials.Dispose();
		}


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
			TestContext = context;
			TestContext.Properties.Add("KeyFile", @"C:\AWS\Key.dat");
			TestContext.Properties.Add("HTMLFile", @"C:\AWS\Email.html");
			TestContext.Properties.Add("AttachmentFile", @"C:\AWS\Sample.pdf");
		}


		/// <summary>
		///     Send an HTML email to the Amazon Simple <see cref="Email" /> Service.
		/// </summary>
		[TestMethod]
		public void SendHtmlEmail()
		{
			var ses = new Email(Credentials);
			Assert.IsTrue(
				ses.Send(
					"anthonyhassard@yahoo.com",
					"anthonyhassard@yahoo.com",
					"Test HTML Message",
					File.ReadAllText(TestContext.Properties["HTMLFile"].ToString()),
					true));
			Assert.IsTrue(!string.IsNullOrWhiteSpace(ses.MessageId));
			Console.Out.WriteLine("Message ID: {0}", ses.MessageId);
		}


		/// <summary>
		///     Send a plain-text email to the Amazon Simple <see cref="Email" /> Service.
		/// </summary>
		[TestMethod]
		public void SendPlainTextEmail()
		{
			var ses = new Email(Credentials);
			Assert.IsTrue(
				ses.Send(
					"anthonyhassard@yahoo.com",
					"anthonyhassard@yahoo.com",
					"Test Message",
					"Sent from a test method in the NAV.AWS framework",
					false),
				ses.ErrorMessage);
			Assert.IsTrue(!string.IsNullOrWhiteSpace(ses.MessageId));
			Console.Out.WriteLine("Message ID: {0}", ses.MessageId);
		}


		/// <summary>
		///     Send an HTML email with a PDF attachment to the Amazon Simple <see cref="Email" /> Service.
		/// </summary>
		[TestMethod]
		public void SendRawEmail()
		{
			var ses = new Email(Credentials);
			ses.AddToAddress("anthonyhassard@yahoo.com");
			ses.FromAddress = "anthonyhassard@yahoo.com";
			ses.AttachmentFilePath = TestContext.Properties["AttachmentFile"].ToString();
			ses.HTML = true;
			ses.MessageBody = File.ReadAllText(TestContext.Properties["HTMLFile"].ToString());
			ses.MessageSubject = "Test HTML Message (With PDF Attachment)";
			Assert.IsTrue(ses.Send());
			Assert.IsTrue(!string.IsNullOrWhiteSpace(ses.MessageId));
			Console.Out.WriteLine("Message ID: {0}", ses.MessageId);
		}
	}
}
