using System;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NAV.AWS.Tests
{
	/// <summary>
	/// Tests for common AWS functions and static members for use across all tests in the assembly
	/// </summary>
	[TestClass]
	public class AWS
	{
		/// <summary>
		///     The field use by the <see cref="NAV.AWS.Credentials" /> property.
		/// </summary>
		private static Credentials _credentials;


		/// <summary>
		///     A <see langword="static" /> TestContext object for use throughout the test class.
		/// </summary>
		/// <value>
		///     The test context.
		/// </value>
		public static TestContext CommonTestContext { get; set; }


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
						using (var keyFile = new BinaryReader(File.OpenRead(CommonTestContext.Properties["KeyFile"].ToString())))
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
		///     <see cref="Initialize" /> the test class and add the AWS key file to the TestContext properties dictionary.
		/// </summary>
		/// <remarks>
		///     For testing purposes, I am saving the access key and secret key in a file on the host machine. The first 20
		///     characters contain the access key, and the next 40 characters contain the secret key.
		/// </remarks>
		/// <param name="context">The context.</param>
		[AssemblyInitialize]
		public static void Initialize(TestContext context)
		{
			CommonTestContext = context;
			CommonTestContext.Properties.Add("KeyFile", @"C:\AWS\Key.dat");
		}


		/// <summary>
		///     Clean up and dispose objects after the tests are finished.
		/// </summary>
		[AssemblyCleanup]
		public static void Cleanup()
		{
			if (_credentials != null)
				_credentials.Dispose();
		}


		/// <summary>
		/// Test that the AWS credentials work properly.
		/// </summary>
		[TestMethod]
		public void VerifyCredentials()
		{
			Assert.IsNotNull(Credentials);
		}
	}
}
