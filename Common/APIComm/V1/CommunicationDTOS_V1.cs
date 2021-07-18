using System.Collections.Generic;

using Newtonsoft.Json;

namespace Assignment.APIComm.V1
{
	[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
	public abstract class CommunicationDTOBase
	{
		// ErrorMessageBinder are just to show how to assign error message keys which automate access to localization dictionary

		//[ErrorMessageBinder("CommDTOError_OtherError")]
		public bool? OtherError { get; set; }

		//[ErrorMessageBinder("CommDTOError_NetworkError")]
		public bool? NetworkError { get; set; }

		//[ErrorMessageBinder("CommDTOError_AuthorizationError")]
		public bool? AuthorizationError { get; set; }

		//[ErrorMessageBinder("CommDTOError_DatabaseOCCError")]
		public bool? DatabaseOCCError { get; set; }

		public bool Completed { get; set; }
	}
}