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
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Interactions
{
    public class DumpAllCASParts : DebugEnablerInteraction<GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if (obj is CityHall)
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        public override bool Run()
        {
            try
            {
                StringBuilder builder = new StringBuilder();

                PartSearch search = new PartSearch();
                foreach(CASPart part in search)
                {
                    builder.Append(Common.NewLine + "Key: " + part.Key);

                    builder.Append(Common.NewLine + " Body Type: " + part.BodyType);
                    builder.Append(Common.NewLine + " Age Gender: " + part.AgeGenderSpecies);

                    builder.Append(Common.NewLine + " Category: ");

                    foreach(OutfitCategories category in Enum.GetValues(typeof(OutfitCategories)))
                    {
                        if ((part.CategoryFlags & (uint)category) == (uint)category)
                        {
                            builder.Append(category + " ");
                        }
                    }
                    
                    foreach(OutfitCategoriesExtended category in Enum.GetValues(typeof(OutfitCategoriesExtended)))
                    {
                        if ((part.CategoryFlags & (uint)category) == (uint)category)
                        {
                            builder.Append(category + " ");
                        }
                    }
                }

                search.Reset();

                Common.WriteLog(builder.ToString());
                return true;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<DumpAllCASParts>
        {
            // Methods
            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("DumpAllCASParts:MenuName");
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { "Outfits..." };
            }
        }
    }
}