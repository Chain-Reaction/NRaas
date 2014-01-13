using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Alarms
{
    public class RecoverStrandedToddlers : AlarmOption
    {
        public RecoverStrandedToddlers()
        { }

        public override string GetTitlePrefix()
        {
            return "RecoverStrandedToddlers";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return Overwatch.Settings.mRecoverStrandedToddlers;
            }
            set
            {
                Overwatch.Settings.mRecoverStrandedToddlers = value;
            }
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            try
            {
                Overwatch.Log("Recover Stranded Toddlers");

                string msg = null;

                List<Sim> sims = new List<Sim>(Sims3.Gameplay.Queries.GetObjects<Sim>());
                foreach (Sim sim in sims)
                {
                    if (sim.SimDescription == null) continue;

                    if (!sim.SimDescription.ToddlerOrBelow) continue;

                    if (sim.LotHome == null) continue;

                    if (sim.LotCurrent == sim.LotHome) continue;

                    if (sim.Posture is BeingCarriedPosture) continue;

                    if (DaycareWorkdaySituation.GetDaycareWorkdaySituationForLot(sim.LotCurrent) != null) continue;

                    bool guardian = false;
                    foreach (Sim member in Households.AllSims(sim.Household))
                    {
                        if ((member.SimDescription.TeenOrAbove) && (member.LotCurrent == sim.LotCurrent))
                        {
                            guardian = true;
                            break;
                        }
                    }

                    if (guardian) continue;

                    try
                    {
                        if (Instantiation.AttemptToPutInSafeLocation(sim, false))
                        {
                            SpeedTrap.Sleep();
                            msg += Common.NewLine + sim.Name;
                        }
                    }
                    catch
                    { }
                }

                if (msg != null)
                {
                    Overwatch.AlarmNotify(Common.Localize("RecoverStrandedToddlers:Success", false, new object[] { msg }));
                }
            }
            catch (Exception exception)
            {
                Common.Exception(Name, exception);
            }
        }
    }
}
