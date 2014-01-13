using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupCollecting : DelayedLoadupOption
    {
        protected static void OnLearnedSkill(Event e)
        {
            HasGuidEvent<SkillNames> skillEvent = e as HasGuidEvent<SkillNames>;
            if (skillEvent == null) return;

            if (skillEvent.Guid != SkillNames.Collecting) return;

            EnactCorrections(skillEvent.HasGuidObject as Collecting);
        }

        protected static void EnactCorrections(Collecting skill)
        {
            Overwatch.Log(" Correcting: " + skill.SkillOwner.FullName);

            if (skill.mGlowBugData == null)
            {
                skill.mGlowBugData = new Dictionary<Sims3.Gameplay.Objects.Insect.InsectType, Collecting.GlowBugStats>();

                Overwatch.Log("  GlowBugData Fixed");
            }

            if (skill.mMushroomData == null)
            {
                skill.mMushroomData = new Dictionary<string, Collecting.MushroomStats>();

                Overwatch.Log("  MushroomData Fixed");
            }

            if (skill.mHarvestData == null)
            {
                skill.mHarvestData = new Dictionary<string, Collecting.HarvestStats>();

                Overwatch.Log("  HarvestData Fixed");
            }

            if (skill.mMetalData != null)
            {
                List<RockGemMetal> remove = new List<RockGemMetal>();

                foreach (KeyValuePair<RockGemMetal, Collecting.MetalStats> data in skill.mMetalData)
                {
                    if (data.Value.count == 0)
                    {
                        remove.Add(data.Key);
                    }
                }

                foreach (RockGemMetal metal in remove)
                {
                    skill.mMetalData.Remove(metal);
                }
            }

            Gardening gardening = skill.SkillOwner.SkillManager.GetSkill<Gardening>(SkillNames.Gardening);
            if ((gardening != null) && (gardening.mHarvestCounts.Count != 0))
            {
                bool success = false;

                foreach (KeyValuePair<string, Gardening.PlantInfo> plants in gardening.mHarvestCounts)
                {
                    string ingredient = null;
                    foreach (IngredientData choice in IngredientData.IngredientDataList)
                    {
                        if (choice.PlantName == plants.Key)
                        {
                            ingredient = choice.mStringKey;
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(ingredient)) continue;

                    Collecting.HarvestStats data = null;
                    if (!skill.mHarvestData.TryGetValue(ingredient, out data))
                    {
                        data = new Collecting.HarvestStats();
                        skill.mHarvestData.Add(ingredient, data);

                        data.quality = (int)plants.Value.BestQuality;
                        if (data.quality <= 0)
                        {
                            data.quality = (int)Quality.Bad;
                        }
                    }
                    else
                    {
                        if (data.quality < (int)plants.Value.BestQuality)
                        {
                            data.quality = (int)plants.Value.BestQuality;
                        }

                        if (data.count >= plants.Value.HarvestablesCount) continue;
                    }

                    if (data.mostExpensive < plants.Value.MostExpensive)
                    {
                        data.mostExpensive = plants.Value.MostExpensive;
                    }

                    if (data.count < plants.Value.HarvestablesCount)
                    {
                        data.count = plants.Value.HarvestablesCount;
                    }

                    success = true;
                }

                if (success)
                {
                    Overwatch.Log("  HarvestData Reconciled");
                }
            }
        }

        protected static ListenerAction OnGetNCutGems(Event e)
        {
            try
            {
                CutGemEvent gemEvent = e as CutGemEvent;
                if (gemEvent != null)
                {
                    Gem gem = gemEvent.TargetObject as Gem;
                    if (gem != null)
                    {
                        Collecting skill = e.Actor.SkillManager.AddElement(SkillNames.Collecting) as Collecting;
                        if (skill != null)
                        {
                            if (!skill.mGemData.ContainsKey(gem.Guid))
                            {
                                skill.mGemData.Add(gem.Guid, new Collecting.GemStats(0));
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception(e.Actor, e.TargetObject, exception);
            }

            return ListenerAction.Keep;
        }

        public override void OnDelayedWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kSkillLearnedSkill, OnLearnedSkill);

            // Must be immediate
            EventTracker.AddListener(EventTypeId.kGetNCutGems, OnGetNCutGems);

            Overwatch.Log("CleanupCollecting");

            foreach (SimDescription sim in Household.AllSimsLivingInWorld())
            {
                Collecting skill = sim.SkillManager.GetSkill<Collecting>(SkillNames.Collecting);
                if (skill == null) continue;

                EnactCorrections(skill);
            }
        }
    }
}
