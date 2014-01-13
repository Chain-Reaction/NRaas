using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace
{
    public class GameObjectReference : List<KeyValuePair<object, FieldInfo>>
    {
        public GameObjectReference()
        {}

        public bool HasReferences
        {
            get { return (Count > 0); }
        }

        public class StackFrame
        {
            public readonly ReferenceWrapper mReference;

            public readonly FieldInfo mField;

            public readonly string mDepth;

            public StackFrame(ReferenceWrapper reference, FieldInfo field, string depth)
            {
                mReference = reference;
                mField = field;
                mDepth = depth;
            }
        }

        public List<KeyValuePair<object, FieldInfo>> GetFullReferenceList()
        {
            Dictionary<object, bool> processed = new Dictionary<object, bool>();

            List<KeyValuePair<object, FieldInfo>> results = new List<KeyValuePair<object, FieldInfo>>();

            foreach (KeyValuePair<object, FieldInfo> obj in this)
            {
                results.Add(obj);
            }

            int index = 0;
            while (index < results.Count)
            {
                KeyValuePair<object, FieldInfo> stack = results[index];
                index++;

                if (processed.ContainsKey(stack.Key)) continue;
                processed.Add(stack.Key, true);

                if (stack.Key is GameObject) continue;

                GameObjectReference reference = ObjectLookup.GetReference(new ReferenceWrapper(stack.Key));
                if (reference != null)
                {
                    foreach (KeyValuePair<object, FieldInfo> obj in reference)
                    {
                        results.Add(obj);
                    }
                }
            }

            return results;
        }

        public void LogReferences()
        {
            Dictionary<ReferenceWrapper, bool> processed = new Dictionary<ReferenceWrapper, bool>();

            List<StackFrame> references = new List<StackFrame>();

            foreach (KeyValuePair<object, FieldInfo> obj in this)
            {
                references.Add(new StackFrame(new ReferenceWrapper(obj.Key), obj.Value, ""));
            }

            int index = 0;
            while (index < references.Count)
            {
                StackFrame stack = references[index];
                index++;

                bool repeat = false;
                if (processed.ContainsKey(stack.mReference))
                {
                    repeat = true;
                }
                else
                {
                    processed.Add(stack.mReference, true);
                }

                GameObjectReference reference = ObjectLookup.GetReference(stack.mReference);

                ErrorTrap.LogCorrection("  Reference: " + stack.mDepth + stack.mReference.mObject.GetType() + " " + stack.mField + (repeat ? " (Recursion)" : ""));

                if (stack.mReference.mObject is GameObject) continue;

                if (repeat) continue;

                if (reference != null)
                {
                    foreach (KeyValuePair<object, FieldInfo> obj in reference)
                    {
                        references.Insert(index, new StackFrame(new ReferenceWrapper(obj.Key), obj.Value, stack.mDepth + " "));
                    }
                }
            }
        }
    }
}
