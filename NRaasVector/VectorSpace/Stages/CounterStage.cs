using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;

namespace NRaas.VectorSpace.Stages
{
    public abstract class CounterStage : HitMissStage
    {
        string mCounter;

        public CounterStage(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "Counter", Name))
            {
                mCounter = row.GetString("Counter");
            }
        }

        public string Counter
        {
            get { return mCounter; }
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Counter: " + mCounter;

            return result;
        }
    }
}
