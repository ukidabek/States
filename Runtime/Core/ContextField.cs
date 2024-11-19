using System;

namespace Utilities.States
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ContextField : Attribute
	{
		private string m_id = String.Empty;
		public string ID => m_id;

		public ContextField(string id = "")
		{
			m_id = id;
		}
	}
}