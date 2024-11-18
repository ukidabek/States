using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Utilities.States
{
	public class ContextHandler
	{
		private HashSet<IState> m_handledStaticStates = new HashSet<IState>();

		private const BindingFlags Binding_Flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance;
		private static readonly Dictionary<Type, MemberInfo[]> ContextFieldsByTypeDictionary = new();
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void GenerateStateLogicCache()
		{
			var interfaceType = typeof(IStateLogic);
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var types = assemblies.SelectMany(assembly => assembly.GetTypes())
				.Where(type => interfaceType.IsAssignableFrom(type) && !type.IsAbstract);

			foreach (var type in types)
			{
				var members = type.GetMembers(Binding_Flags);
				
				var contextMembers = members
					.Where(member =>member.GetCustomAttribute<ContextField>() != null)
					.ToArray();
				
				if (!contextMembers.Any()) continue;
				ContextFieldsByTypeDictionary.Add(type, contextMembers);
			}
		}
		
		private bool ValidateType(Context context, MemberInfo member)
		{
			var contextType = context.Type;
			var memberType = member switch
			{
				FieldInfo fieldInfo => fieldInfo.FieldType,
				PropertyInfo propertyInfo => propertyInfo.PropertyType,
				_ => default
			};

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

				IEnumerable<MemberInfo> members = default;
				if (ContextFieldsByTypeDictionary.TryGetValue(logicType, out var memberInfos))
				{
					members = memberInfos;
				}
				else
				{
					members = logicType.GetMembers(Binding_Flags)
						.Where(member => member.GetCustomAttribute<ContextField>() != null);
				}

				foreach (var member in members)
				{
					var attribute = member.GetCustomAttribute<ContextField>();
					if (attribute == null) continue;

					Context context = default;
					var attributeID = attribute.ID;
					context = string.IsNullOrEmpty(attributeID) ? 
						contexts.FirstOrDefault(context => ValidateType(context, member)) : 
						contexts.FirstOrDefault(context => ValidateType(context, member) && context.Id == attributeID);

					if (context == null)
						continue;

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