using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Utilities.States.Default
{
	public class ExternalContext : MonoBehaviour
	{
		[SerializeField] private List<Context> m_context = new List<Context>();
		public ReadOnlyCollection<Context> Context { get; private set; }

		private void Awake()
		{
			Context = new ReadOnlyCollection<Context>(m_context);
		}
	}
}
