using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
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
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
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
    public class SetQuality : DebugEnablerInteraction<GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if ((obj is Ingredient) || (obj is Plant))
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        public override bool Run()
        {
            try
            {
                List<Item> choices = new List<Item>();

                foreach(Quality quality in Enum.GetValues(typeof(Quality)))
                {
                    if (quality == Quality.Any) continue;

                    choices.Add(new Item(quality));
                }

                Item choice = new CommonSelection<Item>(Common.Localize("SetQuality:MenuName"), choices).SelectSingle();
                if (choice == null) return true;

                int intQuality = (int)choice.Value;
                intQuality--;

                Ingredient ingredient = Target as Ingredient;
                if (ingredient != null)
                {
                    if (ingredient.Plantable != null)
                    {
                        ingredient.Plantable.QualityLevel = Plant.kQualityLevels[intQuality];
                    }
                    else
                    {
                        ingredient.SetQuality(choice.Value);
                    }
                }
                else
                {
                    Plant plant = Target as Plant;
                    if (plant != null)
                    {
                        plant.mQualityLevel = Plant.kQualityLevels[intQuality];
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
            return true;
        }

        [DoesntRequireTuning]
        private class Definition : DebugEnablerDefinition<SetQuality>
        {
            public Definition()
            { }

            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("SetQuality:MenuName");
            }
        }

        public class Item : ValueSettingOption<Quality>
        {
            public Item (Quality quality)
                : base(quality, QualityHelper.GetQualityLocalizedString(quality), 0)
            { }
        }
    }
}