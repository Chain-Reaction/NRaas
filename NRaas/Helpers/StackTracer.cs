using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public abstract class StackTracer
    {
        public delegate bool OnMatch(StackTrace trace, StackFrame frame);

        Dictionary<string, List<StackFrameBase>> mTests = new Dictionary<string, List<StackFrameBase>>();

        StringBuilder mTrace = new StringBuilder();

        public StackTracer()
        { }

        public virtual void Reset()
        {
            mTrace = new StringBuilder();
        }

        public void AddTest(Type declaringType, string methodPrefix, OnMatch func)
        {
            AddTest(new TypeStackFrame(declaringType, methodPrefix, func));
        }
        public void AddTest(string declaringType, string methodPrefix, OnMatch func)
        {
            AddTest(new NamedStackFrame(declaringType, methodPrefix, func));
        }
        public void AddTest(StackFrameBase test)
        {
            string typeName = test.TypeName;

            List<StackFrameBase> list;
            if (!mTests.TryGetValue(typeName, out list))
            {
                list = new List<StackFrameBase>();
                mTests.Add(typeName, list);
            }

            list.Add(test);
        }

        public string Trace
        {
            get { return mTrace.ToString(); }
        }

        public bool Perform()
        {
            Reset();

            StackTrace trace = new StackTrace(false);

            StackFrame[] frames= trace.GetFrames();
            for (int i=frames.Length-1; i>=0; i--)
            {
                StackFrame frame = frames[i];

                string typeName = frame.GetMethod().DeclaringType.FullName;

                mTrace.Append(Common.NewLine + typeName + " : " + frame.GetMethod());

                List<StackFrameBase> tests;
                if (mTests.TryGetValue("", out tests))
                {
                    if (Test(frame, trace, tests)) return true;
                }

                if (mTests.TryGetValue(typeName, out tests))
                {
                    if (Test(frame, trace, tests)) return true;
                }
            }

            return false;
        }

        protected bool Test(StackFrame frame, StackTrace trace, List<StackFrameBase> tests)
        {
            foreach (StackFrameBase test in tests)
            {
                if (test.Test(frame))
                {
                    mTrace.Append(Common.NewLine + " Matched: " + test.ToString());

                    if (test.Perform(trace, frame))
                    {
                        mTrace.Append(Common.NewLine + " Perform Success");
                        return true;
                    }
                }
            }

            return false;
        }

        public override string ToString()
        {
            return Trace;
        }

        public abstract class StackFrameBase
        {
            OnMatch mFunc;

            string mMethodPrefix;

            public StackFrameBase(string methodPrefix, OnMatch func)
            {
                mFunc = func;
                mMethodPrefix = methodPrefix;
            }

            public virtual bool Test(StackFrame frame)
            {
                return frame.GetMethod().ToString().StartsWith(mMethodPrefix);
            }

            public abstract string TypeName
            {
                get;
            }

            public bool Perform(StackTrace trace, StackFrame frame)
            {
                if (mFunc == null) return true;

                return mFunc(trace, frame);
            }

            public override string ToString()
            {
                return "Method: " + mMethodPrefix;
            }
        }

        public class TypeStackFrame : StackFrameBase
        {
            Type mDeclaringType;

            public TypeStackFrame(Type declaringType, string methodPrefix, OnMatch func)
                : base(methodPrefix, func)
            {
                mDeclaringType = declaringType;
            }

            public override string TypeName
            {
                get { return mDeclaringType.FullName; }
            }

            public override string ToString()
            {
                return "Type: " + mDeclaringType + Common.NewLine + base.ToString();
            }
        }

        public class NamedStackFrame : StackFrameBase
        {
            string mDeclaringType;

            public NamedStackFrame(string declaringType, string methodPrefix, OnMatch func)
                : base(methodPrefix, func)
            {
                mDeclaringType = declaringType;
            }

            public override string TypeName
            {
                get { return mDeclaringType; }
            }

            public override string ToString()
            {
                return "Type: " + mDeclaringType + Common.NewLine + base.ToString();
            }
        }
    }
}
