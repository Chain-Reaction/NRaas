using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Checks
{
    public interface ICheck
    {
        bool IPrePerform(IScriptLogic obj, bool postLoad);

        bool IPostPerform(IScriptLogic obj, bool postLoad);

        void Finish();
    }

    public abstract class Check<T> : ICheck
        where T : class
    {
        public bool IPrePerform(IScriptLogic obj, bool postLoad)
        {
            if (obj is T)
            {
                try
                {
                    return PrePerform(obj as T, postLoad);
                }
                catch (Exception e)
                {
                    Common.Exception(GetType().ToString(), e);
                }
            }

            return false;
        }

        public bool IPostPerform(IScriptLogic obj, bool postLoad)
        {
            if (obj is T)
            {
                try
                {
                    return PostPerform(obj as T, postLoad);
                }
                catch (Exception e)
                {
                    Common.Exception(GetType().ToString(), e);
                }
            }

            return false;
        }

        protected void DebugLogCorrection(string text)
        {
            ErrorTrap.DebugLogCorrection(text);
        }

        protected void LogCorrection(string text)
        {
            ErrorTrap.LogCorrection(text);
        }

        public bool Perform(T obj, bool postLoad)
        {
            if (!PrePerform(obj, postLoad)) return false;
            
            return PostPerform(obj, postLoad);
        }

        protected abstract bool PrePerform(T obj, bool postLoad);

        protected virtual bool PostPerform(T obj, bool postLoad)
        {
            return true;
        }

        public virtual void Finish()
        { }
    }
}
