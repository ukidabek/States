﻿using System.Collections.Generic;
using UnityEngine;

namespace Utilities.States.Default
{
	public class ExternalContext : MonoBehaviour
	{
		[SerializeField] private List<Context> m_context = new List<Context>();
		public IEnumerable<Context> Context => m_context;
	}
}
