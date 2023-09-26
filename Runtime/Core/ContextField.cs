using System;

namespace Utilities.States
{
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