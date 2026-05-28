using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace States.Core
{
	public class ContextHandler
	{
		private const BindingFlags Binding_Flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance;
		private static readonly Dictionary<Type, MemberInfo[]> ContextFieldsByTypeDictionary = new(200);
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void GenerateStateLogicCache()
		{
			ContextFieldsByTypeDictionary.Clear();
			
			var interfaceType = typeof(IContextDestination);
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
		
		private HashSet<IState> m_staticStates = new HashSet<IState>();
		
		public ContextHandler()
		{
		}

		public ContextHandler(IEnumerable<IState> staticStates)
		{
			foreach (var state in staticStates)
				m_staticStates.Add(state);
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

		private IEnumerable<MemberInfo> GetMemberInfos(Type logicType)
		{
			IEnumerable<MemberInfo> members = default;
			if (ContextFieldsByTypeDictionary.TryGetValue(logicType, out var memberInfos))
				members = memberInfos;
			else
			{
				members = logicType
					.GetMembers(Binding_Flags)
					.Where(member => member.GetCustomAttribute<ContextField>() != null);
			}

			return members;
		}
		
		public void FillState(IState state, IEnumerable<Context> contexts)
		{
			if (m_staticStates.Contains(state)) return;

			if (state.IsStatic) m_staticStates.Add(state);

			var contextDestinations = state.GetContextDestination();
			foreach (var logic in contextDestinations)
			{
				var logicType = logic.GetType();

				var members = GetMemberInfos(logicType);

				foreach (var member in members)
				{
					var attribute = member.GetCustomAttribute<ContextField>();
					
					if (attribute == null) continue;
					
					Context context = default;
					var attributeID = attribute.ID;
					context = string.IsNullOrEmpty(attributeID) ? 
						contexts.FirstOrDefault(context => ValidateType(context, member)) : 
						contexts.FirstOrDefault(context => ValidateType(context, member) && context.Id == attributeID);

					if (context == null) continue;

					InjectReference(member, logic, context.Object);
				}
			}
		}
		
		public void ClearState(IState state)
		{
			var contextDestinations = state.GetContextDestination();
			foreach (var logic in contextDestinations)
			{
				var logicType = logic.GetType();

				var members = GetMemberInfos(logicType);
				foreach (var member in members)
				{
					var attribute = member.GetCustomAttribute<ContextField>();
					
					if (attribute == null) continue;
					ClearReference(member, logic);
				}
			}

			m_staticStates.Remove(state);
		}

		private void InjectReference(MemberInfo member, object target, object value)
		{
			switch (member)
			{
				case FieldInfo fieldInfo:
					fieldInfo.SetValue(target, value);
					break;
				case PropertyInfo propertyInfo:
					propertyInfo.SetValue(target, value);
					break;
			}
		}
		
		private void ClearReference(MemberInfo member, object target)
		{
			object GetDefaultValue(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;
			
			switch (member)
			{
				case FieldInfo fieldInfo:
					fieldInfo.SetValue(target, GetDefaultValue(fieldInfo.FieldType));
					break;
				case PropertyInfo propertyInfo:
					propertyInfo.SetValue(target, GetDefaultValue(propertyInfo.PropertyType));
					break;
			}
		}
	}
}