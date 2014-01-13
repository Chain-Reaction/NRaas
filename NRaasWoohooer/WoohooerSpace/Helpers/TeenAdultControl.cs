using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Interactions;
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
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Helpers
{
    public class TeenAdultControl : Common.IWorldLoadFinished
    {
        static Dictionary<ulong, Dictionary<ulong, LongTermRelationshipTypes>> sPreviousState = new Dictionary<ulong, Dictionary<ulong, LongTermRelationshipTypes>>();

        static bool sInLTRChanged = false;

        static Tracer sTracer = new Tracer();

        protected static LongTermRelationshipTypes GetPreviousState(SimDescription actor, SimDescription target)
        {
            Dictionary<ulong, LongTermRelationshipTypes> otherSims;
            if (sPreviousState.TryGetValue(actor.SimDescriptionId, out otherSims))
            {
                LongTermRelationshipTypes value;
                if (otherSims.TryGetValue(target.SimDescriptionId, out value))
                {
                    return value;
                }
            }

            return LongTermRelationshipTypes.Undefined;
        }

        public void OnWorldLoadFinished()
        {
            new Common.ImmediateEventListener(EventTypeId.kSocialInteraction, OnSocialEvent);
            new Common.ImmediateEventListener(EventTypeId.kRelationshipLTRChanged, OnLTRChanged);

            sPreviousState.Clear();

            foreach (SimDescription sim in SimListing.GetResidents(false).Values)
            {
                foreach (Relationship relation in Relationship.Get(sim))
                {
                    SetPreviousState(sim, relation.GetOtherSimDescription(sim), relation.CurrentLTR);
                }
            }
        }

        protected static void OnLTRChanged(Event e)
        {
            using (Common.TestSpan span = new Common.TestSpan(ScoringLookup.Stats, "Duration TeenAdultControl:OnLTRChanged"))
            {
                if (!sInLTRChanged)
                {
                    try
                    {
                        sInLTRChanged = true;

                        RelationshipLTRChangedEvent ltrEvent = e as RelationshipLTRChangedEvent;
                        if ((ltrEvent != null) && (ltrEvent.OldLTR != ltrEvent.RelationshipState))
                        {
                            Common.StringBuilder msg = new Common.StringBuilder("TeenAdult:OnLTRChanged");

                            Relationship relation = ltrEvent.Relationship;

                            SimDescription descA = relation.SimDescriptionA;
                            SimDescription descB = relation.SimDescriptionB;

                            bool perform = false;

                            LTRData oldLTR = LTRData.Get(ltrEvent.OldLTR);
                            if (oldLTR != null)
                            {
                                if (LongTermRelationship.RelationshipIsInappropriate(oldLTR, descA, descB, ltrEvent.OldLTR))
                                {
                                    perform = true;
                                }
                            }

                            LongTermRelationshipTypes wasRel = ltrEvent.RelationshipState;

                            msg += Common.NewLine + relation.SimDescriptionA.FullName;
                            msg += Common.NewLine + relation.SimDescriptionB.FullName;
                            msg += Common.NewLine + "Old: " + ltrEvent.OldLTR;
                            msg += Common.NewLine + "Was: " + wasRel;
                            msg += Common.NewLine + "Perform: " + perform;

                            if ((perform) && (descA.IsHuman) && (descB.IsHuman))
                            {
                                if (((descA.Teen) && (descB.YoungAdultOrAbove)) || ((descB.Teen) && (descA.YoungAdultOrAbove)))
                                {
                                    sTracer.Perform();

                                    if (sTracer.mRevert)
                                    {
                                        Perform(relation, ltrEvent.OldLTR);

                                        msg += Common.NewLine + "Now: " + ltrEvent.RelationshipState;
                                    }

                                    if (!sTracer.mIgnore)
                                    {
                                        msg += Common.NewLine + sTracer;

                                        Common.DebugStackLog(msg);
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        sInLTRChanged = false;
                    }
                }
            }
        }

        protected static void OnSocialEvent(Event e)
        {
            using (Common.TestSpan span = new Common.TestSpan(ScoringLookup.Stats, "Duration TeenAdultControl:OnSocialEvent"))
            {
                SocialEvent socialEvent = e as SocialEvent;
                if ((socialEvent != null) && (socialEvent.WasAccepted))
                {
                    Common.StringBuilder msg = new Common.StringBuilder("TeenAdult:OnSocialEvent");

                    Sim actor = socialEvent.Actor as Sim;
                    Sim target = socialEvent.TargetObject as Sim;

                    if ((actor == null) || (target == null)) return;

                    if ((!actor.SimDescription.Teen) || (actor.SimDescription.Teen == target.SimDescription.Teen)) return;

                    msg += Common.NewLine + actor.FullName;
                    msg += Common.NewLine + target.FullName;
                    msg += Common.NewLine + socialEvent.SocialName;

                    Relationship relation = Relationship.Get(actor, target, false);
                    if (relation == null) return;

                    LongTermRelationshipTypes newState = LongTermRelationshipTypes.Undefined;

                    switch (socialEvent.SocialName)
                    {
                        case "Propose Going Steady":
                            if ((actor.Partner != target.SimDescription) || (target.Partner != actor.SimDescription))
                            {
                                Relationships.SetPartner(actor.SimDescription, target.SimDescription);

                                newState = LongTermRelationshipTypes.Partner;
                            }
                            break;
                        case "Propose Marriage":
                            newState = LongTermRelationshipTypes.Fiancee;
                            break;
                        case "Have Private Wedding":
                        case "Get Married Using Wedding Arch":
                            newState = LongTermRelationshipTypes.Spouse;
                            break;
                        case "Lets Just Be Friends":
                        case "Break Up":
                            newState = LongTermRelationshipTypes.Ex;

                            SetPreviousState(actor.SimDescription, target.SimDescription, newState);
                            SetPreviousState(target.SimDescription, actor.SimDescription, newState);
                            break;
                        case "Divorce":
                            newState = LongTermRelationshipTypes.ExSpouse;

                            SetPreviousState(actor.SimDescription, target.SimDescription, newState);
                            SetPreviousState(target.SimDescription, actor.SimDescription, newState);
                            break;
                        default:
                            if (!relation.AreRomantic())
                            {
                                List<SocialRuleRHS> list = SocialRuleRHS.Get(socialEvent.SocialName);
                                if (list != null)
                                {
                                    bool romantic = false;

                                    foreach (SocialRuleRHS rhs in list)
                                    {
                                        if ((rhs.InteractionBitsAdded & LongTermRelationship.InteractionBits.Romantic) == LongTermRelationship.InteractionBits.Romantic)
                                        {
                                            romantic = true;
                                            break;
                                        }
                                    }

                                    if (romantic)
                                    {
                                        msg += Common.NewLine + "A";

                                        newState = LongTermRelationshipTypes.RomanticInterest;
                                    }
                                }
                            }
                            else
                            {
                                msg += Common.NewLine + "C";

                                newState = relation.CurrentLTR;
                            }
                            break;
                    }

                    msg += Common.NewLine + newState;

                    if (newState != LongTermRelationshipTypes.Undefined)
                    {
                        Perform(relation, newState);
                    }

                    //Common.DebugStackLog(msg);
                }
            }
        }

        protected static void SetPreviousState(SimDescription a, SimDescription b, LongTermRelationshipTypes type)
        {
            if ((!a.Teen) && (!b.Teen)) return;

            Dictionary<ulong,LongTermRelationshipTypes> otherSims;
            if (!sPreviousState.TryGetValue(a.SimDescriptionId, out otherSims))
            {
                otherSims = new Dictionary<ulong, LongTermRelationshipTypes>();
                sPreviousState[a.SimDescriptionId] = otherSims;
            }

            otherSims[b.SimDescriptionId] = type;
        }

        protected static void Perform(Relationship relation, LongTermRelationshipTypes newState)
        {
            if (relation.CurrentLTR == newState) return;

            LongTermRelationshipTypes oldState = relation.CurrentLTR;

            LTRData data = LTRData.Get(newState);
            if (data == null) return;

            bool isChangingWorlds = GameStates.sIsChangingWorlds;
            float liking = relation.LTR.Liking;
            try
            {
                GameStates.sIsChangingWorlds = true;

                relation.LTR.ChangeBitsForState(newState);
                relation.LTR.ChangeState(newState);
            }
            finally
            {
                relation.LTR.mLiking = liking;
                GameStates.sIsChangingWorlds = isChangingWorlds;
            }

            switch (newState)
            {
                case LongTermRelationshipTypes.Partner:
                case LongTermRelationshipTypes.Fiancee:
                case LongTermRelationshipTypes.Spouse:
                    relation.SimDescriptionA.SetPartner(relation.SimDescriptionB);
                    break;
            }

            relation.LTR.UpdateUI();

            SetPreviousState(relation.SimDescriptionA, relation.SimDescriptionB, newState);
            SetPreviousState(relation.SimDescriptionB, relation.SimDescriptionA, newState);
        }

        public class Tracer : StackTracer
        {
            public bool mRevert;

            public bool mIgnore;

            public Tracer()
            {
                //AddTest(typeof(SocialInteractionA), "Void CallProceduralEffectAfterUpdate", OnRevert);

                AddTest("NRaas.MasterControllerSpace.Sims.Intermediate.RelationshipOption", "Void ForceChangeState", OnAllow);
                AddTest("NRaas.StoryProgressionSpace.Managers.ManagerRomance", "Void ForceChangeState", OnAllow);

                AddTest(typeof(SocialInteractionA), "Boolean Run", OnIgnore);
                AddTest(typeof(Conversation), "Void UpdateSimAfterInteraction", OnIgnore);
            }

            public override void Reset()
            {
                mRevert = true;
                mIgnore = false;

                base.Reset();
            }

            protected bool OnAllow(StackTrace trace, StackFrame frame)
            {
                mRevert = false;

                return true;
            }

            protected bool OnIgnore(StackTrace trace, StackFrame frame)
            {
                mIgnore = true;

                return true;
            }

            public override string ToString()
            {
                string result = null;

                result += "Revert: " + mRevert;
                result += Common.NewLine + "Ignore: " + mIgnore;

                result += Common.NewLine + base.ToString();

                return result;
            }
        }
    }
}
