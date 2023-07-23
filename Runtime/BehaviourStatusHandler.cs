﻿using System;
using UnityEngine;

namespace Stages
{
	[Serializable, Obsolete("BehaviourStatusHandler is obsolete! Use GenericStatusHandler!")]
	public class BehaviourStatusHandler : StatusHandler<Behaviour>
	{
		public BehaviourStatusHandler(Behaviour managedObject, bool status) : base(managedObject, status) { }

		public override void Reset() => m_managedObject.enabled = !m_status;

		public override void Set() => m_managedObject.enabled = m_status;
	}
}