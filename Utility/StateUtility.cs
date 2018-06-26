using BaseGameLogic.States.Assembly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BaseGameLogic.States
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
        public static void GetAllRequiredReferences(object state, FieldInfo[] requiredFieldList,  GameObject parent = null, bool overrideReference = false)
        {
            //parent = parent == null ? GetRootTransform(this.transform).gameObject : parent;

            //requiredFieldList = requiredFieldList == null ? GetAllRequiredFields() : requiredFieldList;

            foreach (FieldInfo field in requiredFieldList)
                if (overrideReference || field.GetValue(state) == null)
                    field.SetValue(state, GetComponentDeep(parent, field.FieldType));
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