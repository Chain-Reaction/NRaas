using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims
{
    public class TestSocials : OptionItem, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "TestSocials";
        }

        protected static void DoInteraction(Sim ths)
        {
            string msg = ths.FullName;
                      
            if (AutonomyRestrictions.IsAnyAutonomyEnabled(ths))
            {
                msg += Common.NewLine + "IsAnyAutonomyEnabled";

                if (!ths.Autonomy.InAutonomyManagerQueue)
                {
                    msg += Common.NewLine + "Not InAutonomyManagerQueue";

                    if (ths.CanRunAutonomyImmediately())
                    {
                        msg += Common.NewLine + "CanRunAutonomyImmediately";

                        AutonomyManager.Add(ths.Autonomy);
                    }
                    else
                    {
                        float autonomyDelayDuringSocializing;
                        float timeSinceInteractionQueueBecameEmpty = ths.Autonomy.TimeSinceInteractionQueueBecameEmpty;
                        if ((ths.Conversation != null) && (ths.IsActiveSim || ((ths.Conversation.WhoTalkedLast != null) && ths.Conversation.WhoTalkedLast.IsActiveSim)))
                        {
                            msg += Common.NewLine + "Section A";

                            autonomyDelayDuringSocializing = Sims3.Gameplay.Autonomy.Autonomy.AutonomyDelayDuringSocializing;
                        }
                        else if (ths.mExitReason == ExitReason.UserCanceled)
                        {
                            msg += Common.NewLine + "Section B";

                            autonomyDelayDuringSocializing = Sims3.Gameplay.Autonomy.Autonomy.AutonomyDelayAfterUserCancellation;
                        }
                        else
                        {
                            msg += Common.NewLine + "Section C";

                            autonomyDelayDuringSocializing = Sims3.Gameplay.Autonomy.Autonomy.AutonomyDelayNormal;
                        }

                        msg += Common.NewLine + "timeSinceInteractionQueueBecameEmpty=" + timeSinceInteractionQueueBecameEmpty;
                        msg += Common.NewLine + "autonomyDelayDuringSocializing=" + autonomyDelayDuringSocializing;

                        if (((timeSinceInteractionQueueBecameEmpty < 0f) || (timeSinceInteractionQueueBecameEmpty >= autonomyDelayDuringSocializing)) || ((ths.Service != null) || ths.SimDescription.HasActiveRole))
                        {
                            AutonomyManager.Add(ths.Autonomy);
                        }
                    }
                }
            }

            Common.Notify(msg);
        }

        public static void AddSocials(SocialComponent ths, Sim actor, Sim target, List<SocialInteractionCandidate> socials, bool isAutonomous, int maxNumSocials, List<InteractionObjectPair> results, string[] inlineParentMenu, TraitNames trait, ref string msg)
        {
            int num = 0x0;
            InteractionPriority priority = new InteractionPriority(isAutonomous ? InteractionPriorityLevel.Autonomous : InteractionPriorityLevel.UserDirected);
            GreyedOutTooltipCallback greyedOutTooltipCallback = null;
            foreach (SocialInteractionCandidate candidate in socials)
            {
                using (Common.TestSpan span = new Common.TestSpan(TimeSpanLogger.Bin, candidate.Name, Common.DebugLevel.Stats))
                {
                    string[] strArray = inlineParentMenu;
                    bool flag = true;
                    if (num >= maxNumSocials)
                    {
                        break;
                    }
                    ActiveTopic topic = candidate.Topic;
                    if (topic != null)
                    {
                        ActiveTopicData data = topic.Data;
                        if (data.HasPieMenuOverride())
                        {
                            flag = false;
                            strArray = new string[] { data.GetText(actor, new object[0x0]) };
                        }
                    }
                    ActionData data2 = ActionData.Get(candidate.Name);
                    if (data2.AppearsOnTopLevel)
                    {
                        strArray = new string[0x0];
                    }
                    if (!ths.DoesSocialAppear(candidate.Name, results, strArray))
                    {
                        InteractionObjectPair iop = null;
                        if (((data2.IntendedCommodityString == CommodityTypes.Friendly) && ((actor.Posture is ISeatedSocialPosture) || (target.Posture is ISeatedSocialPosture))) && data2.AllowCarryChild)
                        {
                            InteractionDefinition interaction = new SeatedSocialInteractionA.SeatedDefinition(candidate.Name, strArray, candidate.Topic, false, trait, false);
                            iop = new InteractionObjectPair(interaction, target);
                            InteractionInstanceParameters parameters = new InteractionInstanceParameters(iop, actor, priority, isAutonomous, true);
                            if (!IUtil.IsPass(interaction.Test(ref parameters, ref greyedOutTooltipCallback)))
                            {
                                iop = null;
                            }
                            if (((candidate.Name == "Chat") && !actor.CanBeInSameGroupTalkAsMe(target)) && (actor.Posture is ISeatedSocialPosture))
                            {
                                string[] strArray2 = new string[0x0];
                                if (!ths.DoesSocialAppear(candidate.Name, results, strArray2))
                                {
                                    interaction = new SeatedSocialInteractionA.SeatedDefinition(candidate.Name, strArray2, candidate.Topic, false, trait, true);
                                    InteractionObjectPair pair2 = new InteractionObjectPair(interaction, target);
                                    parameters = new InteractionInstanceParameters(pair2, actor, priority, isAutonomous, true);
                                    if (IUtil.IsPass(interaction.Test(ref parameters, ref greyedOutTooltipCallback)))
                                    {
                                        results.Add(pair2);
                                    }
                                }
                            }
                        }
                        if (iop == null)
                        {
                            iop = new InteractionObjectPair(new SocialInteractionA.Definition(candidate.Name, strArray, candidate.Topic, false, trait), target);
                        }
                        InteractionInstanceParameters parameters2 = new InteractionInstanceParameters(iop, actor, priority, isAutonomous, true);

                        InteractionTestResult result = iop.InteractionDefinition.Test(ref parameters2, ref greyedOutTooltipCallback);

                        msg += Common.NewLine + parameters2.InteractionDefinition.GetInteractionName(ref parameters2) + ": " + result;

                        if ((IUtil.IsPass(result) || (greyedOutTooltipCallback != null)) || Sims3.Gameplay.UI.PieMenu.PieMenuShowFailureReason)
                        {
                            results.Add(iop);
                            if (flag)
                            {
                                num++;
                            }
                        }
                    }
                }
            }
        }

        public static List<SocialInteractionCandidate> Get(Sim actor, Sim target, ShortTermContextTypes category, LongTermRelationshipTypes group, bool isActive, ActiveTopic topic, bool isAutonomous, ref string msg)
        {
            Dictionary<LongTermRelationshipTypes, Dictionary<bool, List<string>>> dictionary;
            List<SocialInteractionCandidate> list = new List<SocialInteractionCandidate>();

            if (ActionAvailabilityData.sStcInteractions.TryGetValue(category, out dictionary))
            {
                Dictionary<bool, List<string>> dictionary2;
                List<string> list2;
                bool flag = dictionary.TryGetValue(group, out dictionary2);
                if (!flag)
                {
                    group = LongTermRelationshipTypes.Default;
                    flag = dictionary.TryGetValue(group, out dictionary2);
                }
                if (!flag || !dictionary2.TryGetValue(isActive, out list2))
                {
                    msg += Common.NewLine + "Get Fail 1 " + category;
                    return list;
                }

                msg += Common.NewLine + "Get Found " + category + " " + list2.Count;

                foreach (string str in list2)
                {
                    ActionData data = ActionData.Get(str);
                    GreyedOutTooltipCallback greyedOutTooltipCallback = null;

                    InteractionTestResult result = data.Test(actor, target, isAutonomous, topic, ref greyedOutTooltipCallback);

                    msg += Common.NewLine + " " + str + " " + result;

                    if ((IUtil.IsPass(result) || (greyedOutTooltipCallback != null)) || Sims3.Gameplay.UI.PieMenu.PieMenuShowFailureReason)
                    {
                        list.Add(new SocialInteractionCandidate(str, data.GetParentMenu(actor, target), topic));
                    }
                }
            }
            else
            {
                msg += Common.NewLine + "Get Fail 2 " + category;
            }

            return list;
        }

        // From Conversation
        public static List<SocialInteractionCandidate> GetSocialsForSTC(CommodityTypes commodity, Sim actor, Sim target, Relationship r, Conversation c, bool isAutonomous, ref string msg)
        {
            using (Common.TestSpan span = new Common.TestSpan(TimeSpanLogger.Bin, "GetSocialsForSTC: " + commodity, Common.DebugLevel.Stats))
            {
                ShortTermContextTypes str2;
                List<SocialInteractionCandidate> list3;
                LongTermRelationshipTypes group = LongTermRelationshipTypes.Stranger;
                if (r != null)
                {
                    group = r.LTR.CurrentLTR;
                }
                bool flag = (r != null) && ((r.STC == null) || (commodity == r.STC.CurrentCommodity));
                if (!SocialComponent.SocialCommodityIsAvailable(commodity, target, actor, r))
                {
                    return null;
                }
                if (flag)
                {
                    bool flag2;
                    str2 = Conversation.GetCurrentSTC(actor, target, r, c, out flag2);
                }
                else
                {
                    str2 = STCData.FirstSTC(commodity);
                }
                List<SocialInteractionCandidate> collection = ActionAvailabilityData.Get(actor, target, str2, group, true, null, isAutonomous);
                if ((isAutonomous && (r != null)) && (r.STC.CurrentCommodity == commodity))
                {
                    ShortTermContextTypes str4;
                    collection = new List<SocialInteractionCandidate>(collection);
                    ShortTermContextTypes str3 = STCData.PreviousSTC(str2);
                    do
                    {
                        collection.AddRange(ActionAvailabilityData.Get(actor, target, str3, group, true, null, isAutonomous));
                        str4 = str3;
                        str3 = STCData.PreviousSTC(str3);
                    }
                    while (!(str4 == str3));
                }
                if (isAutonomous)
                {
                    goto Label_0167;
                }
                ShortTermContextTypes category = STCData.FirstSTC(commodity);
            Label_00D5:
                msg += Common.NewLine + "GetSocialsForSTC " + category;

                foreach (SocialInteractionCandidate candidate in Get(actor, target, category, group, true, null, isAutonomous, ref msg))
                {
                    ActionData action = ActionData.Get(candidate.Name);
                    if (Conversation.SimHasTraitEncouragingOrRequiringSocial(actor, action))
                    {
                        if (category != STCData.FirstSTC(commodity))
                        {
                            candidate.OnlyAppearsInTraitMenu = true;
                        }

                        collection.Add(candidate);
                    }
                }
                ShortTermContextTypes str6 = STCData.NextSTC(category);
                if (str6 != category)
                {
                    category = str6;
                    goto Label_00D5;
                }
            Label_0167:
                msg += Common.NewLine + "Collection " + collection.Count;

                list3 = new List<SocialInteractionCandidate>();
                foreach (SocialInteractionCandidate candidate2 in collection)
                {
                    ActionData data2 = ActionData.Get(candidate2.Name);
                    if (flag || !data2.DoesSocialOnlyAppearWhenSTCIsCurrent)
                    {
                        msg += Common.NewLine + " " + candidate2.Name;

                        list3.Add(candidate2);
                    }
                }
                return list3;
            }
        }

        public class SocialInteractionCandidateCollectionEx : SocialInteractionCandidateCollection
        {
            public SocialInteractionCandidateCollectionEx(Sim actor, Sim target, Relationship r, Conversation c, bool isAutonomous, ref string msg)
                : base (actor, target, r, c, isAutonomous)
            {
                using (Common.TestSpan span = new Common.TestSpan(TimeSpanLogger.Bin, "SocialInteractionCandidateCollectionEx", Common.DebugLevel.Stats))
                {
                    mCommodityDictionary = new Dictionary<CommodityTypes, SocialInteractionCandidateForCommodity>();
                    mTraitDictionary = new Dictionary<TraitNames, SocialInteractionCandidateForCommodity>();
                    mActor = actor;
                    mTarget = target;

                    List<SocialInteractionCandidate> candidates = Conversation.GetActiveTopicInteractions(r.LTR.CurrentLTR, actor, target, c, isAutonomous);
                    CreateFromCandidates(CommodityTypes.Undefined, candidates);

                    if (actor.IsHuman && target.IsHuman)
                    {

                        foreach (CommodityTypes types in CommodityData.AllPlayableCommodities)
                        {
                            msg += Common.NewLine + "Commodity: " + types;

                            List<SocialInteractionCandidate> list2 = GetSocialsForSTC(types, actor, target, r, c, isAutonomous, ref msg);

                            using (Common.TestSpan span2 = new Common.TestSpan(TimeSpanLogger.Bin, "CreateFromCandidates: " + types, Common.DebugLevel.Stats))
                            {
                                CreateFromCandidates(types, list2);
                            }
                        }
                    }
                }
            } 
        }

        // From SocialComponent
        private static List<InteractionObjectPair> GetUnfilteredSocials(SocialComponent ths, Sim actor, Sim target, Relationship r)
        {
            string msg = actor.FullName;
            msg += Common.NewLine + target.FullName;

            bool isAutonomous = false;
            List<InteractionObjectPair> results = new List<InteractionObjectPair>();
            if (actor != target)
            {
                bool flag2;
                int numSocials = STCData.GetNumSocials(ths.Conversation);

                SocialInteractionCandidateCollection candidates = new SocialInteractionCandidateCollectionEx(actor, target, r, ths.Conversation, isAutonomous, ref msg);

                Conversation.GetCurrentSTC(actor, target, r, ths.Conversation, out flag2);
                foreach (CommodityTypes str in CommodityData.AllPlayableCommodities)
                {
                    using (Common.TestSpan span = new Common.TestSpan(TimeSpanLogger.Bin, "Commodity: " + str, Common.DebugLevel.Stats))
                    {
                        msg += Common.NewLine + "1 " + str;

                        switch (str)
                        {
                            case CommodityTypes.Amorous:
                                {
                                    if ((actor.CanGetRomantic(target, false) || !actor.SimDescription.IsHuman) || !target.SimDescription.IsHuman)
                                    {
                                        break;
                                    }
                                    results.Add(new InteractionObjectPair(Sim.BetrayedSimsFutileRomance.Singleton, target));
                                    continue;
                                }
                            case CommodityTypes.Steamed:
                                {
                                    if (!r.STC.IsPositive)
                                    {
                                        break;
                                    }
                                    continue;
                                }
                            case CommodityTypes.Insulting:
                                if (isAutonomous && actor.BuffManager.HasElement(BuffNames.Admonished))
                                {
                                    continue;
                                }
                                break;
                        }

                        msg += Common.NewLine + "2 " + str;

                        int num2 = numSocials;
                        if (CommodityData.AppearsAsSubMenu(str))
                        {
                            msg += Common.NewLine + "AppearsAsSubMenu " + str;

                            List<SocialInteractionCandidate> socialsFor = candidates.GetSocialsFor(str);
                            if (socialsFor != null)
                            {
                                msg += Common.NewLine + "socialsFor " + str + " " + socialsFor.Count;

                                string changeToneText = CommodityData.Get(str).GetChangeToneText();
                                AddSocials(ths, actor, target, socialsFor, isAutonomous, num2, results, new string[] { changeToneText }, TraitNames.Unknown, ref msg);
                                if (str == CommodityTypes.Friendly)
                                {
                                    List<SocialInteractionCandidate> socials = candidates.GetSocialsFor(CommodityTypes.Neutral);
                                    if (socials != null)
                                    {
                                        AddSocials(ths, actor, target, socials, isAutonomous, num2, results, new string[] { changeToneText }, TraitNames.Unknown, ref msg);
                                    }
                                }
                            }
                        }
                    }
                }

                string str3 = Localization.LocalizeString("Gameplay/Socializing:Special", new object[0x0]);
                foreach (Trait trait in actor.TraitManager.List)
                {
                    List<SocialInteractionCandidate> list4 = candidates.GetSocialsFor(trait.Guid);
                    if ((list4 != null) && (list4.Count > 0x0))
                    {
                        string str5 = trait.TraitName(actor.IsFemale);
                        string[] inlineParentMenu = new string[] { str3, str5 + Localization.Ellipsis };
                        AddSocials(ths, actor, target, list4, isAutonomous, numSocials, results, inlineParentMenu, trait.Guid, ref msg);
                    }
                }

                if (((ths.Conversation != null) && (actor.Household != target.Household)) && ((!actor.IsAtHome || target.IsAtHome) && (actor.IsAtHome || !target.IsAtHome)))
                {
                    SocialInteractionCandidate candidate = new SocialInteractionCandidate(ths.GetGoodbyeSocial(actor, target), new string[0], null);
                    ths.AddSocials(actor, target, new List<SocialInteractionCandidate>(new SocialInteractionCandidate[] { candidate }), isAutonomous, numSocials, results, new string[0x0], TraitNames.Unknown);
                }
            }

            Common.WriteLog(msg);

            return results;
        }

        protected static void TestOutfits(SimDescription sim)
        {
            string msg = sim.FullName;

            if (sim.CreatedSim != null)
            {
                msg += Common.NewLine + "Current: " + sim.CreatedSim.CurrentOutfitCategory + " " + sim.CreatedSim.CurrentOutfitIndex;
            }

            if (sim.IsUsingMaternityOutfits)
            {
                msg += Common.NewLine + "--  Using Maternity --";
            }

            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    msg += Common.NewLine + "Regular";
                }
                else
                {
                    if (sim.mMaternityOutfits == null)
                    {
                        msg += Common.NewLine + "No Maternity";
                        continue;
                    }

                    msg += Common.NewLine + "Maternity";
                }

                foreach (OutfitCategories category in Enum.GetValues(typeof(OutfitCategories)))
                {
                    ArrayList outfits = null;

                    if (i == 0)
                    {
                        outfits = sim.Outfits[category] as ArrayList;
                    }
                    else
                    {
                        outfits = sim.mMaternityOutfits[category] as ArrayList;
                    }

                    if (outfits == null)
                    {
                        msg += Common.NewLine + category + " None";
                    }
                    else
                    {
                        msg += Common.NewLine + category + " Count: " + outfits.Count;

                        int index = 0;
                        foreach (SimOutfit outfit in outfits)
                        {
                            if (outfit == null)
                            {
                                msg += Common.NewLine + index + " Missing";
                            }
                            else
                            {
                                msg += Common.NewLine + index + " Valid=" + outfit.IsValid;
                            }
                            index++;
                        }
                    }
                }
            }

            Common.WriteLog(msg);
        }

        public bool Test(GameHitParameters<SimDescriptionObject> parameters)
        {
            return base.Test(new GameHitParameters<GameObject>(parameters.mActor, parameters.mTarget, parameters.mHit));
        }

        public OptionResult Perform(GameHitParameters<SimDescriptionObject> parameters)
        {
            return base.Perform(new GameHitParameters<GameObject>(parameters.mActor, parameters.mTarget, parameters.mHit));
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            try
            {
                Sim target = parameters.mTarget as Sim;

                using (Common.TestSpan span = new Common.TestSpan(TimeSpanLogger.Bin, "Total", Common.DebugLevel.Stats))
                {
                    GetUnfilteredSocials(parameters.mActor.SocialComponent, parameters.mActor as Sim, target, Relationship.Get(parameters.mActor as Sim, target, false));
                }

                Common.RecordErrors();
            }
            catch (Exception e)
            {
                GameHitParameters<GameObject>.Exception(parameters, e);
            }
            return OptionResult.SuccessClose;
        }
    }
}
