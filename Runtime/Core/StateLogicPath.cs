using System;

namespace States.Core
{
	public class StateLogicPath : Attribute
    {
        private string m_path = string.Empty;

		public StateLogicPath(string path)
		{
			this.m_path = path;
		}

		public string Path => m_path;
    }
}