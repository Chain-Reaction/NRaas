using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
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
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims
{
    public class TestPopupMenu : OptionItem, ISimOption
    {
        static string sMsg = null;

        protected class UIMouseEventArgsEx : UIMouseEventArgs
        {
            public UIMouseEventArgsEx()
            {
                Vector2 position = UIManager.GetCursorPosition();

                base.mF1 = position.x;
                base.mF2 = position.y;
            }
        }

        public override string GetTitlePrefix()
        {
            return "TestPopupMenu";
        }

        public static void AddSocials(string name, SocialComponent ths, CommodityTypes commodity, Sim actor, Sim target, List<SocialInteractionCandidate> socials, bool isAutonomous, int maxNumSocials, List<InteractionObjectPair> results, string[] inlineParentMenu, TraitNames trait)
        {
            sMsg += Common.NewLine + "AddSocials " + name + " " + commodity + " " + socials.Count;

            int num = 0x0;
            InteractionPriority priority = new InteractionPriority(isAutonomous ? InteractionPriorityLevel.Autonomous : InteractionPriorityLevel.UserDirected);
            GreyedOutTooltipCallback greyedOutTooltipCallback = null;
            foreach (SocialInteractionCandidate candidate in socials)
            {
                string[] strArray = inlineParentMenu;
                bool flag = true;
                if (num >= maxNumSocials)
                {
                    break;
                }

                sMsg += Common.NewLine + " Testing " + candidate.Name;

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

                    sMsg += Common.NewLine + "  " + candidate.Name + " = " + result;

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

        public static List<SocialInteractionCandidate> Get(string name, Sim actor, Sim target, ShortTermContextTypes stc, LongTermRelationshipTypes group, bool isActive, ActiveTopic topic, bool isAutonomous)
        {
            sMsg += Common.NewLine + "GetActionAvailability " + name + " " + stc + " " + group + " " + isActive;

            Dictionary<LongTermRelationshipTypes, Dictionary<bool, List<string>>> dictionary;
            if (ActionAvailabilityData.sStcInteractions.TryGetValue(stc, out dictionary))
            {
                return GetInternal(name, actor, target, group, isActive, topic, isAutonomous, dictionary);
            }
            else
            {
                sMsg += Common.NewLine + " Empty";
            }
            return new List<SocialInteractionCandidate>();
        }

        private static List<SocialInteractionCandidate> GetInternal(string name, Sim actor, Sim target, LongTermRelationshipTypes group, bool isActive, ActiveTopic topic, bool isAutonomous, Dictionary<LongTermRelationshipTypes, Dictionary<bool, List<string>>> interactionCategories)
        {
            Dictionary<bool, List<string>> dictionary;
            List<string> list2;
            bool flag = interactionCategories.TryGetValue(group, out dictionary);
            if (!flag)
            {
                sMsg += Common.NewLine + " Try Default";

                group = LongTermRelationshipTypes.Default;
                flag = interactionCategories.TryGetValue(group, out dictionary);
            }
            List<SocialInteractionCandidate> list = new List<SocialInteractionCandidate>();
            if (flag && dictionary.TryGetValue(isActive, out list2))
            {
                sMsg += Common.NewLine + " Choices = " + list2.Count;

                foreach (string str in list2)
                {
                    ActionData data = ActionData.Get(str);
                    GreyedOutTooltipCallback greyedOutTooltipCallback = null;

                    InteractionTestResult result = data.Test(actor, target, isAutonomous, topic, ref greyedOutTooltipCallback);

                    sMsg += Common.NewLine + "  " + str + " = " + result;

                    if ((IUtil.IsPass(result) || (greyedOutTooltipCallback != null)) || Sims3.Gameplay.UI.PieMenu.PieMenuShowFailureReason)
                    {
                        list.Add(new SocialInteractionCandidate(str, data.GetParentMenu(actor, target), topic));
                    }
                }
            }
            return list;
        }

        public static List<SocialInteractionCandidate> GetSocialsForSTC(CommodityTypes commodity, Sim actor, Sim target, Relationship r, Conversation c, bool isAutonomous)
        {
            ShortTermContextTypes types2;
            List<SocialInteractionCandidate> list3;
            LongTermRelationshipTypes stranger = LongTermRelationshipTypes.Stranger;
            if (r != null)
            {
                stranger = r.LTR.CurrentLTR;
            }
            bool flag = (r != null) && ((r.STC == null) || (commodity == r.STC.CurrentCommodity));
            if (!SocialComponent.SocialCommodityIsAvailable(commodity, target, actor, r))
            {
                sMsg += Common.NewLine + "Denied";
                return null;
            }
            if (flag)
            {
                bool flag2;
                types2 = Conversation.GetCurrentSTC(actor, target, r, c, out flag2);

                sMsg += Common.NewLine + "CurrentSTC " + types2 + " " + r.STC.CurrentCommodity;
            }
            else
            {
                types2 = STCData.FirstSTC(commodity);

                sMsg += Common.NewLine + "FirstSTC " + types2 + " " + r.STC.CurrentCommodity;
            }

            List<SocialInteractionCandidate> collection = Get("1", actor, target, types2, stranger, true, null, isAutonomous);

            if ((isAutonomous && (r != null)) && (r.STC.CurrentCommodity == commodity))
            {
                ShortTermContextTypes types4;
                collection = new List<SocialInteractionCandidate>(collection);
                ShortTermContextTypes types3 = STCData.PreviousSTC(types2);
                do
                {
                    collection.AddRange(Get("2", actor, target, types3, stranger, true, null, isAutonomous));

                    types4 = types3;
                    types3 = STCData.PreviousSTC(types3);
                }
                while (types4 != types3);
            }
            if (isAutonomous)
            {
                goto Label_014C;
            }
            ShortTermContextTypes stc = STCData.FirstSTC(commodity);
        Label_00C4:
            List<SocialInteractionCandidate> traitList = Get("3", actor, target, stc, stranger, true, null, isAutonomous);

            foreach (SocialInteractionCandidate candidate in traitList)
            {
                ActionData action = ActionData.Get(candidate.Name);
                if (Conversation.SimHasTraitEncouragingOrRequiringSocial(actor, action))
                {
                    if (stc != STCData.FirstSTC(commodity))
                    {
                        candidate.OnlyAppearsInTraitMenu = true;
                    }
                    collection.Add(candidate);
                }
            }
            ShortTermContextTypes types6 = STCData.NextSTC(stc);
            if (types6 != stc)
            {
                stc = types6;
                goto Label_00C4;
            }
        Label_014C:
            sMsg += Common.NewLine + "Final = " + collection.Count;

            list3 = new List<SocialInteractionCandidate>();
            foreach (SocialInteractionCandidate candidate2 in collection)
            {
                sMsg += Common.NewLine + "Test " + candidate2.Name;

                ActionData data2 = ActionData.Get(candidate2.Name);
                if (flag || !data2.DoesSocialOnlyAppearWhenSTCIsCurrent)
                {
                    sMsg += Common.NewLine + " Added " + candidate2.Name;

                    list3.Add(candidate2);
                }
            }
            return list3;
        }

        private static List<InteractionObjectPair> GetUnfilteredSocials(SocialComponent ths, Sim actor, Sim target, Relationship r)
        {
            bool isAutonomous = false;
            List<InteractionObjectPair> results = new List<InteractionObjectPair>();
            if (actor != target)
            {
                int numSocials = STCData.GetNumSocials(ths.Conversation);
                SocialInteractionCandidateCollection candidates = new SocialInteractionCandidateCollection(actor, target, r, ths.Conversation, isAutonomous);
                foreach (CommodityTypes types in CommodityData.AllPlayableCommodities)
                {
                    sMsg += Common.NewLine + "CommodityType: " + types;
                    switch (types)
                    {
                        case CommodityTypes.Amorous:
                            {
                                if (actor.CanGetRomantic(target, false))
                                {
                                    break;
                                }
                                results.Add(new InteractionObjectPair(Sim.BetrayedSimsFutileRomance.Singleton, target));

                                sMsg += Common.NewLine + "Amorous Denied";
                                continue;
                            }
                        case CommodityTypes.Steamed:
                            {
                                if (!r.STC.IsPositive)
                                {
                                    break;
                                }

                                sMsg += Common.NewLine + "Steamed Denied";
                                continue;
                            }
                        case CommodityTypes.Insulting:
                            if (isAutonomous && actor.BuffManager.HasElement(BuffNames.Admonished))
                            {
                                sMsg += Common.NewLine + "Insulting Denied";
                                continue;
                            }
                            break;
                    }
                    int maxNumSocials = numSocials;
                    if (CommodityData.AppearsAsSubMenu(types))
                    {
                        GetSocialsForSTC(types, actor, target, r, ths.Conversation, isAutonomous);

                        List<SocialInteractionCandidate> socialsFor = candidates.GetSocialsFor(types);
                        if (socialsFor != null)
                        {
                            string changeToneText = CommodityData.Get(types).GetChangeToneText();
                            AddSocials("1", ths, types, actor, target, socialsFor, isAutonomous, maxNumSocials, results, new string[] { changeToneText }, TraitNames.Unknown);
                            if (types == CommodityTypes.Friendly)
                            {
                                List<SocialInteractionCandidate> socials = candidates.GetSocialsFor(CommodityTypes.Neutral);
                                if (socials != null)
                                {
                                    AddSocials("2", ths, CommodityTypes.Neutral, actor, target, socials, isAutonomous, maxNumSocials, results, new string[] { changeToneText }, TraitNames.Unknown);
                                }
                            }
                        }
                    }
                }

                string str3 = Common.LocalizeEAString("Gameplay/Socializing:Special");
                foreach (Trait trait in actor.TraitManager.List)
                {
                    List<SocialInteractionCandidate> list4 = candidates.GetSocialsFor(trait.Guid);
                    if ((list4 != null) && (list4.Count > 0x0))
                    {
                        string str5 = trait.TraitName(actor.IsFemale);
                        string[] inlineParentMenu = new string[] { str3, str5 + Localization.Ellipsis };
                        AddSocials("3", ths, CommodityTypes.Undefined, actor, target, list4, isAutonomous, numSocials, results, inlineParentMenu, trait.Guid);
                    }
                }
                if (((ths.Conversation != null) && (actor.Household != target.Household)) && ((!actor.IsAtHome || target.IsAtHome) && (actor.IsAtHome || !target.IsAtHome)))
                {
                    SocialInteractionCandidate candidate = new SocialInteractionCandidate(ths.GetGoodbyeSocial(actor, target), new string[0], null);
                    AddSocials("4", ths, CommodityTypes.Undefined, actor, target, new List<SocialInteractionCandidate>(new SocialInteractionCandidate[] { candidate }), isAutonomous, numSocials, results, new string[0x0], TraitNames.Unknown);
                }
            }
            return results;
        }

        private static List<InteractionObjectPair> SocialsForNewConversation(SocialComponent ths, Sim actor, Sim target, bool isAutonomous)
        {
            string msg = null;

            List<InteractionObjectPair> list = new List<InteractionObjectPair>();
            string[] path = new string[0x0];
            InteractionPriority priority = new InteractionPriority(isAutonomous ? InteractionPriorityLevel.Autonomous : InteractionPriorityLevel.UserDirected);
            GreyedOutTooltipCallback greyedOutTooltipCallback = null;
            foreach (string str in CelebrityManager.CanSocialize(actor, target) ? new string[] { "Greet Friendly", "Greet Insulting" } : new string[] { "Greet Celebrity" })
            {
                InteractionObjectPair iop = new InteractionObjectPair(new SocialInteractionA.Definition(str, path, null, false), target);
                InteractionInstanceParameters parameters = new InteractionInstanceParameters(iop, actor, priority, isAutonomous, true);

                InteractionTestResult result = iop.InteractionDefinition.Test(ref parameters, ref greyedOutTooltipCallback);

                msg += Common.NewLine + "A " + str + " " + iop.InteractionDefinition.GetType().ToString() + " " + result;

                if (result == InteractionTestResult.Pass)
                {
                    list.Add(iop);
                }
            }
            foreach (SocialInteractionCandidate candidate in Conversation.GetActiveTopicInteractions(Relationship.GetLongTermRelationship(actor, target), actor, target, null, isAutonomous))
            {
                if (candidate.Topic.Data.AvailableWhenConversationStarts)
                {
                    InteractionObjectPair pair2 = new InteractionObjectPair(new SocialInteractionA.Definition(candidate.Name, path, null, false), target);
                    InteractionInstanceParameters parameters2 = new InteractionInstanceParameters(pair2, actor, priority, isAutonomous, true);

                    InteractionTestResult result = pair2.InteractionDefinition.Test(ref parameters2, ref greyedOutTooltipCallback);

                    msg += Common.NewLine + "B " + candidate.Name + " " + pair2.InteractionDefinition.GetType().ToString() + " " + result;

                    if (result == InteractionTestResult.Pass)
                    {
                        list.Add(pair2);
                    }
                }
            }

            Common.WriteLog(msg);

            return list;
        }

        public static IEnumerable<InteractionObjectPair> GetAllInteractionsForPieMenu(SocialComponent ths, Sim actor)
        {
            Sim mSim = ths.mSim;
            Relationship r = Relationship.Get(mSim, actor, false);
            if ((r == null) || (r.LTR.CurrentLTR == LongTermRelationshipTypes.Stranger))
            {
                sMsg += Common.NewLine + "NewConversation2";

                return SocialsForNewConversation(ths, actor, mSim, false);
            }
            if (ths.mSim.NeedsToBeGreeted(actor))
            {
                sMsg += Common.NewLine + "Greeting2";

                return ths.SocialsForGreeting(actor, ths.mSim);
            }
            LTRData data = LTRData.Get(r.LTR.CurrentLTR);
            bool flag = false;
            GroupingSituation situationOfType = actor.GetSituationOfType<GroupingSituation>();
            if (situationOfType != null)
            {
                flag = situationOfType.IsSimInGroup(mSim);
            }
            DateAndTime whenLastTalked = Relationship.GetWhenLastTalked(actor, mSim);
            if (((!flag && (actor.Conversation == null)) && ((actor.Household != mSim.Household) && (data.HowWellWeKnowEachOther <= 0x1))) && (SimClock.ElapsedTime(TimeUnit.Hours, whenLastTalked) > 24f))
            {
                sMsg += Common.NewLine + "NewConversation3";

                return SocialsForNewConversation(ths, actor, mSim, false);
            }
            if (!CelebrityManager.CanSocialize(actor, mSim))
            {
                sMsg += Common.NewLine + "Impress";

                return SocialComponent.SocialsForImpressCelebrity(actor, mSim);
            }

            sMsg += Common.NewLine + "All";

            return GetUnfilteredSocials(ths, actor, mSim, r);
        }

        public static IEnumerable<InteractionObjectPair> GetAllInteractionsForSim(SocialComponent ths, Sim actor, bool isAutonomous)
        {
            if ((ths.mSim.IsPerformingAService && !VisitSituation.IsSocializing(ths.mSim)) && !(ths.mSim.Service is Butler))
            {
                sMsg += Common.NewLine + "Service";

                return ths.GetAllServiceInteractions(actor);
            }
            List<InteractionObjectPair> list = new List<InteractionObjectPair>();
            if (isAutonomous)
            {
                sMsg += Common.NewLine + "Autonomous";

                list.AddRange(ths.GetAllInteractionsForAutonomy(actor));
                return list;
            }
            if ((ths.mSim.Posture is ISeatedSocialPosture) && (actor.Posture is ISeatedSocialPosture))
            {
                Relationship relationship = Relationship.Get(ths.mSim, actor, false);
                if ((relationship == null) || (relationship.LTR.CurrentLTR == LongTermRelationshipTypes.Stranger))
                {
                    sMsg += Common.NewLine + "New Conversation";

                    list.AddRange(SocialsForNewConversation(ths, actor, ths.mSim, false));
                    return list;
                }
                if (ths.mSim.NeedsToBeGreeted(actor))
                {
                    sMsg += Common.NewLine + "Greeting";

                    list.AddRange(ths.SocialsForGreeting(actor, ths.mSim));
                    return list;
                }

                sMsg += Common.NewLine + "Empty";

                return new List<InteractionObjectPair>();
            }

            sMsg += Common.NewLine + "PieMenu";

            list.AddRange(GetAllInteractionsForPieMenu(ths, actor));
            return list;
        }

        public static List<InteractionObjectPair> GetAllInteractionsForActor(Sim ths, IActor actor)
        {
            List<InteractionObjectPair> allInteractionsForActor;
            InteractionInstance currentInteraction = ths.CurrentInteraction;
            Sim sim = actor as Sim;
            if (ths.DisablePieMenuOnSim)
            {
                sMsg += Common.NewLine + "Disabled";

                return new List<InteractionObjectPair>();
            }
            if (ths.BuffManager.HasElement(BuffNames.BabyIsComing))
            {
                sMsg += Common.NewLine + "Pregnant";

                allInteractionsForActor = new List<InteractionObjectPair>();
                allInteractionsForActor.Add(new InteractionObjectPair(Pregnancy.TakeToHospital.Singleton, ths));
                return allInteractionsForActor;
            }
            allInteractionsForActor = new List<InteractionObjectPair>(); //base.GetAllInteractionsForActor(actor);
            if (actor != ths)
            {
                List<InteractionObjectPair> list2;
                DaycareSituation daycareSituationForSim = DaycareSituation.GetDaycareSituationForSim(ths);
                if ((daycareSituationForSim != null) && daycareSituationForSim.OverrideActorInteractions)
                {
                    sMsg += Common.NewLine + "Daycare";

                    return daycareSituationForSim.GetAllInteractionsForActor(sim, ths);
                }
                if (FirefighterEmergencySituation.FindFirefighterEmergencySituationInvolvingSim(ths) != null)
                {
                    sMsg += Common.NewLine + "Firefighter";

                    allInteractionsForActor.Clear();
                }
                else
                {
                    if (actor.Posture.GetType() != ths.Posture.GetType())
                    {
                        sMsg += Common.NewLine + "Posture Socials " + actor.Posture.GetType().ToString() + " " + ths.Posture.GetType().ToString();

                        actor.Posture.AddSocialInteractions(ths, allInteractionsForActor);
                    }
                    if (actor.Posture.AllowsNormalSocials() && ths.Posture.AllowsNormalSocials())
                    {
                        int count = 0;

                        foreach (InteractionObjectPair pair in GetAllInteractionsForSim(ths.SocialComponent, sim, false))
                        {
                            pair.AddInteractions(actor, allInteractionsForActor);
                            count++;
                        }

                        sMsg += Common.NewLine + "Socials " + count;
                    }
                    if (currentInteraction != null)
                    {
                        InteractionTuning tuning = currentInteraction.InteractionObjectPair.Tuning;
                        if (((tuning != null) && tuning.mTradeoff.JoinableInteraction) && (!(currentInteraction.InteractionDefinition is IDoNotAddJoinInteraction) && !Sim.HaveSameCurrentInteraction(sim, ths)))
                        {
                            Sim.Join.Definition interaction = new Sim.Join.Definition();
                            new InteractionObjectPair(interaction, ths).AddInteractions(actor, allInteractionsForActor);
                        }
                    }
                    if ((sim.CurrentInteraction != null) && (sim.CurrentInteraction != currentInteraction))
                    {
                        InteractionObjectPair interactionObjectPair = sim.CurrentInteraction.InteractionObjectPair;
                        if (((interactionObjectPair.Tuning != null) && interactionObjectPair.Tuning.mTradeoff.JoinableInteraction) && !Sim.HaveSameCurrentInteraction(sim, ths))
                        {
                            Sim.AskToJoin.Definition definition2 = new Sim.AskToJoin.Definition();
                            new InteractionObjectPair(definition2, ths).AddInteractions(actor, allInteractionsForActor);
                        }
                    }
                }
                if (ths.mSituationSpecificInteractions.TryGetValue(sim, out list2) && (list2.Count > 0x0))
                {
                    foreach (InteractionObjectPair pair5 in list2)
                    {
                        bool flag = false;
                        foreach (InteractionObjectPair pair6 in allInteractionsForActor)
                        {
                            SocialInteractionA.Definition interactionDefinition = pair6.InteractionDefinition as SocialInteractionA.Definition;
                            SocialInteractionA.Definition definition4 = pair5.InteractionDefinition as SocialInteractionA.Definition;
                            if (((definition4 != null) && (interactionDefinition != null)) && (interactionDefinition.ActionKey == definition4.ActionKey))
                            {
                                string[] path = interactionDefinition.GetPath(actor.IsFemale);
                                string[] strArray2 = definition4.GetPath(actor.IsFemale);
                                if (strArray2.Length == path.Length)
                                {
                                    bool flag2 = true;
                                    for (int i = 0x0; i < strArray2.Length; i++)
                                    {
                                        if (strArray2[i] != path[i])
                                        {
                                            flag2 = false;
                                            break;
                                        }
                                    }
                                    if (flag2)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!flag)
                        {
                            allInteractionsForActor.Add(pair5);
                        }
                    }
                }
            }
            else
            {
                foreach (InteractionObjectPair pair7 in ths.SoloInteractions)
                {
                    pair7.AddInteractions(actor, allInteractionsForActor);
                }
            }
            foreach (Sim.IAddsSimInteractions interactions in Inventories.QuickDuoFind<Sim.IAddsSimInteractions, GameObject>(actor.Inventory))
            {
                interactions.GetAllInteractions(actor as Sim, ths, allInteractionsForActor);
            }
            allInteractionsForActor.Sort();
            return allInteractionsForActor;
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
                if (!AcceptCancelDialog.Show("You are about to run 'TestPopupMenu', proceed?")) return OptionResult.Failure;

                sMsg = null;

                List<InteractionObjectPair> interactions = GetAllInteractionsForActor(parameters.mActor as Sim, parameters.mTarget as Sim);

                Sims3.Gameplay.UI.PieMenu.TestAndBringUpPieMenu(parameters.mActor, new UIMouseEventArgsEx(), parameters.mHit, interactions, InteractionMenuTypes.Normal);

                Common.WriteLog(sMsg);
            }
            catch (Exception e)
            {
                GameHitParameters<GameObject>.Exception(parameters, e);
            }
            return OptionResult.SuccessClose;
        }
    }
}
