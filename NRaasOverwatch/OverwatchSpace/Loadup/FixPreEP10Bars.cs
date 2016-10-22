using Sims3.Gameplay.Objects.Counters;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class FixPreEP10Bars : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("FixPreEP10Bars");
            

            foreach (BarAdvanced bar in Sims3.Gameplay.Queries.GetObjects<BarAdvanced>())
            {
                if (bar.InWorld && bar.mInteractions == null)
                {
                    bar.mInteractions = new List<BarAdvanced.BarInteraction>();

                    if (bar.LotCurrent == null) continue;
                    Overwatch.Log("Bar at " + bar.LotCurrent.Name + " fixed");
                }
            }
        }
    }
}
