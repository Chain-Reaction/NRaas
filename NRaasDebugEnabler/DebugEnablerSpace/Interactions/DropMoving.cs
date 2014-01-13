using NRaas.CommonSpace.Interactions;
using NRaas.DebugEnablerSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Interactions
{
    public class DropMoving : DebugEnablerInteraction<GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if (obj is Sim)
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<DropMoving>
        {
            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("DropMoving:MenuName");
            }

            public override bool Test(IActor a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                if (!DebugEnabler.Settings.mEnabled) return false;

                if (!Sims3.Gameplay.Moving.MovingSituation.MovingInProgress) return false;

                return true;
            }
        }

        public override bool Run()
        {
            try
            {
                List<Situation> allSituationList = new List<Situation>(Situation.sAllSituations);
                foreach (Situation sit in allSituationList)
                {
                    Sims3.Gameplay.Moving.MovingSituation moving = sit as Sims3.Gameplay.Moving.MovingSituation;
                    if (moving == null) continue;

                    if (moving.Lot == LotManager.ActiveLot)
                    {
                        moving.Exit();
                    }
                }

                string failReason = null;
                if (Sims3.Gameplay.InWorldSubState.IsEditTownValid(LotManager.ActiveLot, ref failReason))
                {
                    SimpleMessageDialog.Show(Common.Localize("DropMoving:MenuName"), Common.Localize("DropMoving:Success"));
                }
                else
                {
                    SimpleMessageDialog.Show(Common.Localize("DropMoving:MenuName"), failReason);
                }
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }
    }
}