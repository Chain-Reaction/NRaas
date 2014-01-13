using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Alarms
{
    public class ResetCheck : AlarmOption, Common.IWorldQuit
    {
        static Dictionary<ulong, StuckSimData> sData = new Dictionary<ulong, StuckSimData>();

        public override string GetTitlePrefix()
        {
            return "ResetCheck";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mResetCheck;
            }
            set
            {
                NRaas.Overwatch.Settings.mResetCheck = value;
            }
        }

        public void OnWorldQuit()
        {
            sData.Clear();
        }

        protected static bool ValidAger(SimDescription sim)
        {
            if (AgingManager.Singleton == null) return false;

            if (!AgingManager.Singleton.Enabled) return false;

            if (SimTypes.IsTourist(sim)) return false;

            if (sim.Elder) return false;

            if (sim.IsPregnant) return false;

            if (!sim.AgingEnabled) return false;

            if (SimTypes.IsPassporter(sim)) return false;

            if (sim.Household == Household.ActiveHousehold) return false;

            return true;
        }

        protected static bool TestAging(SimDescription sim, StuckSimData data)
        {
            if (!ValidAger(sim))
            {
                data.mAgingSituation = false;
                data.mWasNegative = false;
                return false;
            }

            Sim createdSim = sim.CreatedSim;
            if ((createdSim != null) && (createdSim.GetSituationOfType<AgeUpNpcSituation>() != null))
            {
                if (data.mAgingSituation)
                {
                    return true;
                }

                data.mAgingSituation = true;
            }
            else
            {
                data.mAgingSituation = false;
            }

            int daysToTransition = (int)AgingManager.Singleton.AgingYearsToSimDays(AgingManager.GetMaximumAgingStageLength(sim));
            daysToTransition -= (int)AgingManager.Singleton.AgingYearsToSimDays(sim.AgingYearsSinceLastAgeTransition);

            if (daysToTransition < -1)
            {
                if (data.mWasNegative)
                {
                    return true;
                }

                data.mWasNegative = true;
            }
            else
            {
                data.mWasNegative = false;
            }

            return false;
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            Overwatch.Log(Name);

            foreach (Sim createdSim in new List<Sim> (LotManager.Actors))
            {
                if (createdSim.SimDescription == null) continue;

                StuckSimData other;
                if (!sData.TryGetValue(createdSim.SimDescription.SimDescriptionId, out other))
                {
                    other = new StuckSimData();

                    sData.Add(createdSim.SimDescription.SimDescriptionId, other);
                }

                try
                {
                    bool reset = false;

                    SimDescription sim = createdSim.SimDescription;

                    if (TestAging(sim, other))
                    {
                        reset = true;
                    }

                    if (reset)
                    {
                        string notice = Common.Localize(GetTitlePrefix() + ":" + other.Reason + "Two", sim.IsFemale, new object[] { sim });

                        if (other.mAgingSituation)
                        {
                            ResetSimTask.Perform(createdSim, true);
                            sData.Remove(sim.SimDescriptionId);                            
                        }
                        else
                        {
                            AgeTransitionTask.Perform(createdSim);
                        }

                        Overwatch.AlarmNotify(notice);

                        Overwatch.Log(notice);
                    }
                    else if (other.Valid)
                    {
                        if (Overwatch.Settings.mReportFirstResetCheck)
                        {
                            string notice = Common.Localize(GetTitlePrefix() + ":" + other.Reason + "One", sim.IsFemale, new object[] { sim });

                            Overwatch.Notify(createdSim, notice);

                            Overwatch.Log(notice);
                            Overwatch.Log(" Was Negative: " + other.mWasNegative);
                            Overwatch.Log(" Aging Situation: " + other.mAgingSituation);
                        }

                        if (other.mWasNegative)
                        {
                            AgeTransitionTask.Perform(createdSim);
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(createdSim, e);
                }
            }
        }

        protected class StuckSimData
        {
            public bool mWasNegative;

            public bool mAgingSituation;

            public bool mPushedAging;

            public StuckSimData()
            { }

            public bool Valid
            {
                get
                {
                    if (mWasNegative) return true;

                    if (mAgingSituation) return true;

                    return false;
                }
            }

            public string Reason
            {
                get
                {
                    if ((mAgingSituation) || (mWasNegative))
                    {
                        return "Aging";
                    }

                    return "Generic";
                }
            }

            public override string ToString()
            {
                string text = base.ToString();

                text += Common.NewLine + "Was Negative: " + mWasNegative;
                text += Common.NewLine + "Aging Situation: " + mAgingSituation;

                return text;
            }
        }
    }
}
