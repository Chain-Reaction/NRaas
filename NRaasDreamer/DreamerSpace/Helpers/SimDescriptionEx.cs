using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DreamerSpace.Helpers
{
    public class SimDescriptionEx
    {
        public static void IncrementLifetimeHappiness(SimDescription ths, float delta)
        {
            if (ths.CreatedSim.BuffManager.HasElement(BuffNames.HappilyEverAfter))
            {
                delta *= SimDescription.kHappilyEverAfterMultiple;
            }

            if ((!Sims3.SimIFace.Environment.HasEditInGameModeSwitch) && (delta > 0f))
            {
                ths.mLifetimeHappiness += delta;
                ths.mSpendableHappiness += delta;

                if (ths.OnLifetimeHappinessChanged != null)
                {
                    ths.OnLifetimeHappinessChanged();
                }

                if ((ths.Household != null) && (ths.mLifetimeHappiness >= SimDescription.kLifetimeHappinessRewardThreshold) && !ths.Household.LifetimeHappinessNotificationShown)
                {
                    if (ths.CreatedSim != null)
                    {
                        ths.CreatedSim.ShowTNSIfSelectable(SimDescription.LocalizeString("LifetimeHappinessNotification", new object[] { ths }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, ths.CreatedSim.ObjectId);
                    }
                    ths.Household.LifetimeHappinessNotificationShown = true;
                }
            }
        }
    }
}
