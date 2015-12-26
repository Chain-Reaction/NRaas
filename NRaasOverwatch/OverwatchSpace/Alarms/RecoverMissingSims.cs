using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Alarms
{
    public class RecoverMissingSims : AlarmOption
    {        
        public RecoverMissingSims()
        { }

        public override string GetTitlePrefix()
        {
            return "RecoverMissingSims";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mRecoverMissingSims;
            }
            set
            {
                NRaas.Overwatch.Settings.mRecoverMissingSims = value;
            }
        }

        public static Sim.Placeholder FindPlaceholderForSim(SimDescription simDesc)
        {
            if (simDesc.LotHome != null)
            {
                foreach (Sim.Placeholder placeholder in simDesc.LotHome.GetObjects<Sim.Placeholder>())
                {
                    if (placeholder.SimDescription == simDesc)
                    {
                        return placeholder;
                    }
                }
            }
            return null;
        }

        public static string Perform(Household house, bool ignorePlaceholders)
        {
            if (house.LotHome == null) return null;

            string msg = null;

            if (Households.NumSims(house) != Households.AllSims(house).Count)
            {
                List<SimDescription> sims = new List<SimDescription>(Households.All(house));
                foreach (SimDescription description in sims)
                {
                    bool flag = true;
                    foreach (Sim sim in Households.AllSims(house))
                    {
                        if (sim.SimDescription == description)
                        {
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                    {
                        FixInvisibleTask.Perform(description, false);

                        msg += RecoverMissingSimTask.Perform(description, ignorePlaceholders);
                    }
                }
            }

            return msg;
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            try
            {
                Overwatch.Log("Recover Missing Sims");

                string msg = null;

                List<Sim> sims = new List<Sim>(LotManager.Actors);
                foreach (Sim sim in sims)
                {
                    SimDescription simDesc = sim.SimDescription;
                    if (simDesc == null) continue;

                    if ((!sim.SimDescription.IsValidDescription) || (sim.Household == null))
                    {
                        try
                        {
                            GreyedOutTooltipCallback callback = null;
                            if (RecoverMissingSimTask.Allowed(sim.SimDescription, false, ref callback))
                            {
                                simDesc.Fixup();

                                if (Instantiation.AttemptToPutInSafeLocation(sim, true))
                                {
                                    sim.Autonomy.Motives.RecreateMotives(sim);
                                    sim.SetObjectToReset();
                                }

                                msg += Common.NewLine + simDesc.FullName;
                            }
                        }
                        catch(Exception e)
                        {
                            Common.Exception(simDesc, e);
                        }
                    }
                }

                List<Household> houses = new List<Household>(Household.sHouseholdList);
                foreach (Household house in houses)
                {
                    msg += Perform(house, prompt);
                }                

                if ((msg != null) && (msg != ""))
                {
                    Overwatch.AlarmNotify(Common.Localize("RecoverMissingSims:Success", false, new object[] { msg }));
                }
            }
            catch (Exception exception)
            {
                Common.Exception(Name, exception);
            }
        }        
    }
}
