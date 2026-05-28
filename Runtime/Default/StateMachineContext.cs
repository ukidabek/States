using System.Collections;
using System.Collections.Generic;
using States.Core;
using UnityEngine;

namespace States.Default
{
	public class StateMachineContext : MonoBehaviour, IEnumerable<Context>
	{
		[SerializeField] private List<Context> m_context = new List<Context>();
		public IReadOnlyList<Context> Context => m_context;
		
		public IEnumerator<Context> GetEnumerator() => m_context.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
