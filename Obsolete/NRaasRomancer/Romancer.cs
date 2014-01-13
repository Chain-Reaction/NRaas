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
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class Romancer
    {
        [Tunable]
        protected static bool kInstantiator = false;

        [Persistable(false)]
        private static EventListener sBoughtObjectLister = null;

        static Romancer()
        {
            World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinishedHandler);
        }
        public Romancer()
        { }

        public static void AddInteractions(Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas obj)
        {
            foreach (InteractionObjectPair pair in obj.Interactions)
            {
                if (pair.InteractionDefinition.GetType() == Version.Singleton.GetType())
                {
                    return;
                }
            }

            obj.AddInteraction(SetRelationshp.Singleton);
            obj.AddInteraction(HigherLevel.Singleton);
            obj.AddInteraction(LowerLevel.Singleton);
            obj.AddInteraction(SetLevel.Singleton);
            obj.AddInteraction(StatePartner.Singleton);
            obj.AddInteraction(Version.Singleton);
        }

        public static void OnWorldLoadFinishedHandler(object sender, EventArgs e)
        {
            List<Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas> others = new List<Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas>(Sims3.Gameplay.Queries.GetObjects<Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas>());
            foreach (Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas obj in others)
            {
                AddInteractions(obj);
            }

            sBoughtObjectLister = EventTracker.AddListener(EventTypeId.kBoughtObject, new ProcessEventDelegate(OnObjectBought));
        }

        protected static ListenerAction OnObjectBought(Sims3.Gameplay.EventSystem.Event e)
        {
            if (e.Id == EventTypeId.kBoughtObject)
            {
                Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas obj = e.TargetObject as Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas;
                if (obj != null)
                {
                    AddInteractions(obj);
                }
            }

            return ListenerAction.Keep;
        }

        public class Version : ImmediateInteraction<Sim, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new VersionDefinition();

            public Version()
            { }

            // Nested Types
            [DoesntRequireTuning]
            private sealed class VersionDefinition : ImmediateInteractionDefinition<Sim, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas, Version>
            {
                public override string[] GetPath()
                {
                    return new string[] { "Romancer..." };
                }

                protected override string GetInteractionName(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, InteractionObjectPair interaction)
                {
                    return "Version";
                }

                protected override bool Test(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    return true;
                }
            }

            protected override bool Run()
            {
                SimpleMessageDialog.Show("Romancer Version", "Version 3");
                return true;
            }
        }

        public abstract class RelationshpBase : ImmediateInteraction<Sim, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas>
        {
            public RelationshpBase()
            {}

            protected abstract bool AllowChildren();

            protected abstract bool AllowMe();

            protected abstract bool Run(SimDescription a, SimDescription b);

            protected override bool Run()
            {
                List<PhoneSimPicker.SimPickerInfo> sims = ListOfAllSims(base.Actor, AllowChildren (), AllowMe ());
                if ((sims == null) || (sims.Count == 0))
                {
                    SimpleMessageDialog.Show("Romancer", "There is no one available on the lot to list.");
                    return false;
                }

                List<object> list = PhoneSimPicker.Show(true, ModalDialog.PauseMode.PauseSimulator, sims, "Select Other", "Select", "Cancel");
                if ((list == null) || (list.Count == 0))
                {
                    return false;
                }

                SimDescription other = list[0] as SimDescription;

                return Run(base.Actor.SimDescription, other);
            }
        }

        public class SetRelationshp : RelationshpBase
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            public SetRelationshp()
            { }

            // Nested Types
            [DoesntRequireTuning]
            private sealed class Definition : ImmediateInteractionDefinition<Sim, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas, SetRelationshp>
            {
                public override string[] GetPath()
                {
                    return new string[] { "Romancer..." };
                }

                protected override string GetInteractionName(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, InteractionObjectPair interaction)
                {
                    return "Set Relationship...";
                }

                protected override bool Test(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    return true;
                }
            }

            protected override bool AllowChildren()
            {
                return true;
            }

            protected override bool AllowMe()
            {
                return false;
            }

            protected override bool Run(SimDescription a, SimDescription b)
            {
                Relationship relation = Relationship.Get(a, b, true);
                if (relation == null) return false;

                string text = StringInputDialog.Show("Change Long Term Relationship", "Enter the amount to change the existing long term relationship between\n" + a.FirstName + " " + a.LastName + "\nand\n" + b.FirstName + " " + b.LastName + "\n\nRange is -100 to 100", relation.LTR.Liking.ToString ("F1"), 256, StringInputDialog.Validation.None);
                if ((text == null) || (text == "")) return false;

                float value = 0f;
                if (!float.TryParse(text, out value))
                {
                    SimpleMessageDialog.Show("Change Long Term Relationship", "Value '" + text + "' could not be converted to a number.");
                    return false;
                }

                relation.LTR.SetLiking (value);
                return true;
            }
        }

        public class SetLevel : RelationshpBase
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            public SetLevel()
            { }

            // Nested Types
            [DoesntRequireTuning]
            private sealed class Definition : ImmediateInteractionDefinition<Sim, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas, SetLevel>
            {
                public override string[] GetPath()
                {
                    return new string[] { "Romancer..." };
                }

                protected override string GetInteractionName(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, InteractionObjectPair interaction)
                {
                    return "Set to Level...";
                }

                protected override bool Test(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    if (a.SimDescription.ChildOrBelow) return false;

                    return true;
                }
            }

            protected override bool AllowChildren()
            {
                return false;
            }

            protected override bool AllowMe()
            {
                return false;
            }

            protected class NewLevel
            {
                public bool mUp;
                public string mState;

                public NewLevel(string state, bool up)
                {
                    mState = state;
                    mUp = up;
                }
            }

            protected override bool Run(SimDescription a, SimDescription b)
            {
                Relationship relation = Relationship.Get(a, b, true);
                if (relation == null) return false;

                string currentState = relation.LTR.CurrentLTR;

                List<NewLevel> allOptions = new List<NewLevel>();

                string newState = ChangeRelationship.NextNegativeRomanceState(currentState);
                while (newState != null)
                {
                    if (!relation.LTR.RelationshipIsInappropriate(LTRData.Get(newState)))
                    {
                        allOptions.Insert(0, new NewLevel(newState, false));
                    }

                    newState = ChangeRelationship.NextNegativeRomanceState(newState);
                }

                allOptions.Add(new NewLevel (currentState, true));

                if (currentState == "Stranger")
                {
                    newState = "Acquaintance";
                    allOptions.Add(new NewLevel(newState, true));
                }
                else
                {
                    newState = currentState;
                }

                newState = ChangeRelationship.NextPositiveRomanceState(newState);
                while (newState != null)
                {
                    if (!relation.LTR.RelationshipIsInappropriate(LTRData.Get(newState)))
                    {
                        allOptions.Add(new NewLevel(newState, true));
                    }

                    newState = ChangeRelationship.NextPositiveRomanceState(newState);
                }

                List<ObjectPicker.HeaderInfo> headers = new List<ObjectPicker.HeaderInfo>();
                headers.Add(new ObjectPicker.HeaderInfo("Level", "Relationship Level", 230));

                int count = 0;

                List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();
                foreach (NewLevel level in allOptions)
                {
                    ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(level, new List<ObjectPicker.ColumnInfo>());

                    count++;

                    item.ColumnInfo.Add(new ObjectPicker.TextColumn(count.ToString () + ". " + level.mState));

                    rowInfo.Add(item);
                }

                List<ObjectPicker.TabInfo> tabInfo = new List<ObjectPicker.TabInfo>();
                tabInfo.Add(new ObjectPicker.TabInfo("shop_all_r2", Localization.LocalizeString("Ui/Caption/ObjectPicker:All", new object[0]), rowInfo));

                string buttonTrue = "Ok";
                string buttonFalse = Localization.LocalizeString("Ui/Caption/ObjectPicker:Cancel", new object[0]);

                List<ObjectPicker.RowInfo> list = ObjectPickerDialog.Show(true, ModalDialog.PauseMode.PauseSimulator, "Relationship", buttonTrue, buttonFalse, tabInfo, headers, 1, new Vector2(-1f, -1f), true);

                List<string> selection = new List<string>();

                if ((list == null) || (list.Count == 0)) return false;

                NewLevel choice = list[0].Item as NewLevel;
                if (choice == null) return false;

                if (choice.mState == currentState) return false;

                bool bFirst = true;
                if (choice.mUp)
                {
                    while (BumpUp(a, b, bFirst))
                    {
                        if (choice.mState == relation.LTR.CurrentLTR) return true;

                        if (currentState == relation.LTR.CurrentLTR) return false;
                        currentState = relation.LTR.CurrentLTR;

                        bFirst = false;
                    }
                }
                else
                {
                    while (BumpDown(a, b, bFirst))
                    {
                        if (choice.mState == relation.LTR.CurrentLTR) return true;

                        if (currentState == relation.LTR.CurrentLTR) return false;
                        currentState = relation.LTR.CurrentLTR;

                        bFirst = false;
                    }
                }

                return true;
            }
        }

        protected static bool BumpUp(SimDescription a, SimDescription b, bool prompt)
        {
            Relationship relation = Relationship.Get(a, b, true);
            if (relation == null) return false;

            string currentState = relation.LTR.CurrentLTR;

            string nextState = null;
            if (currentState == "Stranger")
            {
                nextState = "Acquaintance";
            }
            else
            {
                nextState = ChangeRelationship.NextPositiveRomanceState(currentState);
            }

            if (nextState == null)
            {
                if (prompt)
                {
                    SimpleMessageDialog.Show("Bump to Next Level", "There is no way to go higher than these sims already are.");
                }
                return false;
            }

            if (relation.LTR.RelationshipIsInappropriate(LTRData.Get(nextState)))
            {
                if (prompt)
                {
                    SimpleMessageDialog.Show("Bump to Next Level", "A relationship of " + nextState + " is unavailable to the selected sims.");
                }
                return false;
            }

            if (a.Genealogy.IsBloodRelated(b.Genealogy))
            {
                if ((prompt) && (!AcceptCancelDialog.Show(a.FirstName + " " + a.LastName + " and " + b.FirstName + " " + b.LastName + " are blood related.  Proceed?")))
                {
                    return false;
                }
            }

            if (currentState == "Romantic Interest")
            {
                if ((a.Partner != null) && (a.Partner != b))
                {
                    if ((b.Partner != null) && (b.Partner != a))
                    {
                        if ((prompt) && (!AcceptCancelDialog.Show("Both " + a.FirstName + " " + a.LastName + " and " + b.FirstName + " " + b.LastName + " already have different partners.  Proceed?")))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if ((prompt) && (!AcceptCancelDialog.Show(a.FirstName + " " + a.LastName + " already has a different partner.  Proceed?")))
                        {
                            return false;
                        }
                    }
                }
                else if ((b.Partner != null) && (b.Partner != a))
                {
                    if ((prompt) && (!AcceptCancelDialog.Show(b.FirstName + " " + b.LastName + " already has a different partner.  Proceed?")))
                    {
                        return false;
                    }
                }

                if (a.Partner != null)
                {
                    BumpDown(a, a.Partner, false);
                }
                if (b.Partner != null)
                {
                    BumpDown(b, b.Partner, false);
                }

                a.SetPartner(b);
            }

            relation.LTR.ForceChangeState(nextState);

            if (currentState == relation.LTR.CurrentLTR) return false;

            StyledNotification.Show(new StyledNotification.Format("Relationship between " + a.FirstName + " " + a.LastName + " and " + b.FirstName + " " + b.LastName + " changed to " + nextState, StyledNotification.NotificationStyle.kAlert));
            return true;
        }

        public class HigherLevel : RelationshpBase
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            public HigherLevel()
            { }

            // Nested Types
            [DoesntRequireTuning]
            private sealed class Definition : ImmediateInteractionDefinition<Sim, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas, HigherLevel>
            {
                public override string[] GetPath()
                {
                    return new string[] { "Romancer..." };
                }

                protected override string GetInteractionName(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, InteractionObjectPair interaction)
                {
                    return "Bump to Higher Level...";
                }

                protected override bool Test(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    if (a.SimDescription.ChildOrBelow) return false;

                    return true;
                }
            }

            protected override bool AllowChildren()
            {
                return false;
            }

            protected override bool AllowMe()
            {
                return false;
            }

            protected override bool Run(SimDescription a, SimDescription b)
            {
                BumpUp(a, b, true);
                return true;
            }
        }

        public class LowerLevel : RelationshpBase
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            public LowerLevel()
            { }

            // Nested Types
            [DoesntRequireTuning]
            private sealed class Definition : ImmediateInteractionDefinition<Sim, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas, LowerLevel>
            {
                public override string[] GetPath()
                {
                    return new string[] { "Romancer..." };
                }

                protected override string GetInteractionName(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, InteractionObjectPair interaction)
                {
                    return "Bump to Lower Level...";
                }

                protected override bool Test(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    if (a.SimDescription.ChildOrBelow) return false;

                    return true;
                }
            }

            protected override bool AllowChildren()
            {
                return false;
            }

            protected override bool AllowMe()
            {
                return false;
            }

            protected override bool Run(SimDescription a, SimDescription b)
            {
                return BumpDown(a, b, true);
            }
        }

        protected static bool BumpDown(SimDescription a, SimDescription b, bool prompt)
        {
            Relationship relation = Relationship.Get(a, b, true);
            if (relation == null) return false;

            string currentState = relation.LTR.CurrentLTR;

            string nextState = ChangeRelationship.NextNegativeRomanceState(currentState);
            if (nextState == null)
            {
                if (prompt)
                {
                    SimpleMessageDialog.Show("Bump to Lower Level", "There is no way to go lower than these sims already are.");
                }
                return false;
            }

            if (relation.LTR.RelationshipIsInappropriate(LTRData.Get(nextState)))
            {
                if (prompt)
                {
                    SimpleMessageDialog.Show("Bump to Lower Level", "A relationship of " + nextState + " is unavailable to the selected sims.");
                }
                return false;
            }

            if (a.Partner == b)
            {
                a.Partner = null;
                b.Partner = null;
            }

            relation.LTR.ForceChangeState(nextState);
            if (currentState == relation.LTR.CurrentLTR) return false;

            StyledNotification.Show(new StyledNotification.Format("Relationship between " + a.FirstName + " " + a.LastName + " and " + b.FirstName + " " + b.LastName + " changed to " + nextState, StyledNotification.NotificationStyle.kAlert));
            return true;
        }

        public class StatePartner : RelationshpBase
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            public StatePartner()
            { }

            // Nested Types
            [DoesntRequireTuning]
            private sealed class Definition : ImmediateInteractionDefinition<Sim, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas, StatePartner>
            {
                public override string[] GetPath()
                {
                    return new string[] { "Romancer..." };
                }

                protected override string GetInteractionName(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, InteractionObjectPair interaction)
                {
                    return "Query Partner...";
                }

                protected override bool Test(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    return true;
                }
            }

            protected override bool AllowChildren()
            {
                return false;
            }

            protected override bool AllowMe()
            {
                return true;
            }

            protected override bool Run(SimDescription a, SimDescription b)
            {
                if (b.Partner == null)
                {
                    SimpleMessageDialog.Show("Query Partner", b.FirstName + " is currently unpartnered");
                }
                else
                {
                    SimpleMessageDialog.Show("Query Partner", b.FirstName + "'s partner is " + b.Partner.FirstName + " " + b.Partner.LastName);
                }

                return true;
            }
        }

        public static List<PhoneSimPicker.SimPickerInfo> ListOfAllSims(Sim me, bool allowChildren, bool allowMe)
        {
            List<PhoneSimPicker.SimPickerInfo> sims = new List<PhoneSimPicker.SimPickerInfo>();

            List<Sim> list = new List<Sim>(Sims3.Gameplay.Queries.GetObjects<Sim>());
            foreach (Sim sim in list)
            {
                if (sim.LotCurrent != me.LotCurrent) continue;

                if (!allowMe)
                {
                    if (sim == me) continue;
                }

                if (!allowChildren)
                {
                    if (sim.SimDescription.ChildOrBelow) continue;
                }

                PhoneSimPicker.SimPickerInfo item = new PhoneSimPicker.SimPickerInfo();
                item.FirstName = sim.SimDescription.LastName + ",";
                item.LastName = sim.SimDescription.FirstName;
                item.Thumbnail = sim.SimDescription.GetThumbnailKey(ThumbnailSize.Large, 0);
                item.SimDescription = sim.SimDescription;

                Relationship relation = Relationship.Get(sim.SimDescription, me.SimDescription, false);
                if (relation != null)
                {
                    item.RelationShip = relation.LTR.Liking;
                    item.RelationshipImage = relation.LTR.CurrentLTR;
                    item.Friend = relation.AreFriends();
                }
                else if (sim == me)
                {
                    item.RelationShip = 100f;
                    item.RelationshipImage = "Best Friend";
                    item.Friend = true;
                }
                else
                {
                    item.RelationShip = 0f;
                    item.RelationshipImage = "Stranger";
                    item.Friend = false;
                }

                item.RelationshipText = LTRData.Get(item.RelationshipImage).GetName(me.SimDescription, sim.SimDescription);
                item.RelationshipWithFirstName = me.SimDescription.FirstName;

                item.CoWorker = false;
                if ((me.CareerManager != null) && (me.CareerManager.Job != null))
                {
                    if (sim.SimDescription == me.CareerManager.Job.Boss)
                    {
                        item.CoWorker = true;
                    }
                    else
                    {
                        foreach (SimDescription description2 in me.CareerManager.Job.Coworkers)
                        {
                            if (description2 == sim.SimDescription)
                            {
                                item.CoWorker = true;
                                break;
                            }
                        }
                    }
                }

                sims.Add(item);
            }

            return sims;
        }
    }
}
