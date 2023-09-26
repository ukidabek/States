using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.States
{
	[Serializable]
	public class Context
	{
		[SerializeField] private string m_id = string.Empty;
		public string Id => m_id;

		[SerializeField] private Object m_object = null;
		public Object Object => m_object;

		public Type Type => m_object.GetType();

		public Context() { }

		public Context(Object @object = null)
		{
			m_object = @object;
		}

		public Context(string id = "", Object @object = null)
		{
			m_id = string.IsNullOrEmpty(id) ? string.Empty : id;
			m_object = @object;
		}
	}
}