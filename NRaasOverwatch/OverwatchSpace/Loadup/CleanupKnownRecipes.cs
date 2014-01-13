using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Metadata;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupKnownRecipes : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupKnownRecipes");

            foreach(SimDescription sim in Household.EverySimDescription())
            {
                if (sim.SkillManager == null) continue;

                Cooking skill = sim.SkillManager.GetSkill<Cooking>(SkillNames.Cooking);
                if (skill == null) continue;

                if (skill.KnownRecipes == null) continue;

                foreach (string recipe in new List<string>(skill.KnownRecipes))
                {
                    if (!Recipe.NameToRecipeHash.ContainsKey(recipe))
                    {
                        skill.KnownRecipes.Remove(recipe);

                        Overwatch.Log(" Removed " + recipe + ": " + sim.FullName);
                    }
                }
            }
        }
    }
}
