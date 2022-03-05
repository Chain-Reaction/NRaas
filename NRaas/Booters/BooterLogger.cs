using Sims3.Gameplay.Utilities;

namespace NRaas.CommonSpace.Booters
{
    public class BooterLogger : Common.TraceLogger<BooterLogger>
    {
        readonly static BooterLogger sLogger = new BooterLogger();

        public static void AddTrace(string msg)
        {            
            sLogger.PrivateAddTrace(msg);
        }
        public static void AddError(string msg)
        {            
            sLogger.PrivateAddError(msg);
        }

        public static bool Exists(XmlDbRow row, string key, string name)
        {
            if (row.Exists(key)) return true;
            
            AddError(name + " " + key + " Missing");
            return false;
        }

        protected override string Name
        {
            get { return "Messages"; }
        }

        protected override BooterLogger Value
        {
            get { return sLogger; }
        }
    }
}
