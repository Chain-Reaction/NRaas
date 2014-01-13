using NRaas.WoohooerSpace.Interactions;
using NRaas.WoohooerSpace.Scoring;
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
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Helpers
{
    public class AmorousCommodity : Common.IPreLoad
    {
        public readonly static CommodityTypes sAmorous2 = (CommodityTypes)ResourceUtils.HashString64("Amorous2");

        public readonly static ShortTermContextTypes sFlirty2 = (ShortTermContextTypes)ResourceUtils.HashString64("Flirty2");
        public readonly static ShortTermContextTypes sHot2 = (ShortTermContextTypes)ResourceUtils.HashString64("Hot2");
        public readonly static ShortTermContextTypes sSeductive2 = (ShortTermContextTypes)ResourceUtils.HashString64("Seductive2");

        protected static void CloneSTCData(ShortTermContextTypes newSTC, CommodityTypes newCom, ShortTermContextTypes oldSTC)
        {
            STCData oldData = STCData.Get(oldSTC);

            STCData.Add(new STCData(newSTC, oldData.mText, oldData.mPassiveText, newCom, oldData.RangeMin, oldData.RangeMax, oldData.IdleJazzState, oldData.LTRDecayFromBelow, oldData.LTRDecayFromAbove, oldData.VisitLengthChange, oldData.CharismaIncrease, oldData.X1, oldData.Y1, oldData.X2, oldData.Y2, oldData.X3, oldData.Y3, oldData.MoodThresholdForAcceptRequest));

            ActionAvailabilityData.sStcInteractions.Add(newSTC, ActionAvailabilityData.sStcInteractions[oldSTC]);
        }

        public void OnPreLoad()
        {
            CommodityData amorous = CommodityData.Get(CommodityTypes.Amorous);

            CommodityData.Add(new CommodityData(
                sAmorous2, 
                amorous.ChangeToneText, 
                amorous.mIsPositive, 
                false /*amorous.mIsSymmetric*/, 
                amorous.mAppearsAsSubMenu, 
                amorous.EndStanceX, 
                amorous.EndStanceY, 
                amorous.NumSocialsDecremented, 
                amorous.NextCommodity1, 
                amorous.NextCommodity2,
                amorous.X1,
                amorous.X2,
                amorous.Y1,
                amorous.Y2,
                amorous.Y3
            ));

            CommodityTable.sMap[sAmorous2] = new Dictionary<CommodityTypes, float>();

            CommodityTable.Add(sAmorous2, sAmorous2, 1);

            foreach (CommodityTypes type in Enum.GetValues(typeof(CommodityTypes)))
            {
                if (type == CommodityTypes.Undefined) continue;

                CommodityTable.Add(sAmorous2, type, CommodityTable.Get(CommodityTypes.Amorous, type));

                CommodityTable.Add(type, sAmorous2, CommodityTable.Get(type, CommodityTypes.Amorous));
            }

            foreach (SocialRuleLHS oldRule in new List<SocialRuleLHS>(SocialRuleLHS.Get("GR")))
            {
                if (oldRule.GeneralCommodity != CommodityTypes.Amorous) continue;

                SocialRuleLHS newRule = new SocialRuleLHS(
                    "GR",
                    sAmorous2,
                    oldRule.mSpeechAct,
                    oldRule.ThirdParty,
                    oldRule.STCommodityPositive,
                    new Pair<CommodityTypes, bool>(oldRule.STCommodity.First, oldRule.STCommodity.Second),
                    new Pair<ShortTermContextTypes, bool>(oldRule.STContext.First, oldRule.STContext.Second),
                    oldRule.TargetMood,
                    new Pair<LongTermRelationshipTypes, bool>(oldRule.LTRelationship.First, oldRule.LTRelationship.Second),
                    oldRule.LTRMin,
                    oldRule.LTRMax,
                    "",
                    "",
                    "",
                    oldRule.mSkillLevel,
                    oldRule.mSkillThresholdAbove,
                    "",
                    "",
                    "",
                    "",
                    "",
                    oldRule.Partner,
                    oldRule.WrongGenderPreference,
                    oldRule.Repetition,
                    oldRule.ActorAgeRestrictions,
                    oldRule.TargetAgeRestrictions,
                    oldRule.TargetBetrayed,
                    oldRule.SpecificityOverride,
                    oldRule.STEffectCommodity,
                    oldRule.LTROverride
                );

                newRule.mActorTraits = oldRule.mActorTraits;
                newRule.mTargetTraits = oldRule.mTargetTraits;
                newRule.mSkill = oldRule.mSkill;
                newRule.mActorIncreasedEffectiveness = oldRule.mActorIncreasedEffectiveness;
                newRule.mActorReducedEffectiveness = oldRule.mActorReducedEffectiveness;
                newRule.mTargetIncreasedEffectiveness = oldRule.mTargetIncreasedEffectiveness;
                newRule.mTargetReducedEffectiveness = oldRule.mTargetReducedEffectiveness;
                newRule.ProceduralPrecondition = oldRule.ProceduralPrecondition;
            }

            SocialRuleLHS.Get("GR").Sort(new Comparison<SocialRuleLHS>(SocialRuleLHS.SortSocialRules));

            CloneSTCData(sFlirty2, sAmorous2, ShortTermContextTypes.Flirty);
            CloneSTCData(sSeductive2, sAmorous2, ShortTermContextTypes.Seductive);
            CloneSTCData(sHot2, sAmorous2, ShortTermContextTypes.Hot);

            STCData.SetNextSTC(sFlirty2, sSeductive2);
            STCData.SetNextSTC(sSeductive2, sHot2);
        }
    }
}
