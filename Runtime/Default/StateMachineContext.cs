using System.Collections.Generic;
using States.Core;
using UnityEngine;

namespace States.Default
{
	public class StateMachineContext : MonoBehaviour
	{
		[SerializeField] private List<Context> m_context = new List<Context>();
		public IEnumerable<Context> Context => m_context;
	}
}
