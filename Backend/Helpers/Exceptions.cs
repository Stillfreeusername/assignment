using System;

namespace Backend.Helpers.Exceptions
{
	[Serializable()]
	public class BackendException : Exception
	{
		public BackendException() : base() { }
		public BackendException(string message) : base(message) { }
		public BackendException(string message, Exception innerException) : base(message, innerException) { }

		protected BackendException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

		public virtual string ToSafeString()
		{
			return GetType().FullName;
		}
	}

	[Serializable()]
	public class InvalidIDException : BackendException
	{
		public InvalidIDException() : base() { }
		public InvalidIDException(string message) : base(message) { }
		public InvalidIDException(string message, Exception innerException) : base(message, innerException) { }

		protected InvalidIDException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
	public class InvalidSGTIN96_CompanyProduct_CombinationException : BackendException
	{
		public InvalidSGTIN96_CompanyProduct_CombinationException() : base() { }
		public InvalidSGTIN96_CompanyProduct_CombinationException(string message) : base(message) { }
		public InvalidSGTIN96_CompanyProduct_CombinationException(string message, Exception innerException) : base(message, innerException) { }

		protected InvalidSGTIN96_CompanyProduct_CombinationException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
	public class InvalidSGTIN96TagException : BackendException
	{
		public InvalidSGTIN96TagException() : base() { }
		public InvalidSGTIN96TagException(string message) : base(message) { }
		public InvalidSGTIN96TagException(string message, Exception innerException) : base(message, innerException) { }

		protected InvalidSGTIN96TagException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
	public class CompanyNotFoundException : BackendException
	{
		public CompanyNotFoundException() : base() { }
		public CompanyNotFoundException(string message) : base(message) { }
		public CompanyNotFoundException(string message, Exception innerException) : base(message, innerException) { }

		protected CompanyNotFoundException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
	public class ProductNotFoundException : BackendException
	{
		public ProductNotFoundException() : base() { }
		public ProductNotFoundException(string message) : base(message) { }
		public ProductNotFoundException(string message, Exception innerException) : base(message, innerException) { }

		protected ProductNotFoundException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

	/* ----------------- database related exceptions ----------------- */

	[Serializable()]
	public class ContainerCastException : BackendException
	{
		public ContainerCastException() : base() { }
		public ContainerCastException(string message) : base(message) { }
		public ContainerCastException(string message, Exception innerException) : base(message, innerException) { }

		protected ContainerCastException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
	public class DBItemReadException : BackendException
	{
		public DBItemReadException() : base() { }
		public DBItemReadException(string message) : base(message) { }
		public DBItemReadException(string message, Exception innerException) : base(message, innerException) { }

		protected DBItemReadException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
	public class DBItemCreateException : BackendException
	{
		public DBItemCreateException() : base() { }
		public DBItemCreateException(string message) : base(message) { }
		public DBItemCreateException(string message, Exception innerException) : base(message, innerException) { }

		protected DBItemCreateException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
	public class DBItemUpsertException : BackendException
	{
		public DBItemUpsertException() : base() { }
		public DBItemUpsertException(string message) : base(message) { }
		public DBItemUpsertException(string message, Exception innerException) : base(message, innerException) { }

		protected DBItemUpsertException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
	public class DBItemReplaceException : BackendException
	{
		public DBItemReplaceException() : base() { }
		public DBItemReplaceException(string message) : base(message) { }
		public DBItemReplaceException(string message, Exception innerException) : base(message, innerException) { }

		protected DBItemReplaceException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
	public class DBItemDeleteException : BackendException
	{
		public DBItemDeleteException() : base() { }
		public DBItemDeleteException(string message) : base(message) { }
		public DBItemDeleteException(string message, Exception innerException) : base(message, innerException) { }

		protected DBItemDeleteException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}