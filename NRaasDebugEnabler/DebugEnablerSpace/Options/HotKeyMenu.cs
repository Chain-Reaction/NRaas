using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.DebugEnablerSpace.Interactions;
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
    public class HotKeyMenu : OperationSettingOption<GameObject>, IObjectOption
    {
        GameObject mTarget;

        public override string GetTitlePrefix()
        {
            return "HotKey";
        }

        public override string Name
        {
            get
            {
                return Common.Localize("HotKeyMenu:MenuName");
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mTarget = parameters.mTarget;

            return DebugEnabler.Settings.mEnabled;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<InteractionObjectPair> interactions = DebugMenu.GetInteractions(parameters.mActor, parameters.mTarget, parameters.mHit);
            if (interactions == null) return OptionResult.Failure;

            List<Item> allChoices = new List<Item>();

            foreach (InteractionObjectPair pair in interactions)
            {
                string name = null;

                string[] path = pair.InteractionDefinition.GetPath(parameters.mActor.IsFemale);

                if (path != null)
                {
                    foreach (string entry in path)
                    {
                        name += entry + " \\ ";
                    }
                }

                name += pair.InteractionDefinition.GetInteractionName(parameters.mActor, parameters.mTarget, pair);

                allChoices.Add(new Item (pair.InteractionDefinition, name));
            }

            CommonSelection<Item>.Results choices = new CommonSelection<Item>(Common.Localize("HotKeyMenu:MenuName"), allChoices).SelectMultiple();
            if ((choices == null) || (choices.Count == 0)) return OptionResult.Failure;

            foreach(Item choice in choices)
            {
                if (DebugEnabler.Settings.mInteractions.ContainsKey(choice.Value.GetType()))
                {
                    DebugEnabler.Settings.mInteractions.Remove(choice.Value.GetType());
                }
                else
                {
                    DebugEnabler.Settings.mInteractions.Add(choice.Value.GetType(), true);
                }
            }

            return OptionResult.SuccessClose;
        }

        public class Item : ValueSettingOption<InteractionDefinition>
        {
            public Item(InteractionDefinition definition, string name)
                : base (definition, name, DebugEnabler.Settings.mInteractions.ContainsKey(definition.GetType()) ? 1 : 0)
            { }

            public override string DisplayKey
            {
                get { return "HotKey"; }
            }
        }
    }
}