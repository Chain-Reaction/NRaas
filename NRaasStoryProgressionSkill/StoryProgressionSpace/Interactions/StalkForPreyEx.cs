using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class StalkForPreyEx : CatHuntingComponent.StalkForPrey, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<GameObject, CatHuntingComponent.StalkForPrey.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.SkillThresholdValue = 0;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        private new class Definition : CatHuntingComponent.StalkForPrey.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new StalkForPreyEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (SimTypes.IsSelectable(a))
                {
                    if (a.SkillManager.GetSkillLevel(SkillNames.CatHunting) < 1)
                    {
                        return false;
                    }
                }

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}

