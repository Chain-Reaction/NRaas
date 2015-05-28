using NRaas.CommonSpace.Helpers;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class FixLushEpidemic : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("FixLushEpidemic");

            if (GameUtils.IsInstalled(ProductVersion.EP9) && !GameUtils.IsUniversityWorld())
            {
                MotiveTuning mTuning = null;
                foreach (MotiveTuning tuning in MotiveTuning.GetAllTunings(CommodityKind.Juiced))
                {
                    MotiveSatisfactionCurve autoSatisfyCurve = new MotiveSatisfactionCurve
                    {
                        Loops = true
                    };
                    Curve curve = autoSatisfyCurve;
                    curve.Add(new Vector2(0f, -49f));
                    Curve motiveDecayCurve = autoSatisfyCurve.GetMotiveDecayCurve();
                    tuning.mMotiveDecayCurve = motiveDecayCurve;
                    tuning.mAutoSatisfyCurve = autoSatisfyCurve;
                    mTuning = tuning;
                }

                if (mTuning != null)
                {
                    foreach (Sim sim in LotManager.Actors)
                    {
                        if (sim.Autonomy.Motives.HasMotive(CommodityKind.Juiced))
                        {
                            sim.mMotiveTuning[(int)CommodityKind.Juiced] = mTuning;
                            sim.Autonomy.Motives.RemoveMotive(CommodityKind.Juiced);
                            sim.Autonomy.Motives.CreateMotive(CommodityKind.Juiced);
                        }
                    }
                }


                Overwatch.Log("Banished autosatisfy of Juiced motive");
            }
        }        
    }
}