using UnityEngine;
using System;
using System.Reflection;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace BaseGameLogic.States.Utility
{
    public class StateUtility
    {

        public class Requirement
        {
            private object @object = null;
            public object Object { get { return @object; } }

            private FieldInfo _fieldInfo = null;
            public FieldInfo FieldInfo { get { return _fieldInfo; } }

            public Requirement(object @object, FieldInfo fieldInfo)
            {
                this.@object = @object;
                _fieldInfo = fieldInfo;
            }
        }

        private static Requirement[] GetRequirementsInList(IList list)
        {
            List<Requirement> Requirements = new List<Requirement>();

            for (int i = 0; i < list.Count; i++)
            {
                var fields = StatesAssemblyExtension.GetAllFieldsWithAttribute(list[i].GetType(), typeof(RequiredReferenceAttribute)).ToArray();
                for (int j = 0; j < fields.Length; j++)
                {
                    if (fields[j].FieldType.IsArray || fields[j].FieldType.GetInterfaces().Contains(typeof(IList)))
                        Requirements.AddRange(GetRequirementsInList(fields[j].GetValue(list[i]) as IList));
                    else
                        Requirements.Add(new Requirement(list[i], fields[j]));
                }

            }
            return Requirements.ToArray();
        }

        public static Requirement[] GetAllRequirements(object @object)
        {
            var fieldList = StatesAssemblyExtension.GetAllFieldsWithAttribute(@object.GetType(), typeof(RequiredReferenceAttribute));
            List<Requirement> requirement = new List<Requirement>();

            for (int i = 0; i < fieldList.Count; i++)
            {
                if (fieldList[i].FieldType.IsArray || fieldList[i].FieldType.GetInterfaces().Contains(typeof(IList)))
                    requirement.AddRange(GetRequirementsInList(fieldList[i].GetValue(@object) as IList));
                else
                    requirement.Add(new Requirement(@object, fieldList[i]));
            }

            return requirement.ToArray();
        }


        /// <summary>
        /// Collect information for all required references for state to work.
        /// </summary>
        /// <returns></returns>
        public static FieldInfo[] GetAllRequiredFields(object @object)
        {
            return StatesAssemblyExtension.GetAllFieldsWithAttribute(@object.GetType(), typeof(RequiredReferenceAttribute)).ToArray();
        }

        /// <summary>
        /// Get all references to fields marked with RequiredReference attribute 
        /// </summary>
        /// <param name="parent"></param>
        public static bool GetAllRequiredReferences(object @object, FieldInfo[] requiredFieldList, GameObject parent, bool overrideReference = false)
        {
            bool continsAllComponents = true;
            foreach (FieldInfo field in requiredFieldList)
            {
                if (overrideReference || field.GetValue(@object) == null)
                {
                    var component = GetComponentDeep(parent, field.FieldType);
                    if(component == null)
                    {
                        Debug.LogErrorFormat("Object {0} don't contain all required components type of {1} for {2}",
                            parent.name, field.FieldType.ToString(), @object.GetType().ToString());
                        if (continsAllComponents)
                            continsAllComponents = false;
                    }
                    else
                    {
                        field.SetValue(@object, component);
                    }
                }
            }

            return continsAllComponents;
        }

        public static bool SetField(object @object, GameObject parent, FieldInfo field, bool overrideReference = false)
        {
            if (overrideReference || field.GetValue(@object) == null)
            {
                var component = GetComponentDeep(parent, field.FieldType);
                if (component == null)
                {
                    Debug.LogErrorFormat("Object {0} don't contain all required components type of {1} for {2}",
                        parent.name, field.FieldType.ToString(), @object.GetType().ToString());
                    return false;
                }
                else
                {
                    field.SetValue(@object, component);
                }
            }

            return true;
        }

        public static bool GetAllRequiredReferences(Requirement[] requiredFieldList, GameObject parent, bool overrideReference = false)
        {
            bool continsAllComponents = true;
            for (int i = 0; i < requiredFieldList.Length; i++)
            {
                continsAllComponents = SetField(requiredFieldList[i].Object, parent, requiredFieldList[i].FieldInfo, overrideReference);
                if (!continsAllComponents)
                    break;
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