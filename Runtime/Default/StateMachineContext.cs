﻿using System.Collections.Generic;
using UnityEngine;

namespace States.Core.Default
{
	public class StateMachineContext : MonoBehaviour
	{
		[SerializeField] private List<Context> m_context = new List<Context>();
		public IEnumerable<Context> Context => m_context;
	}
}
