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
    public class Pollinator
    {
        [Tunable]
        protected static bool kInstantiator = false;

        [Persistable(false)]
        private static EventListener sBoughtObjectLister = null;

        static Pollinator()
        {
            World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinishedHandler);
        }
        public Pollinator()
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

            obj.AddInteraction(TryForBaby.Singleton);
            obj.AddInteraction(TryForBabyMe.Singleton);
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

        public static bool AllowOverstuffed
        {
            get
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().ToString().ToLower().Contains("awesome,"))
                    {
                        foreach (Type type in assembly.GetTypes())
                        {
                            if (type.FullName.ToLower () == "awesome.config")
                            {
                                List<FieldInfo> fields = new List<FieldInfo>(type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static));

                                foreach (FieldInfo info in fields)
                                {
                                    if (info.Name.ToLower() == "allowoverstuffedhouses")
                                    {
                                        object value = info.GetValue(null);

                                        if (value is bool)
                                        {
                                            if (value.ToString() == "True")
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
                return false;
            }
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
                    return new string[] { "Pollinator..." };
                }

                protected override string GetInteractionName(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, InteractionObjectPair interaction)
                {
                    return "Version";
                }

                protected override bool Test(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    if (a.Household == null) return false;

                    if (a.Household != Household.ActiveHousehold) return false;

                    if (a.Household.IsServiceNpcHousehold) return false;

                    if (a.LotHome == null) return false;

                    if (!a.SimDescription.TeenOrAbove) return false;

                    return true;
                }
            }

            protected override bool Run()
            {
                SimpleMessageDialog.Show("Pollinator Version", "Version 8");
                return true;
            }
        }

        public class TryForBaby : ImmediateInteraction<Sim, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new TryForBabyDefinition();

            public TryForBaby()
            {}

            // Nested Types
            [DoesntRequireTuning]
            private sealed class TryForBabyDefinition : ImmediateInteractionDefinition<Sim, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas, TryForBaby>
            {
                public override string[] GetPath()
                {
                    return new string[] { "Pollinator..." };
                }

                protected override string GetInteractionName(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, InteractionObjectPair interaction)
                {
                    return "Pollinate Other...";
                }

                protected override bool Test(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    if (a.Household == null) return false;

                    if (a.Household != Household.ActiveHousehold) return false;

                    if (a.Household.IsServiceNpcHousehold) return false;

                    if (a.LotHome == null) return false;

                    if (!a.SimDescription.TeenOrAbove) return false;

                    return true;
                }
            }

            protected override bool Run()
            {
                List<PhoneSimPicker.SimPickerInfo> sims = ListOfAllSims(base.Actor);
                if ((sims == null) || (sims.Count == 0))
                {
                    SimpleMessageDialog.Show("Pollination", "There is no one available on the lot to pollinate.");
                    return false;
                }

                List<object> list = PhoneSimPicker.Show(true, ModalDialog.PauseMode.PauseSimulator, sims, "Select Partner", "Select", "Cancel");
                if ((list == null) || (list.Count == 0))
                {
                    return false;
                }

                SimDescription other = list[0] as SimDescription;

                Sim otherSim = other.CreatedSim;
                if ((otherSim == null) || (otherSim == base.Actor))
                {
                    return Pollinator.Pollinate(null, base.Actor);
                }
                else
                {
                    return Pollinator.Pollinate(base.Actor, otherSim);
                }
            }
        }

        public class TryForBabyMe : ImmediateInteraction<Sim, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new TryForBabyMeDefinition();

            public TryForBabyMe()
            { }

            // Nested Types
            [DoesntRequireTuning]
            private sealed class TryForBabyMeDefinition : ImmediateInteractionDefinition<Sim, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas, TryForBabyMe>
            {
                public override string[] GetPath()
                {
                    return new string[] { "Pollinator..." };
                }

                protected override string GetInteractionName(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, InteractionObjectPair interaction)
                {
                    return "Pollinate Me...";
                }

                protected override bool Test(Sim a, Sims3.Gameplay.Objects.Decorations.Mimics.SculpturePlantFiccas target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    if (a.Household == null) return false;

                    if (a.Household != Household.ActiveHousehold) return false;

                    if (a.Household.IsServiceNpcHousehold) return false;

                    if (a.LotHome == null) return false;

                    if (!a.SimDescription.TeenOrAbove) return false;

                    return true;
                }
            }

            protected override bool Run()
            {
                List<PhoneSimPicker.SimPickerInfo> sims = ListOfAllSims(base.Actor);
                if ((sims == null) || (sims.Count == 0))
                {
                    SimpleMessageDialog.Show("Pollination", "There is no one available on the lot to pollinate.");
                    return false;
                }

                List<object> list = PhoneSimPicker.Show(true, ModalDialog.PauseMode.PauseSimulator, sims, "Select Partner", "Select", "Cancel");
                if ((list == null) || (list.Count == 0))
                {
                    return false;
                }

                SimDescription other = list[0] as SimDescription;

                Sim otherSim = other.CreatedSim;
                if (otherSim == null)
                {
                    SimpleMessageDialog.Show("Pollination", other.FirstName + " " + other.LastName + " was unavailable for pollination.");
                    return false;
                }

                if (otherSim == base.Actor)
                {
                    otherSim = null;
                }

                return Pollinator.Pollinate(otherSim, base.Actor);
            }
        }

        public static bool Pollinate (Sim man, Sim woman)
        {
            if (woman.LotHome == null)
            {
                SimpleMessageDialog.Show("Pollination", woman.Name + " is homeless.");
                return false;
            }
            else if (woman.Household.IsServiceNpcHousehold)
            {
                SimpleMessageDialog.Show("Pollination", woman.Name + " is an NPC and can not be pollinated.");
                return false;
            }
            else if ((!AllowOverstuffed) && (woman.Household.NumMembersCountingPregnancy >= 8))
            {
                SimpleMessageDialog.Show("Pollination", woman.Name + " house has too many people.");
                return false;
            }
            else if (woman.SimDescription.Pregnancy != null)
            {
                SimpleMessageDialog.Show("Pollination", woman.Name + " has already been pollinated.");
                return false;
            }
            else if (woman.SimDescription.AgingState.IsAgingInProgress())
            {
                SimpleMessageDialog.Show("Pollination", woman.Name + " is too old to be pollinated.");
                return false;
            }
            else if (woman.SimDescription.Elder) 
            {
                SimpleMessageDialog.Show("Pollination", woman.Name + " is too old to be pollinated.");
                return false;
            }

            Sims3.Gameplay.ActorSystems.AgingManager.Singleton.CancelAgingAlarmsForSim(woman);

            Sims3.Gameplay.ActorSystems.Pregnancy p = new Sims3.Gameplay.ActorSystems.Pregnancy(woman, man);

            p.PreggersAlarm = woman.AddAlarmRepeating(1f, TimeUnit.Hours, new AlarmTimerCallback(p.HourlyCallback), 1f, TimeUnit.Hours, "Hourly Pregnancy Update Alarm", AlarmType.AlwaysPersisted);

            woman.SimDescription.Pregnancy = p;

            EventTracker.SendEvent(new Sims3.Gameplay.ActorSystems.PregnancyEvent(EventTypeId.kGotPregnant, woman, man, p, null));

            SimpleMessageDialog.Show("Pollination", woman.Name + " has been successfully pollinated.");
            return true;
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
