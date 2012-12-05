using System;

namespace NAV.AWS
{
	/// <summary>
	/// A base class used to encapsulate AWS errors.
	/// </summary>
	public class Error
	{
		/// <summary>
		/// A string indicating the name of the service that generated the error.
		/// </summary>
		private string _service = "<unknown>";


		/// <summary>
		/// A string indicating the error code.
		/// </summary>
		private string _code = "<unknown>";


		/// <summary>
		/// A string containing the error message text.
		/// </summary>
		private string _messageText = "<unknown>";


		/// <summary>
		/// A string containing information about whether the sender faulted.
		/// </summary>
		private string _senderFault = "<unknown>";


		/// <summary>
		///  A string indicating the name of the service that generated the error.
		/// </summary>
		public string Service
		{
			get { return _service; }
			set { _service = value; }
		}


		/// <summary>
		/// A string indicating the error code.
		/// </summary>
		public string Code
		{
			get { return _code; }
			set { _code = value; }
		}


		/// <summary>
		///  A string containing the error message text.
		/// </summary>
		public string MessageText
		{
			get { return _messageText; }
			set { _messageText = value; }
		}


		/// <summary>
		/// A string containing information about whether the sender faulted.
		/// </summary>
		public string SenderFault
		{
			get { return _senderFault; }
			set { _senderFault = value; }
		}


		/// <summary>
		/// Represents the class's property values in a descriptive string.
		/// </summary>
		/// <returns>A string containing the class's property values.</returns>
		public override string ToString()
		{
			return string.Format(
				"Error Code: {0} on Service {1}\n\nMessage: {2}\n\nSenderFault: {3}",
				Code, Service, MessageText, SenderFault);
		}
	}
}