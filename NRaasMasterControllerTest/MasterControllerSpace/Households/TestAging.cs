using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class TestAging : HouseholdFromList, IHouseholdOption
    {
        public override string GetTitlePrefix()
        {
            return "TestAging";
        }

        protected override OptionResult Run(Lot myLot, Household house)
        {
            try
            {
                AgingManager singleton = AgingManager.Singleton;

                string msg = null;

                foreach (SimDescription sim in Household.AllSimsLivingInWorld())
                {
                    msg += Common.NewLine + sim.FullName;
                    msg += Common.NewLine + " Aging Enabled: " + sim.AgingEnabled;

                    if (sim.AgingState == null)
                    {
                        msg += Common.NewLine + " No State";
                    }
                    else
                    {
                        msg += Common.NewLine + " Old Enough: " + singleton.SimIsOldEnoughToTransition(sim.AgingState);
                        msg += Common.NewLine + " DayPassed: " + sim.AgingState.DayPassedSinceLastTransition;
                        msg += Common.NewLine + " Stage: " + sim.Age;
                        msg += Common.NewLine + " Current: " + singleton.AgingYearsToSimDays(sim.AgingState.AgingYearsPassedSinceLastTransition);
                        msg += Common.NewLine + " Maximum: " + singleton.AgingYearsToSimDays(AgingManager.GetMaximumAgingStageLength(sim));

                        msg += Common.NewLine + " WithoutCake Alarm: " + sim.AgingState.AgeTransitionWithoutCakeAlarm;
                        msg += Common.NewLine + " EarlyMessage Alarm: " + sim.AgingState.AgeTransitionEarlyMessageAlarm;
                        msg += Common.NewLine + " EarlyMessage: " + sim.AgingState.AgeTransitionEarlyMessagesGiven;
                        msg += Common.NewLine + " Message Alarm: " + sim.AgingState.AgeTransitionMessageAlarm;
                        msg += Common.NewLine + " Message: " + sim.AgingState.AgeTransitionMessagesGiven;
                    }

                    msg += Common.NewLine;
                }

                Common.WriteLog(msg);
            }
            catch (Exception e)
            {
                Common.Exception(myLot, e);
            }
            return OptionResult.SuccessClose;
        }
    }
}
