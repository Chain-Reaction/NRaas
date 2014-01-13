using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System.Collections.Generic;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupBrokenSkills : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupBrokenSkills");

            Dictionary<ulong,SimDescription> sims = SimListing.GetResidents(true);
            foreach (SimDescription sim in sims.Values)
            {
                Corrections.CleanupBrokenSkills(sim, Overwatch.Log);
            }

            if (GameUtils.IsInstalled(ProductVersion.EP5))
            {
                if (SkillManager.sPetSkillFatigueTuning.Count == 0)
                {
                    SkillManager.ParsePetSkillFatigueRates(XmlDbData.ReadData("Skills"));

                    Overwatch.Log("Loaded Missing Pet Fatigue Rates");
                }
            }
        }
    }
}
