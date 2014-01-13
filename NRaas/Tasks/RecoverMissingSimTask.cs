using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Tasks
{
    public class RecoverMissingSimTask : Common.FunctionTask
    {
        SimDescription mSim;

        bool mIgnorePlaceholders;

        public RecoverMissingSimTask(SimDescription sim, bool ignorePlaceholders)
        {
            mSim = sim;
            mIgnorePlaceholders = ignorePlaceholders;
        }

        protected override void OnPerform()
        {
            Logger.Append(Perform(mSim, mIgnorePlaceholders));
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

        public static bool Allowed(SimDescription sim, bool ignorePlaceholders, ref GreyedOutTooltipCallback callback)
        {
            if ((!ignorePlaceholders) && (FindPlaceholderForSim(sim) != null))
            {
                callback = Common.DebugTooltip("In Placeholder");
                return false;
            }
            else if (sim.IsEnrolledInBoardingSchool())
            {
                callback = Common.DebugTooltip("Boarding School");
                return false;
            }
            else if ((ParentsLeavingTownSituation.Adults != null) && (ParentsLeavingTownSituation.Adults.Contains(sim.SimDescriptionId)))
            {
                callback = Common.DebugTooltip("Free Vacation");
                return false;
            }
            else if (GameStates.IsOnVacation)
            {
                if (GameStates.sTravelData.mEarlyDepartureIds != null) 
                {
                    if (GameStates.sTravelData.mEarlyDepartureIds.Contains(sim.SimDescriptionId))
                    {
                        callback = Common.DebugTooltip("Early Departure A");
                        return false;
                    }
                }

                if (GameStates.sTravelData.mEarlyDepartures != null)
                {
                    foreach (Sim member in GameStates.sTravelData.mEarlyDepartures)
                    {
                        if (member.SimDescription == sim)
                        {
                            callback = Common.DebugTooltip("Early Departure B");
                            return false;
                        }
                    }
                }
            }
            else if (SimTypes.IsAwayOnSimPort(sim))
            {
                callback = Common.DebugTooltip("Simport");
                return false;
            }

            return true;
        }

        public static string Perform(SimDescription sim, bool ignorePlaceholders)
        {
            GreyedOutTooltipCallback callback = null;
            if (!Allowed(sim, ignorePlaceholders, ref callback)) return null;

            if (Instantiation.Perform(sim, ResetSimTask.Perform) != null)
            {
                return Common.NewLine + sim.FullName;
            }

            return null;
        }

        public class Logger : Common.Logger<Logger>
        {
            readonly static Logger sLogger = new Logger();

            public static void Append(string msg)
            {
                sLogger.PrivateAppend(msg);
            }

            protected override string Name
            {
                get { return "Recover Missing Logs"; }
            }

            protected override Logger Value
            {
                get { return sLogger; }
            }
        }
    }
}
