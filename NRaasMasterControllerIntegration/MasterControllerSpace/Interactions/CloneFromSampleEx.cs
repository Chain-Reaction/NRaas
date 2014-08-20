using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class CloneFromSampleEx : ScienceResearchStation.CloneFromSample, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<ScienceResearchStation, ScienceResearchStation.CloneFromSample.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<ScienceResearchStation, ScienceResearchStation.CloneFromSample.Definition>(Singleton);
        }

        public new class Definition : ScienceResearchStation.CloneFromSample.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CloneFromSampleEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, ScienceResearchStation target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, ScienceResearchStation target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (target.Repairable.Broken)
                {
                    greyedOutTooltipCallback = new GreyedOutTooltipCallback(target.StationIsBroken);
                    return false;
                }
                ScienceSkill element = (ScienceSkill)a.SkillManager.GetElement(SkillNames.Science);
                if ((element == null) || (element.SkillLevel < ScientificSample.CloneFromSample.MinScienceSkillLevel))
                {
                    greyedOutTooltipCallback = new GreyedOutTooltipCallback(ScienceResearchStation.DisplayLevelTooLowTooltip);
                    return false;
                }
                new List<InventoryStack>();
                bool flag = false;
                foreach (InventoryStack stack in a.Inventory.InventoryItems.Values)
                {
                    ScientificSample sample = null;
                    if (stack != null)
                    {
                        sample = stack.List[0].Object as ScientificSample;
                    }
                    if ((sample != null) && ((sample.ScientificSampleType != ScientificSample.SampleType.Dna) /*|| (!GameUtils.IsUniversityWorld() && a.Household.CanAddSpeciesToHousehold(CASAgeGenderFlags.None | CASAgeGenderFlags.Human))*/ ))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(ScienceResearchStation.LocalizeString("SampleNotPresent", new object[0]));
                    return false;
                }
                return flag;
            }
        }
    }
}