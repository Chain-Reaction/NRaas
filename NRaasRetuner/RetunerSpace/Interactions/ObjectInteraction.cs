using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Interactions;
using NRaas.RetunerSpace.Options;
using NRaas.RetunerSpace.Options.ITUN;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RetunerSpace.Interactions
{
    public class ObjectInteraction : ListedInteraction<IObjectOption,GameObject>
    {
        static InteractionDefinition Singleton = new CommonDefinition<ObjectInteraction>();

        GameObject mTarget;

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<GameObject>(Singleton);
        }

        protected override bool Test(IActor actor, GameObject target, GameObjectHit hit, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (Common.IsRootMenuObject(target)) return false;

            if (!Retuner.Settings.mShowObjectMenu) return false;

            return base.Test(actor, target, hit, ref greyedOutTooltipCallback);
        }

        protected override OptionResult Perform(IActor actor, GameObject target, GameObjectHit hit)
        {
            mTarget = target;

            return new InteractionOptionList<IObjectOption, GameObject>.AllList(GetInteractionName(actor, target, hit), SingleSelection, GetOptions).Perform(new GameHitParameters<GameObject>(actor, target, hit));
        }

        protected List<IObjectOption> GetOptions(List<IObjectOption> options)
        {
            if (GameUtils.IsInstalled(ProductVersion.EP8))
            {
                List<IObjectOption> results = new List<IObjectOption>();

                if (Common.kDebugging)
                {
                    results.Add(new ObjectSeasonOption(new CurrentKey(), mTarget));
                }

                foreach (SettingsKey key in Retuner.Settings.Keys)
                {
                    results.Add(new ObjectSeasonOption(key, mTarget));
                }

                return results;
            }
            else
            {
                return ObjectSeasonOption.GetOptions(mTarget);
            }
        }
    }
}
