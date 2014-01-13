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
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class Orienter
    {
        [Tunable]
        protected static bool kInstantiator = false;

        [Persistable(false)]
        private static EventListener sBoughtObjectLister = null;

        static Orienter()
        {
            World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinishedHandler);
        }
        public Orienter()
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

            obj.AddInteraction(OrientMe.Singleton);
            obj.AddInteraction(OrientOther.Singleton);
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
                    return new string[] { "Gender Preference..." };
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
                SimpleMessageDialog.Show("Orienter Version", "Version 1");
                return true;
            }
        }

        public class OrientMe : ImmediateInteraction<Sim, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            public OrientMe()
            {}

            // Nested Types
            [DoesntRequireTuning]
            private sealed class Definition : ImmediateInteractionDefinition<Sim, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas, OrientMe>
            {
                public override string[] GetPath()
                {
                    return new string[] { "Gender Preference..." };
                }

                protected override string GetInteractionName(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, InteractionObjectPair interaction)
                {
                    string msg = "Orient Me";

                    msg += ": M=" + a.SimDescription.mGenderPreferenceMale.ToString();
                    msg += " F=" + a.SimDescription.mGenderPreferenceFemale.ToString();

                    return msg;
                }

                protected override bool Test(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    if (!a.SimDescription.TeenOrAbove) return false;

                    return true;
                }
            }

            protected override bool Run()
            {
                return Perform(base.Actor.SimDescription);
            }

            public static bool Perform (SimDescription me)
            {
                string msg = "\nNumbers above zero signify an attraction, while numbers below zero signify avoidance";

                string text = StringInputDialog.Show("Orienter", "Enter Male Gender Preference for " + me.FirstName + " " + me.LastName + msg, me.mGenderPreferenceMale.ToString(), 256, StringInputDialog.Validation.None);
                if ((text == null) || (text == "")) return false;

                int iValue = 0;
                if (!int.TryParse(text, out iValue))
                {
                    SimpleMessageDialog.Show("Orienter", "The value provided could not be converted into a number.");
                    return false;
                }

                me.mGenderPreferenceMale = iValue;

                text = StringInputDialog.Show("Orienter", "Enter Female Gender Preference for " + me.FirstName + " " + me.LastName + msg, me.mGenderPreferenceFemale.ToString(), 256, StringInputDialog.Validation.None);
                if ((text == null) || (text == "")) return false;

                iValue = 0;
                if (!int.TryParse(text, out iValue))
                {
                    SimpleMessageDialog.Show("Orienter", "The value provided could not be converted into a number.");
                    return false;
                }

                me.mGenderPreferenceFemale = iValue;

                return true;
            }
        }

        public class OrientOther : ImmediateInteraction<Sim, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            public OrientOther()
            { }

            // Nested Types
            [DoesntRequireTuning]
            private sealed class Definition : ImmediateInteractionDefinition<Sim, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas, OrientOther>
            {
                public override string[] GetPath()
                {
                    return new string[] { "Gender Preference..." };
                }

                protected override string GetInteractionName(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, InteractionObjectPair interaction)
                {
                    return "Orient Other...";
                }

                protected override bool Test(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    return true;
                }
            }

            protected override bool Run()
            {
                List<PhoneSimPicker.SimPickerInfo> sims = ListOfAllSims(base.Actor);
                if ((sims == null) || (sims.Count == 0))
                {
                    SimpleMessageDialog.Show("Orienter", "There is no one available on the lot to orient.");
                    return false;
                }

                List<object> list = PhoneSimPicker.Show(true, ModalDialog.PauseMode.PauseSimulator, sims, "Select Sim", "Select", "Cancel");
                if ((list == null) || (list.Count == 0))
                {
                    return false;
                }

                SimDescription other = list[0] as SimDescription;

                return OrientMe.Perform(other);
            }
        }

        public static List<PhoneSimPicker.SimPickerInfo> ListOfAllSims(Sim me)
        {
            List<PhoneSimPicker.SimPickerInfo> sims = new List<PhoneSimPicker.SimPickerInfo>();

            List<Sim> list = new List<Sim>(Sims3.Gameplay.Queries.GetObjects<Sim>());
            foreach (Sim sim in list)
            {
                if (sim.LotCurrent != me.LotCurrent) continue;

                if (!sim.SimDescription.TeenOrAbove) continue;

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
