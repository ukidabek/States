using UnityEngine;
using System;
using System.Reflection;

using BaseGameLogic.States.Utility.Assembly;

namespace BaseGameLogic.States.Utility
{
    public class StateUtility
    {
        /// <summary>
        /// Collect information for all required references for state to work.
        /// </summary>
        /// <returns></returns>
        public static FieldInfo[] GetAllRequiredFields(object state)
        {
            return StatesAssemblyExtension.GetAllFieldsWithAttribute(state.GetType(), typeof(RequiredReferenceAttribute), true).ToArray();
        }

        /// <summary>
        /// Get all references to fields marked with RequiredReference attribute 
        /// </summary>
        /// <param name="parent"></param>
        public static bool GetAllRequiredReferences(object state, FieldInfo[] requiredFieldList,  GameObject parent, bool overrideReference = false)
        {
            bool continsAllComponents = true;
            foreach (FieldInfo field in requiredFieldList)
                if (overrideReference || field.GetValue(state) == null)
                {
                    var component = GetComponentDeep(parent, field.FieldType);
                    if(component == null)
                    {
                        Debug.LogErrorFormat("Object {0} don't contain all required components type of {1} for {2}",
                            parent.name, field.FieldType.ToString(), state.GetType().ToString());
                        if (continsAllComponents)
                            continsAllComponents = false;
                    }
                    else
                    {
                        field.SetValue(state, component);
                    }
                }

            return continsAllComponents;
        }

        protected static Component GetComponentDeep(GameObject gameObject, Type type, bool includeInactive = false)
        {
            Component component = gameObject.GetComponent(type);
            if (component != null)
                return component;

            component = gameObject.GetComponentInChildren(type, includeInactive);
            if (component != null)
                return component;

            component = gameObject.GetComponentInParent(type);
            return component;
        }

        /// <summary>
        /// Returns root transform of object under control of states.
        /// </summary>
        /// <param name="parent">Start transform position</param>
        /// <returns>Root transform</returns>
        public static Transform GetRootTransform(Transform parent)
        {
            if (parent.parent == null)
                return parent;
            else
                return GetRootTransform(parent.parent);
        }
    }
}