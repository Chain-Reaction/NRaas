using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.TuningValues;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TempestSpace.Helpers
{
    public class HarvestPlantEx
    {
        public static bool DoHarvest(HarvestPlant ths, Sim actor, bool hasHarvested, BurglarSituation burglarSituation)
        {
            Slot[] containmentSlots = ths.GetContainmentSlots();
            List<GameObject> objectsHarvested = new List<GameObject>();
            foreach (Slot slot in containmentSlots)
            {
                GameObject containedObject = ths.GetContainedObject(slot) as GameObject;
                if ((containedObject != null) && ths.HarvestHarvestable(containedObject, actor, burglarSituation))
                {
                    objectsHarvested.Add(containedObject);
                }
            }

            if (actor.TraitManager.HasElement(TraitNames.GathererTrait) && RandomUtil.RandomChance01(TraitTuning.GathererTraitExtraHarvestablesChance))
            {
                int gathererTraitNumberOfExtraHarvestables = TraitTuning.GathererTraitNumberOfExtraHarvestables;
                for (int i = 0; i < gathererTraitNumberOfExtraHarvestables; i++)
                {
                    GameObject item = ths.Seed.Copy(false) as GameObject;
                    if (item != null)
                    {
                        objectsHarvested.Add(item);
                    }
                }
            }

            ScienceSkill element = actor.SkillManager.GetSkill<ScienceSkill>(SkillNames.Science);
            if ((element != null) && RandomUtil.RandomChance01(ScienceSkill.kSuccessRateBonusesGardening[element.SkillLevel]))
            {
                int num3 = ScienceSkill.kNumExtraHarvestables[element.SkillLevel];
                for (int j = 0; j < num3; j++)
                {
                    GameObject obj6 = ths.Seed.Copy(false) as GameObject;
                    if (obj6 != null)
                    {
                        objectsHarvested.Add(obj6);
                    }
                }
            }

            if (objectsHarvested.Count <= 0)
            {
                return false;
            }

            foreach (GameObject obj7 in objectsHarvested)
            {
                ths.ManipulateHarvestable(actor, obj7);
            }

            Gardening skill = actor.SkillManager.GetSkill<Gardening>(SkillNames.Gardening);
            Collecting collecting = actor.SkillManager.GetSkill<Collecting>(SkillNames.Collecting);
            int skillDifficulty = ths.PlantDef.GetSkillDifficulty();
            if (skill != null)
            {
                if (skill.SkillLevel <= skillDifficulty)
                {
                    if (actor.SimDescription.IsFairy && skill.IsFairySkill())
                    {
                        skill.AddPoints(ths.PlantDef.SkillPointsHarvest * Skill.SkillLevelBumpMultiplierForFairies);
                    }
                    else
                    {
                        skill.AddPoints(ths.PlantDef.SkillPointsHarvest);
                    }
                }

                if (ths is MoneyTree)
                {
                    MoneyTreeEx.UpdateGardeningSkillJournal(ths as MoneyTree, skill, ths.PlantDef, objectsHarvested);
                }
                else
                {
                    ths.UpdateGardeningSkillJournal(skill, ths.PlantDef, objectsHarvested);
                }
            }

            if (!hasHarvested)
            {
                actor.ShowTNSIfSelectable(Localization.LocalizeString(actor.IsFemale, "Gameplay/Objects/Gardening/HarvestPlant/Harvest:FirstHarvest", new object[] { actor, ths.PlantDef.Name }), StyledNotification.NotificationStyle.kGameMessagePositive, ths.ObjectId, actor.ObjectId);
            }

            if (collecting == null)
            {
                collecting = actor.SkillManager.AddElement(SkillNames.Collecting) as Collecting;
            }

            collecting.CollectedFromHarvest(objectsHarvested);
            ths.PostHarvest();
            return true;
        }
    }
}
