using System;

using System.Collections.Generic;
using System.Net;

using Backend.Helpers.Exceptions;

namespace Backend.Helpers.Extensions
{
	public static partial class BackendExtensions
	{
		public static string ToSafeString(this Exception exception)
		{
			return exception.GetType().FullName;
		}

		public static T EvalDBRead<T>(this Microsoft.Azure.Cosmos.ItemResponse<T> response) where T : class
		{
			if ((int)response.StatusCode >= 200 && (int)response.StatusCode <= 299)
				return response.Resource;
			else
				throw new DBItemReadException();
		}

		public static T EvalDBCreate<T>(this Microsoft.Azure.Cosmos.ItemResponse<T> response) where T : class
		{
			if ((int)response.StatusCode >= 200 && (int)response.StatusCode <= 299)
				return response.Resource;
			else
				throw new DBItemCreateException();
		}

		public static T EvalDBInsert<T>(this Microsoft.Azure.Cosmos.ItemResponse<T> response) where T : class
		{
			if ((int)response.StatusCode >= 200 && (int)response.StatusCode <= 299)
				return response.Resource;
			else
				throw new DBItemCreateException();
		}

		public static T EvalDBUpsert<T>(this Microsoft.Azure.Cosmos.ItemResponse<T> response) where T : class
		{
			if ((int)response.StatusCode >= 200 && (int)response.StatusCode <= 299)
				return response.Resource;
			else
				throw new DBItemUpsertException();
		}

		public static T EvalDBReplace<T>(this Microsoft.Azure.Cosmos.ItemResponse<T> response) where T : class
		{
			if ((int)response.StatusCode >= 200 && (int)response.StatusCode <= 299)
				return response.Resource;
			else
				throw new DBItemReplaceException();
		}

		public static T EvalDBDelete<T>(this Microsoft.Azure.Cosmos.ItemResponse<T> response) where T : class
		{
			if ((int)response.StatusCode >= 200 && (int)response.StatusCode <= 299)
				return response.Resource;
			else
				throw new DBItemDeleteException();
		}
	}
}