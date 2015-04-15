/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */



namespace Com.Brashmonkey.Spriter.xml
{
	/// <summary>Indicates an error during serialization due to misconfiguration or during deserialization due to invalid input data.
	/// 	</summary>
	/// <remarks>Indicates an error during serialization due to misconfiguration or during deserialization due to invalid input data.
	/// 	</remarks>
	/// <author>Nathan Sweet</author>
	[System.Serializable]
	public class SerializationException : System.Exception
	{
		private const long serialVersionUID = 7695152523103193944L;

		private System.Text.StringBuilder trace;

		public SerializationException() : base()
		{
		}

		public SerializationException(string message, System.Exception cause) : base(message
			, cause)
		{
		}

		public SerializationException(string message) : base(message)
		{
		}

		public SerializationException(System.Exception cause) : base(string.Empty, cause)
		{
		}

		/// <summary>Returns true if any of the exceptions that caused this exception are of the specified type.
		/// 	</summary>
		/// <remarks>Returns true if any of the exceptions that caused this exception are of the specified type.
		/// 	</remarks>
		/*public virtual bool causedBy(java.lang.Class type)
		{
			return causedBy(this, type);
		}*/

		/*private bool causedBy(System.Exception ex, java.lang.Class type)
		{
			System.Exception cause = ex.InnerException;
			if (cause == null || cause == ex)
			{
				return false;
			}
			if (type.isAssignableFrom(Sharpen.Runtime.GetClassForObject(cause)))
			{
				return true;
			}
			return causedBy(cause, type);
		}*/

		public override string Message
		{
			get
			{
				if (trace == null)
				{
					return base.Message;
				}
				System.Text.StringBuilder buffer = new System.Text.StringBuilder(512);
				buffer.Append(base.Message);
				if (buffer.Length > 0)
				{
					buffer.Append('\n');
				}
				buffer.Append("Serialization trace:");
				buffer.Append(trace);
				return buffer.ToString();
			}
		}

		/// <summary>Adds information to the exception message about where in the the object graph serialization failure occurred.
		/// 	</summary>
		/// <remarks>
		/// Adds information to the exception message about where in the the object graph serialization failure occurred. Serializers
		/// can catch
		/// <see cref="SerializationException">SerializationException</see>
		/// , add trace information, and rethrow the exception.
		/// </remarks>
		public virtual void addTrace(string info)
		{
			if (info == null)
			{
				throw new System.ArgumentException("info cannot be null.");
			}
			if (trace == null)
			{
				trace = new System.Text.StringBuilder(512);
			}
			trace.Append('\n');
			trace.Append(info);
		}
	}
}
