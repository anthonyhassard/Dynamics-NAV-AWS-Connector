using System;

namespace NAV.AWS.SQS
{
	/// <summary>
	/// A subclass of the <see cref="AWS.Error"/> class containing data specific to SQS errors.
	/// </summary>
	public class Error : AWS.Error
	{
		/// <summary>
		/// The Batch ID assigned to the message producing the error.
		/// </summary>
		public string BatchId { get; set; }


		/// <summary>
		/// Represents the class's property values in a descriptive string.
		/// </summary>
		/// <returns>A string containing the class's property values.</returns>
		public override string ToString()
		{
			return string.Format(
				"Error Code: {0} on Service {1}\nBatchId: {2}\n\nMessage: {3}\n\nSenderFault: {4}",
				Service, Code, BatchId, MessageText, SenderFault);
		}
	}
}

