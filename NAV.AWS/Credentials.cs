using System;
using System.Security;
using Amazon.Runtime;

namespace NAV.AWS
{
	/// <summary>
	///     A helper class intended to hide AWS Credential implementation details.
	/// </summary>
	public class Credentials : IDisposable
	{
		/// <summary>
		///     Object is disposed.
		/// </summary>
		private bool _disposed;


		/// <summary>
		///     Default constructor.
		/// </summary>
		public Credentials() { }


		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="accessKey">The access key.</param>
		/// <param name="secureKey">The secure key.</param>
		public Credentials(string accessKey, SecureString secureKey)
		{
			AccessKey = accessKey;
			SecureKey = secureKey;
		}


		/// <summary>
		///     Gets or sets the user's AWS account access key value.
		/// </summary>
		/// <value>
		///     The access key assigned to the user by AWS.
		/// </value>
		public string AccessKey { get; set; }


		/// <summary>
		///     Gets or sets the user's AWS account secure key value.
		/// </summary>
		/// <remarks>
		///     It's extremely important this <see langword="private" /> key value not be transmitted or stored in a manner
		///     which could allow unauthorized access to the value. Since strings are immutable, it's preferable to use a
		///     <see cref="NAV.AWS.Credentials.SecureKey" /> object and to dispose of it as soon as possible.
		/// </remarks>
		/// <value>
		///     The secure key assigned to the user by AWS.
		/// </value>
		public SecureString SecureKey { get; set; }


		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		/// <summary>
		///     <see cref="Dispose" /> object resources.
		/// </summary>
		/// <param name="disposing">.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (SecureKey != null)
						SecureKey.Dispose();
				}
				SecureKey = null;
				_disposed = true;
			}
		}


		/// <summary>
		///     Provides an instance of the <see cref="BasicAWSCredentials" /> class.
		/// </summary>
		/// <returns>
		///     A <see cref="BasicAWSCredentials" /> object.
		/// </returns>
		internal BasicAWSCredentials GetBasicAWSCredentials()
		{
			return new BasicAWSCredentials(AccessKey, SecureKey);
		}


		/// <summary>
		///     Provides an instance of the <see cref="ImmutableCredentials" /> class.
		/// </summary>
		/// <returns>
		///     An <see cref="ImmutableCredentials" /> object.
		/// </returns>
		internal ImmutableCredentials GetImmutableCredentials()
		{
			return new ImmutableCredentials(AccessKey, SecureKey, string.Empty);
		}
	}
}
