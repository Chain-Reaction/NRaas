using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.Dialogs;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class RelationshipOption : DualSimFromList, IIntermediateOption
    {
        public override string GetTitlePrefix()
        {
            return "RelationshipByName";
        }

        protected override string GetTitleA()
        {
            return Common.Localize("Relationship:SimA");
        }

        protected override string GetTitleB()
        {
            return Common.Localize("Relationship:SimB");
        }

        protected override int GetMaxSelectionA()
        {
            return 0;
        }

        protected override int GetMaxSelectionB(IMiniSimDescription sim)
        {
            return 0;
        }

        protected static string LocalizeStatus (SimDescription a, SimDescription b, LongTermRelationshipTypes state)
        {
            return LTRData.Get(state).GetName(a, b);
        }

        protected override bool AllowSpecies(IMiniSimDescription a, IMiniSimDescription b)
        {
            return true;
        }

        private static bool RelationshipIsInappropriate(LongTermRelationship ltr, LTRData data)
        {
            return RelationshipIsInappropriate(data, ltr.mParent.SimDescriptionA, ltr.mParent.SimDescriptionB, ltr.CurrentLTR);
        }
        private static bool RelationshipIsInappropriate(LTRData data, IMiniSimDescription a, IMiniSimDescription b, LongTermRelationshipTypes currentLTR)
        {
            if (!data.IsRomantic)
            {
                return false;
            }

            if ((a.ChildOrBelow) || (b.ChildOrBelow))
            {
                return true;
            }

            return false;
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            Relationship relation = Relationship.Get(a, b, true);
            if (relation == null) return true;

            LongTermRelationshipTypes currentState = relation.LTR.CurrentLTR;

            List<NewLevel> allOptions = new List<NewLevel>();

            int count = 0;

            LongTermRelationshipTypes newState = NextNegativeEnemyState(currentState);
            while ((count < 25) && (newState != LongTermRelationshipTypes.Undefined))
            {
                count++;

                LTRData level = LTRData.Get(newState);
                if ((level != null) && (!RelationshipIsInappropriate(relation.LTR, level)))
                {
                    allOptions.Insert(0, new NewLevel(newState, false, false));
                }

                newState = NextNegativeEnemyState(newState);
            }

            if (SimTypes.IsEquivalentSpecies(a, b))
            {
                newState = NextNegativeRomanceState(currentState);
                while ((count < 25) && (newState != LongTermRelationshipTypes.Undefined))
                {
                    count++;

                    LTRData level = LTRData.Get(newState);
                    if ((level != null) && (!RelationshipIsInappropriate(relation.LTR, level)))
                    {
                        allOptions.Insert(0, new NewLevel(newState, false, true));
                    }

                    newState = NextNegativeRomanceState(newState);
                }
            }

            allOptions.Add(new NewLevel(currentState, true, false));

            newState = NextPositiveFriendState(currentState);
            while ((count < 25) && (newState != LongTermRelationshipTypes.Undefined))
            {
                count++;

                LTRData level = LTRData.Get(newState);
                if ((level != null) && (!RelationshipIsInappropriate(relation.LTR, level)))
                {
                    allOptions.Add(new NewLevel(newState, true, false));
                }

                newState = NextPositiveFriendState(newState);
            }

            if (SimTypes.IsEquivalentSpecies(a, b))
            {
                newState = NextPositiveRomanceState(currentState);
                while ((count < 25) && (newState != LongTermRelationshipTypes.Undefined))
                {
                    count++;

                    LTRData level = LTRData.Get(newState);
                    if ((level != null) && (!RelationshipIsInappropriate(relation.LTR, level)))
                    {
                        allOptions.Add(new NewLevel(newState, true, true));
                    }

                    newState = NextPositiveRomanceState(newState);
                }
            }

            Dictionary<LongTermRelationshipTypes, bool> states = new Dictionary<LongTermRelationshipTypes, bool>();

            List<NewLevel> options = new List<NewLevel>();

            foreach (NewLevel level in allOptions)
            {
                if (level.mState == LongTermRelationshipTypes.Undefined) continue;

                if (states.ContainsKey(level.mState)) continue;
                states.Add(level.mState, true);

                count++;

                level.Count = count;

                options.Add(level);
            }

            NewLevel choice = new RelationshipSelection(a.FullName, options, a, b).SelectSingle();
            if (choice == null) return false;

            List<string> selection = new List<string>();

            if (choice.mState == currentState) return true;

            Dictionary<LongTermRelationshipTypes, bool> lookup = new Dictionary<LongTermRelationshipTypes, bool>();
            lookup.Add(relation.LTR.CurrentLTR, true);

            bool bFirst = true;
            if (choice.mUp)
            {
                int recurseCount = 0;
                while ((recurseCount < 25) && (BumpUp(a, b, bFirst, choice.mRomantic)))
                {
                    recurseCount++;

                    if (choice.mState == relation.LTR.CurrentLTR) return true;

                    if (lookup.ContainsKey(relation.LTR.CurrentLTR)) return true;
                    lookup.Add(relation.LTR.CurrentLTR, true);

                    bFirst = false;
                }
            }
            else
            {
                int recurseCount = 0;
                while ((recurseCount < 25) && (BumpDown(a, b, bFirst, choice.mRomantic)))
                {
                    recurseCount++;

                    if (choice.mState == relation.LTR.CurrentLTR) return true;

                    if (lookup.ContainsKey(relation.LTR.CurrentLTR)) return true;
                    lookup.Add(relation.LTR.CurrentLTR, true);

                    bFirst = false;
                }
            }

            return true;
        }

        public static LongTermRelationshipTypes NextPositiveFriendState(LongTermRelationshipTypes currentState)
        {
            switch (currentState)
            {
                case LongTermRelationshipTypes.Ex:
                case LongTermRelationshipTypes.ExSpouse:
                case LongTermRelationshipTypes.OldEnemies:
                case LongTermRelationshipTypes.Enemy:
                case LongTermRelationshipTypes.Disliked:
                case LongTermRelationshipTypes.Stranger:
                    return LongTermRelationshipTypes.Acquaintance;

                case LongTermRelationshipTypes.Acquaintance:
                case LongTermRelationshipTypes.DistantFriend:
                case LongTermRelationshipTypes.RomanticInterest:
                    return LongTermRelationshipTypes.Friend;

                case LongTermRelationshipTypes.Friend:
                    return LongTermRelationshipTypes.GoodFriend;

                case LongTermRelationshipTypes.GoodFriend:
                case LongTermRelationshipTypes.Partner:
                case LongTermRelationshipTypes.Fiancee:
                case LongTermRelationshipTypes.Spouse:
                    return LongTermRelationshipTypes.BestFriend;

                case LongTermRelationshipTypes.BestFriend:
                    return LongTermRelationshipTypes.BestFriendsForever;

                default:
                    return LongTermRelationshipTypes.Undefined;
            }
        }

        public static LongTermRelationshipTypes NextPositiveRomanceState(LongTermRelationshipTypes currentState)
        {
            switch (currentState)
            {
                case LongTermRelationshipTypes.RomanticInterest:
                    return LongTermRelationshipTypes.Partner;

                case LongTermRelationshipTypes.Partner:
                    return LongTermRelationshipTypes.Fiancee;

                case LongTermRelationshipTypes.Fiancee:
                    return LongTermRelationshipTypes.Spouse;

                case LongTermRelationshipTypes.Spouse:
                    return LongTermRelationshipTypes.Undefined;

                default:
                    return LongTermRelationshipTypes.RomanticInterest;
            }
        }

        public static LongTermRelationshipTypes NextNegativeEnemyState(LongTermRelationshipTypes currentState)
        {
            switch (currentState)
            {
                case LongTermRelationshipTypes.OldFriend:
                case LongTermRelationshipTypes.BestFriend:
                case LongTermRelationshipTypes.BestFriendsForever:
                case LongTermRelationshipTypes.Partner:
                case LongTermRelationshipTypes.Fiancee:
                case LongTermRelationshipTypes.Spouse:
                    return LongTermRelationshipTypes.Friend;

                case LongTermRelationshipTypes.RomanticInterest:
                case LongTermRelationshipTypes.Friend:
                case LongTermRelationshipTypes.GoodFriend:
                    return LongTermRelationshipTypes.Acquaintance;

                case LongTermRelationshipTypes.Ex:
                case LongTermRelationshipTypes.ExSpouse:
                case LongTermRelationshipTypes.Stranger:
                case LongTermRelationshipTypes.Acquaintance:
                case LongTermRelationshipTypes.DistantFriend:
                    return LongTermRelationshipTypes.Enemy;

                case LongTermRelationshipTypes.OldEnemies:
                case LongTermRelationshipTypes.Enemy:
                case LongTermRelationshipTypes.Disliked:
                    return LongTermRelationshipTypes.Undefined;
            }
            return LongTermRelationshipTypes.Undefined;
        }

        public static LongTermRelationshipTypes NextNegativeRomanceState(LongTermRelationshipTypes currentState)
        {
            switch (currentState)
            {
                case LongTermRelationshipTypes.Partner:
                case LongTermRelationshipTypes.Fiancee:
                    return LongTermRelationshipTypes.Ex;

                case LongTermRelationshipTypes.Spouse:
                    return LongTermRelationshipTypes.ExSpouse;

                case LongTermRelationshipTypes.Acquaintance:
                    return LongTermRelationshipTypes.Stranger;

                case LongTermRelationshipTypes.Stranger:
                    return LongTermRelationshipTypes.Undefined;

                default:
                    return LongTermRelationshipTypes.Acquaintance;
            }
        }

        protected static void ForceChangeState(Relationship relation, LongTermRelationshipTypes state)
        {
            LongTermRelationship.InteractionBits bits = relation.LTR.LTRInteractionBits & (LongTermRelationship.InteractionBits.HaveBeenBestFriends | LongTermRelationship.InteractionBits.HaveBeenFriends | LongTermRelationship.InteractionBits.HaveBeenPartners);

            LTRData data = LTRData.Get(state);
            if (relation.LTR.RelationshipIsInappropriate(data))
            {
                relation.LTR.ChangeBitsForState(state);
                relation.LTR.ChangeState(state);
                relation.LTR.UpdateUI();
            }
            else
            {
                relation.LTR.ForceChangeState(state);
            }

            if (state == LongTermRelationshipTypes.Spouse)
            {
                relation.SimDescriptionA.Genealogy.Marry(relation.SimDescriptionB.Genealogy);

                MidlifeCrisisManager.OnBecameMarried(relation.SimDescriptionA, relation.SimDescriptionB);

                relation.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.Divorce);
                relation.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.Marry);

                relation.SetMarriedInGame();

                if (SeasonsManager.Enabled)
                {
                    relation.WeddingAnniversary = new WeddingAnniversary(SeasonsManager.CurrentSeason, (int)SeasonsManager.DaysElapsed);
                    relation.WeddingAnniversary.SimA = relation.SimDescriptionA;
                    relation.WeddingAnniversary.SimB = relation.SimDescriptionB;
                    relation.WeddingAnniversary.CreateAlarm();
                }
            }

            relation.LTR.AddInteractionBit(bits);
        }

        protected static bool BumpUp(SimDescription a, SimDescription b, bool prompt, bool romantic)
        {
            Relationship relation = Relationship.Get(a, b, true);
            if (relation == null) return false;

            LongTermRelationshipTypes currentState = relation.LTR.CurrentLTR;

            LongTermRelationshipTypes nextState = LongTermRelationshipTypes.Undefined;
            if (romantic)
            {
                nextState = NextPositiveRomanceState(currentState);
            }
            else
            {
                nextState = NextPositiveFriendState(currentState);
            }

            if (nextState == LongTermRelationshipTypes.Undefined)
            {
                if (prompt)
                {
                    SimpleMessageDialog.Show(Common.Localize("Romance:BumpUpTitle"), Common.Localize("Romance:TooHigh"));
                }
                return false;
            }

            /*
            if (relation.LTR.RelationshipIsInappropriate(LTRData.Get(nextState)))
            {
                if (prompt)
                {
                    SimpleMessageDialog.Show(Common.Localize("Romance:BumpUpTitle"), Common.Localize ("Romance:Improper", new object[] { Common.LocalizeStatus (a, b, nextState) }));
                }
                return false;
            }
            */

            if ((romantic) && (a.Genealogy.IsBloodRelated(b.Genealogy)))
            {
                if ((prompt) && (!AcceptCancelDialog.Show(Common.Localize("Romance:BloodPrompt", a.IsFemale, new object [] { a, b }))))
                {
                    return false;
                }
            }

            if ((currentState == LongTermRelationshipTypes.RomanticInterest) && (romantic))
            {
                if ((a.Partner != null) && (a.Partner != b))
                {
                    if ((b.Partner != null) && (b.Partner != a))
                    {
                        if ((prompt) && (!AcceptCancelDialog.Show(Common.Localize("Romance:DualPartnerPrompt", a.IsFemale, new object[] { a, b }))))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if ((prompt) && (!AcceptCancelDialog.Show(Common.Localize("Romance:PartnerPrompt", a.IsFemale, new object[] { a }))))
                        {
                            return false;
                        }
                    }
                }
                else if ((b.Partner != null) && (b.Partner != a))
                {
                    if ((prompt) && (!AcceptCancelDialog.Show(Common.Localize("Romance:PartnerPrompt", b.IsFemale, new object[] { b }))))
                    {
                        return false;
                    }
                }

                if (a.Partner != null)
                {
                    BumpDown(a, a.Partner, false, true);
                }

                if (b.Partner != null)
                {
                    BumpDown(b, b.Partner, false, true);
                }

                if (a.TraitManager == null)
                {
                    a.Fixup();
                }

                if (b.TraitManager == null)
                {
                    b.Fixup();
                }

                Relationships.SetPartner(a, b);               
            }            

            ForceChangeState(relation, nextState);

            if (romantic)
            {
                if (prompt && relation.RomanceVisibilityState != null)
                {
                    long time = 0;
                    relation.TryGetActiveRomanceStartTime(out time);

                    int days = 0;
                    if (time != 0)
                    {
                        days = (int)SimClock.ElapsedTime(TimeUnit.Days, new DateAndTime(time));
                    }

                    string text = StringInputDialog.Show(Common.Localize("Romance:StartTime"), Common.Localize("Romance:StartPrompt", a.IsFemale, new object[] { a, b, SimClock.ElapsedCalendarDays() }), days.ToString());
                    if (string.IsNullOrEmpty(text)) return false;

                    int mValue = 0;                    
                    if (!int.TryParse(text, out mValue) || mValue > SimClock.ElapsedCalendarDays())
                    {                        
                        SimpleMessageDialog.Show(Common.Localize("Romance:StartTime"), Common.Localize("Numeric:ErrorInputIgnored"));
                    }
                    else
                    {
                        relation.RomanceVisibilityState.mStartTime = SimClock.Subtract(SimClock.CurrentTime(), TimeUnit.Days, (float)mValue);
                    }
                }
            }

            if (relation.LTR.CurrentLTR == LongTermRelationshipTypes.BestFriendsForever)
            {
                bool isPetBFF = ((!a.IsHuman) || (!b.IsHuman));

                a.HasBFF = true;
                b.HasBFF = true;
                FindAndRemoveBFF(a, b, isPetBFF);
                FindAndRemoveBFF(b, a, isPetBFF);
            }

            if (currentState == relation.LTR.CurrentLTR) return false;

            StyledNotification.Format format = new StyledNotification.Format(Common.Localize("Romance:Success", a.IsFemale, new object[] { a, b, LocalizeStatus(a, b, relation.LTR.CurrentLTR) }), StyledNotification.NotificationStyle.kGameMessagePositive);
            format.mTNSCategory = NotificationManager.TNSCategory.Lessons;
            StyledNotification.Show(format);
            return true;
        }

        private static void FindAndRemoveBFF(SimDescription actor, SimDescription target, bool isPetBFF)
        {
            foreach (Relationship relationship in Relationship.Get(actor))
            {
                bool isPetRel = relationship.IsPetRel;
                if ((!isPetBFF && !isPetRel) || (isPetBFF && isPetRel))
                {
                    SimDescription otherSimDescription = relationship.GetOtherSimDescription(actor);
                    if (((relationship.LTR.LTRInteractionBits & LongTermRelationship.InteractionBits.BFF) != LongTermRelationship.InteractionBits.None) && (otherSimDescription != target))
                    {
                        relationship.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.BFF);

                        if (!isPetBFF)
                        {
                            relationship.LTR.UpdateLiking(LongTermRelationship.kBFFLostLikingPenalty);
                            otherSimDescription.HasBFF = false;
                            relationship.STC.Update(actor, otherSimDescription, CommodityTypes.Insulting, 50f);
                        }
                    }
                }
            }
        }

        protected static bool BumpDown(SimDescription a, SimDescription b, bool prompt, bool romantic)
        {
            Relationship relation = Relationship.Get(a, b, true);
            if (relation == null) return false;

            LongTermRelationshipTypes currentState = relation.LTR.CurrentLTR;

            LongTermRelationshipTypes nextState = LongTermRelationshipTypes.Undefined;
            if (romantic)
            {
                nextState = NextNegativeRomanceState(currentState);
            }
            else
            {
                nextState = NextNegativeEnemyState(currentState);
            }

            if (nextState == LongTermRelationshipTypes.Undefined)
            {
                if (prompt)
                {
                    SimpleMessageDialog.Show(Common.Localize("Romance:BumpDownTitle"), Common.Localize("Romance:TooLow"));
                }
                return false;
            }

            /*
            if (relation.LTR.RelationshipIsInappropriate(LTRData.Get(nextState)))
            {
                if (prompt)
                {
                    SimpleMessageDialog.Show(Common.Localize("Romance:BumpDownTitle"), Common.Localize ("Romance:Improper", new object[] { Common.LocalizeStatus (a, b, nextState) }));
                }
                return false;
            }
            */

            if ((a.Partner == b) || (b.Partner == a))
            {
                try
                {
                    a.Partner = null;
                }
                catch
                { }

                try
                {
                    b.Partner = null;
                }
                catch
                { }
            }

            ForceChangeState(relation, nextState);

            if (nextState == LongTermRelationshipTypes.Stranger)
            {
                Relationship.RemoveRelationship(relation);
            }
            else
            {
                if (currentState == relation.LTR.CurrentLTR) return false;
            }

            StyledNotification.Format format = new StyledNotification.Format(Common.Localize("Romance:Success", a.IsFemale, new object[] { a, b, LocalizeStatus(a, b, nextState) }), StyledNotification.NotificationStyle.kGameMessagePositive);
            format.mTNSCategory = NotificationManager.TNSCategory.Lessons;
            StyledNotification.Show(format);
            return true;
        }

        protected class NewLevel
        {
            public readonly bool mUp;
            public readonly bool mRomantic;
            public readonly LongTermRelationshipTypes mState;

            int mCount;

            public NewLevel(LongTermRelationshipTypes state, bool up, bool romantic)
            {
                mState = state;
                mUp = up;
                mRomantic = romantic;
            }

            public int Count
            {
                get { return mCount; }
                set { mCount = value; }
            }
        }

        protected class RelationshipSelection : ProtoSelection<NewLevel>
        {
            SimDescription mB;

            public RelationshipSelection(string title, ICollection<NewLevel> options, SimDescription a, SimDescription b)
                : base(title, b.FullName, options)
            {
                mB = b;

                AddColumn(new NameColumn(a, b));
            }

            public class NameColumn : ObjectPickerDialogEx.CommonHeaderInfo<NewLevel>
            {
                SimDescription mA;
                SimDescription mB;

                public NameColumn(SimDescription a, SimDescription b)
                    : base("NRaas.MasterController.Romance:ListHeader", "NRaas.MasterController.Romance:ListTooltip", 370)
                {
                    mA = a;
                    mB = b;
                }

                public override ObjectPicker.ColumnInfo GetValue(NewLevel item)
                {
                    return new ObjectPicker.TextColumn(Common.Localize("Romance:Element", false, new object[] { item.Count, LocalizeStatus(mA, mB, item.mState) }));
                }
            }
        }
    }
}
