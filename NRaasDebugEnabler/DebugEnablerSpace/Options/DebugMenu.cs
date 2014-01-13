using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
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

namespace NRaas.DebugEnablerSpace.Options
{
    public class DebugMenu : OperationSettingOption<GameObject>, IObjectOption
    {
        GameObject mTarget;

        public override string GetTitlePrefix()
        {
            return "Options";
        }

        public override string Name
        {
            get
            {
                string name = null;

                if (mTarget != null)
                {
                    try
                    {
                        name = mTarget.CatalogName;
                    }
                    catch
                    { }
                }

                return Common.Localize("Options:MenuName", false, new object[] { name });
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mTarget = parameters.mTarget;

            return DebugEnabler.Settings.mEnabled;
        }

        protected static List<InteractionObjectPair> GetAllDebugInteractionsForActor(GameObject ths, IActor actor)
        {
            List<InteractionDefinition> debugInteractions = new List<InteractionDefinition>();
            ths.AddDebugInteractions(debugInteractions);
            if (debugInteractions.Count <= 0x0)
            {
                return new List<InteractionObjectPair>();
            }
            List<InteractionObjectPair> interactions = new List<InteractionObjectPair>();
            foreach (InteractionDefinition definition in debugInteractions)
            {
                interactions.Add(new InteractionObjectPair(definition, ths));
            }

            return BuildInteractions(ths, actor, interactions);
        }
        private static List<InteractionObjectPair> BuildInteractions(GameObject ths, IActor s, List<InteractionObjectPair> interactions)
        {
            List<InteractionObjectPair> results = new List<InteractionObjectPair>();
            foreach (InteractionObjectPair pair in interactions)
            {
                if ((s != null) || (pair.InteractionDefinition is IActorlessDefinition))
                {
                    try
                    {
                        pair.AddInteractions(s, results);
                    }
                    catch (Exception e)
                    {
                        Common.Exception(s, ths, e);
                    }
                }
            }
            return results;
        }

        public static List<InteractionObjectPair> GetInteractions(IActor actor, GameObject target, GameObjectHit hit)
        {
            List<InteractionObjectPair> interactions = null;

            try
            {
                interactions = target.GetAllDebugInteractionsForActor(actor);
                if (interactions == null) return null;
            }
            catch (Exception e)
            {
                interactions = GetAllDebugInteractionsForActor(target, actor);

                Common.Exception(actor, target, e);
            }

            List<InteractionObjectPair> additional = new List<InteractionObjectPair>();

            List<IAdjustInteraction> adjustInteractions = Common.DerivativeSearch.Find<IAdjustInteraction>();

            List<IAddInteractionPair> addInteractions = Common.DerivativeSearch.Find<IAddInteractionPair>();
            foreach (IAddInteractionPair interaction in addInteractions)
            {
                interaction.AddPair(target, additional);
            }

            foreach (InteractionObjectPair pair in additional)
            {
                pair.InteractionDefinition.AddInteractions(pair, actor, interactions);
            }

            int index = 0;
            while (index < interactions.Count)
            {
                InteractionObjectPair interaction = interactions[index];
                index++;

                foreach (IAdjustInteraction adjust in adjustInteractions)
                {
                    if (adjust.AdjustInteraction(interaction))
                    {
                        index--;
                        interactions.RemoveAt(index);
                    }
                }
            }

            return interactions;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<InteractionObjectPair> interactions = GetInteractions(parameters.mActor, parameters.mTarget, parameters.mHit);
            if (interactions == null) return OptionResult.Failure;

            return InteractionDefinitionOptionList.Perform(parameters.mActor, parameters.mHit, interactions);
        }
    }
}