using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utilities.States
{
	public class ContextHandler
	{
		private HashSet<IState> m_handledStaticStates = new HashSet<IState>();

		private const BindingFlags Binding_Flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance;

		private bool ValidateType(Context context, MemberInfo member)
		{
			var contextType = context.Type;
			Type memberType = default;

			switch (member)
			{
				case FieldInfo fieldInfo:
					memberType = fieldInfo.FieldType;
					break;
				case PropertyInfo propertyInfo:
					memberType = propertyInfo.PropertyType;
					break;
			}

			if (memberType.IsInterface)
			{
				var interfaces = contextType.GetInterfaces();
				return interfaces.Contains(memberType);
			}

			return memberType.IsAssignableFrom(contextType);
		}

		public void FillState(IState state, IEnumerable<Context> contexts)
		{
			if (m_handledStaticStates.Contains(state)) return;

			if (state.IsStatic)
				m_handledStaticStates.Add(state);

			var contextDestinations = state.GetContextDestination();
			foreach (var logic in contextDestinations)
			{
				var logicType = logic.GetType();
				var members = logicType.GetMembers(Binding_Flags);

				foreach (var member in members)
				{
					var attribute = member.GetCustomAttribute<ContextField>();
					if (attribute == null) continue;

					Context context = default;
					var attributeID = attribute.ID;
					if (string.IsNullOrEmpty(attributeID))
					{
						context = contexts.FirstOrDefault(context => ValidateType(context, member));
						if (context == null)
							continue;
					}
					else
					{
						context = contexts.FirstOrDefault(context => ValidateType(context, member) && context.Id == attributeID);
						if (context == null)
							continue;
					}

					switch (member)
					{
						case FieldInfo fieldInfo:
							fieldInfo.SetValue(logic, context.Object);
							break;
						case PropertyInfo propertyInfo:
							propertyInfo.SetValue(logic, context.Object);
							break;
					}
				}
			}
		}
	}
}