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
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Helpers
{
    public class AttractionHelper : Common.IWorldLoadFinished
    {
        [PersistableStatic]
        static Dictionary<ulong, Dictionary<ulong, LastCheck>> sLastCheck = new Dictionary<ulong, Dictionary<ulong, LastCheck>>();

        static Household sOldHousehold;

        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kSocialInteraction, OnSocialEvent);
            new Common.DelayedEventListener(EventTypeId.kHouseholdSelected, OnHouseSelected);

            sOldHousehold = Household.ActiveHousehold;
        }

        protected static void SetLastCheck(ulong simA, ulong simB, LastCheck check)
        {
            Dictionary<ulong, LastCheck> lookup;
            if (!sLastCheck.TryGetValue(simA, out lookup))
            {
                lookup = new Dictionary<ulong, LastCheck>();
                sLastCheck.Add(simA, lookup);
            }

            lookup[simB] = check;        
        }

        public static void UpdateAttractionControllers()
        {
            UpdateAttractionControllers(Household.ActiveHousehold, Household.ActiveHousehold);
        }
        protected static void UpdateAttractionControllers(Household newHouse, Household oldHouse)
        {
            Dictionary<Relationship, bool> toDispose = new Dictionary<Relationship, bool>();

            if (oldHouse != null)
            {
                foreach (SimDescription sim in Households.All(oldHouse))
                {
                    foreach (Relationship relation in Relationship.Get(sim))
                    {
                        if (relation.AttractionNPCController != null)
                        {
                            if (!toDispose.ContainsKey(relation))
                            {
                                toDispose.Add(relation, true);
                            }
                        }
                    }
                }
            }

            if (newHouse != null)
            {
                foreach (SimDescription sim in Households.All(newHouse))
                {
                    foreach (Relationship relation in Relationship.Get(sim))
                    {
                        if ((!SimTypes.IsSelectable(relation.SimDescriptionA)) && (!SimTypes.IsSelectable(relation.SimDescriptionB))) continue;

                        if (relation.AreAttracted)
                        {
                            if (TestEnableAttractionNPCController(relation))
                            {
                                toDispose.Remove(relation);
                                if (relation.AttractionNPCController == null)
                                {
                                    relation.AttractionNPCController = new AttractionNPCBehaviorController(relation);
                                }
                            }
                        }
                    }
                }
            }

            foreach (Relationship relation in toDispose.Keys)
            {
                relation.AttractionNPCController.Dispose();
            }
        }

        protected static void OnHouseSelected(Event e)
        {
            HouseholdUpdateEvent houseEvent = e as HouseholdUpdateEvent;
            if (houseEvent == null) return;

            UpdateAttractionControllers(houseEvent.Household, sOldHousehold);

            sOldHousehold = houseEvent.Household;
        }

        public static bool TestEnableAttractionNPCController(Relationship relation)
        {
            if (relation == null) return false;

            GreyedOutTooltipCallback greyedOutTooltipCallback = null;
            if (!CommonSocials.SatisfiedInteractionLevel(relation.SimDescriptionA, relation.SimDescriptionB, true, ref greyedOutTooltipCallback)) return false;

            return true;
        }

        protected static LastCheck GetLastCheck(ulong simA, ulong simB, bool createNew, out bool created)
        {
            Dictionary<ulong, LastCheck> lookup;
            if (!sLastCheck.TryGetValue(simA, out lookup))
            {
                lookup = new Dictionary<ulong, LastCheck>();
                sLastCheck.Add(simA, lookup);
            }

            created = false;

            LastCheck lastCheck;
            if (!lookup.TryGetValue(simB, out lastCheck))
            {
                if (createNew)
                {
                    created = true;
                    lastCheck = new LastCheck();
                    lookup.Add(simB, lastCheck);
                }
            }

            return lastCheck;
        }

        protected static void OnSocialEvent(Event e)
        {
            using (Common.TestSpan span = new Common.TestSpan(ScoringLookup.Stats, "Duration AttractionHelper:OnSocialEvent"))
            {
                SocialEvent socialEvent = e as SocialEvent;
                if ((socialEvent != null) && (socialEvent.WasAccepted))
                {
                    Sim actor = socialEvent.Actor as Sim;
                    Sim target = socialEvent.TargetObject as Sim;

                    if ((actor != null) && (target != null))
                    {
                        bool created = false;
                        LastCheck lastCheck = GetLastCheck(actor.SimDescription.SimDescriptionId, target.SimDescription.SimDescriptionId, true, out created);

                        if (created)
                        {
                            SetLastCheck(target.SimDescription.SimDescriptionId, actor.SimDescription.SimDescriptionId, lastCheck);
                        }

                        bool force = false;
                        if ((!lastCheck.mAttractionNotice) && ((SimTypes.IsSelectable(actor)) || (SimTypes.IsSelectable(target))))
                        {
                            force = true;
                        }

                        if ((force) || ((lastCheck.mTime + SimClock.kSimulatorTicksPerSimDay) > SimClock.CurrentTicks))
                        {
                            lastCheck.mTime = SimClock.CurrentTicks;

                            Relationship relation = Relationship.Get(actor, target, false);
                            if (relation != null)
                            {
                                RelationshipEx.CalculateAttractionScore(relation, !lastCheck.mAttractionNotice);

                                if ((SimTypes.IsSelectable(actor)) || (SimTypes.IsSelectable(target)))
                                {
                                    lastCheck.mAttractionNotice = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        [Persistable]
        public class LastCheck
        {
            public long mTime;

            public bool mAttractionNotice;

            public LastCheck()
            {
                mTime = SimClock.CurrentTicks;
                mAttractionNotice = false;
            }
        }
    }
}
