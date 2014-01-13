using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Opportunities;
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
    public class OpportunityControl
    {
        [Tunable]
        protected static bool kInstantiator = false;

        static OpportunityControl()
        {
            World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinishedHandler);
        }
        public OpportunityControl()
        { }

        public static void AddInteractions(GameObject obj)
        {
            foreach (InteractionObjectPair pair in obj.Interactions)
            {
                if (pair.InteractionDefinition.GetType() == Version.Singleton.GetType())
                {
                    return;
                }
            }

            List<InteractionDefinition> interactions = new List<InteractionDefinition>();

            interactions.Add(SpawnOpportunity.Singleton);
            interactions.Add(RandomOpportunity.Singleton);
            interactions.Add(ResetAllOpportunities.Singleton);
            interactions.Add(Version.Singleton);

            foreach (InteractionDefinition interaction in interactions)
            {
                obj.AddInteraction(interaction);
                obj.AddInventoryInteraction(interaction);
            }
        }

        public static void OnWorldLoadFinishedHandler(object sender, EventArgs e)
        {
            try
            {
                List<Computer> computers = new List<Computer>(Sims3.Gameplay.Queries.GetObjects<Computer>());
                foreach (Computer obj in computers)
                {
                    AddInteractions(obj);
                }

                List<PostBoxJobBoard> jobboards = new List<PostBoxJobBoard>(Sims3.Gameplay.Queries.GetObjects<PostBoxJobBoard>());
                foreach (PostBoxJobBoard obj in jobboards)
                {
                    AddInteractions(obj);
                }

                List<Sim> others = new List<Sim>(Sims3.Gameplay.Queries.GetObjects<Sim>());
                foreach (Sim obj in others)
                {
                    AddInteractions(obj);
                }

                EventTracker.AddListener(EventTypeId.kSimInstantiated, new ProcessEventDelegate(OnNewObject));

                EventTracker.AddListener(EventTypeId.kBoughtObject, new ProcessEventDelegate(OnObjectBought));

                EventTracker.AddListener(EventTypeId.kEventSimSelected, new ProcessEventDelegate(OnSelected));
                EventTracker.AddListener(EventTypeId.kHouseholdSelected, new ProcessEventDelegate(OnSelected));

                AlarmManager.Global.AddAlarm(1f, TimeUnit.Minutes, new AlarmTimerCallback(OnTimer), "NRaasOpportunityRemoveActive", AlarmType.NeverPersisted, null);
            }
            catch (Exception exception)
            {
                WriteLog(exception);
            }
        }

        public static bool WriteLog(Exception exception)
        {
            try
            {
                new ScriptError(null, exception, 0).WriteMiniScriptError();
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected static void OnTimer()
        {
            try
            {
                RemoveOpportunityManager();
            }
            catch (Exception e)
            {
                Exception(e);
            }
        }

        protected static void RemoveOpportunityManager()
        {
            if (Household.ActiveHousehold != null)
            {
                foreach (Sim sim in Household.ActiveHousehold.Sims)
                {
                    RemoveOpportunityManager(sim);
                }
            }
        }
        protected static void RemoveOpportunityManager(Sim sim)
        {
            if (sim == null) return;

            if (sim.mOpportunityManager != null)
            {
                if (sim.mOpportunitiesAlarmHandle != AlarmHandle.kInvalidHandle)
                {
                    AlarmManager.Global.RemoveAlarm(sim.mOpportunitiesAlarmHandle);
                    sim.mOpportunitiesAlarmHandle = AlarmHandle.kInvalidHandle;
                }
            }

            if (sim.CareerManager != null)
            {
                Career career = sim.CareerManager.Occupation as Career;
                if ((career != null) &&
                    (career.CareerEventManager != null))
                {
                    career.CareerEventManager.CancelAllActiveEvents();
                    career.CareerEventManager.Events.Clear();
                }

                if ((sim.CareerManager.School != null) &&
                    (sim.CareerManager.School.CareerEventManager != null))
                {
                    sim.CareerManager.School.CareerEventManager.CancelAllActiveEvents();
                    sim.CareerManager.School.CareerEventManager.Events.Clear();
                }
            }
        }

        protected static ListenerAction OnNewObject(Event e)
        {
            try
            {
                Sim obj = e.TargetObject as Sim;
                if (obj != null)
                {
                    AddInteractions(obj);
                }
            }
            catch (Exception exception)
            {
                Exception(exception);
            }

            return ListenerAction.Keep;
        }

        protected static ListenerAction OnSelected(Event e)
        {
            try
            {
                RemoveOpportunityManager();
            }
            catch (Exception exception)
            {
                Exception(exception);
            }

            return ListenerAction.Keep;
        }

        protected static ListenerAction OnObjectBought(Event e)
        {
            try
            {
                if (e.Id == EventTypeId.kBoughtObject)
                {
                    Computer obj = e.TargetObject as Computer;
                    if (obj != null)
                    {
                        AddInteractions(obj);
                    }
                }
            }
            catch (Exception exception)
            {
                Exception(exception);
            }

            return ListenerAction.Keep;
        }

        public static void Notify(string vsText)
        {
            StyledNotification.Format format = new StyledNotification.Format(vsText, ObjectGuid.InvalidObjectGuid, ObjectGuid.InvalidObjectGuid, StyledNotification.NotificationStyle.kSystemMessage);
            format.mTNSCategory = NotificationManager.TNSCategory.Lessons;
            StyledNotification.Show(format);
        }

        public static bool Exception(Exception exception)
        {
            return ((IScriptErrorWindow)AppDomain.CurrentDomain.GetData("ScriptErrorWindow")).DisplayScriptError(null, exception);
        }

        public static string Localize(string key)
        {
            return Localization.LocalizeString("NRaas.OpportunityControl." + key, new object[0]);
        }
        public static string Localize(string key, object[] parameters)
        {
            return Localization.LocalizeString("NRaas.OpportunityControl." + key, parameters);
        }

        protected static List<Opportunity> GetAllOpportunities(Sim sim)
        {
            List<Opportunity> allOpportunities = new List<Opportunity>();

            Dictionary<OpportunityNames, Opportunity> opportunityList = null;
            if (GameUtils.GetCurrentWorld() == WorldName.Egypt)
            {
                opportunityList = OpportunityManager.sAdventureEgyptOpportunityList;
            }
            else if (GameUtils.GetCurrentWorld() == WorldName.China)
            {
                opportunityList = OpportunityManager.sAdventureChinaOpportunityList;
            }
            else if (GameUtils.GetCurrentWorld() == WorldName.France)
            {
                opportunityList = OpportunityManager.sAdventureFranceOpportunityList;
            }
            else
            {
                opportunityList = OpportunityManager.sSkillOpportunityList;
            }

            if (opportunityList == null)
            {
                opportunityList = new Dictionary<OpportunityNames, Opportunity>();
            }

            foreach(Opportunity opp in OpportunityManager.sCareerPhoneCallOpportunityList.Values)
            {
                if (opportunityList.ContainsKey(opp.Guid)) continue;

                opportunityList.Add(opp.Guid, opp);
            }

            if (opportunityList != null)
            {
                foreach (Opportunity opportunity in opportunityList.Values)
                {
                    if (opportunity.IsAvailable(sim))
                    {
                        allOpportunities.Add(opportunity);
                    }
                }
            }

            CareerManager manager = sim.CareerManager;
            if (manager != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    Career career = manager.Occupation as Career;
                    if (i == 1)
                    {
                        career = manager.School;
                    }

                    if (career != null)
                    {
                        foreach (Career.EventDaily daily in career.CareerEventList)
                        {
                            Career.EventOpportunity oppEvent = daily as Career.EventOpportunity;
                            if (oppEvent == null) continue;

                            if (oppEvent.IsAvailable(career))
                            {
                                Opportunity opportunity = null;
                                GenericManager<OpportunityNames, Opportunity, Opportunity>.sDictionary.TryGetValue((ulong)oppEvent.mOpportunity, out opportunity);

                                if (opportunity != null)
                                {
                                    allOpportunities.Add(opportunity);
                                }
                            }
                        }
                    }
                }
            }

            List<Opportunity> allPotentials = new List<Opportunity>();
            foreach (Opportunity opportunity in allOpportunities)
            {
                try
                {
                    if (opportunity.IsCareer)
                    {
                        if (sim.OpportunityManager.HasOpportunity(OpportunityCategory.Career)) continue;
                    }
                    else if (opportunity.IsSkill)
                    {
                        if (sim.OpportunityManager.HasOpportunity(OpportunityCategory.Skill)) continue;
                    }

                    Opportunity toAdd = opportunity.Clone();
                    toAdd.Actor = sim;

                    if (!sim.OpportunityManager.SetupTargets(toAdd))
                    {
                        continue;
                    }
                    toAdd.SetLocalizationIndex();

                    string name = toAdd.Name;

                    allPotentials.Add(toAdd);
                }
                catch (Exception e)
                {
                    Exception(e);
                }
            }

            return allPotentials;
        }

        public class SpawnOpportunity : ImmediateInteraction<Sim, GameObject>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            public SpawnOpportunity()
            { }

            // Nested Types
            [DoesntRequireTuning]
            private sealed class Definition : ImmediateInteractionDefinition<Sim, GameObject, SpawnOpportunity>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[] { "NRaas", Localize("Root:MenuName") };
                }

                public override string GetInteractionName(Sim a, GameObject target, InteractionObjectPair interaction)
                {
                    return Localize ("Select:MenuName");
                }

                public override bool Test(Sim a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    Sim sim = target as Sim;
                    if (sim == null)
                    {
                        sim = a;
                    }

                    if (sim.OpportunityManager == null) return false;

                    return true;
                }
            }

            public override bool Run()
            {
                try
                {
                    Sim sim = Target as Sim;
                    if (sim == null)
                    {
                        sim = Actor;
                    }

                    List<Opportunity> allOpportunities = GetAllOpportunities(sim);
                    if (allOpportunities.Count == 0)
                    {
                        SimpleMessageDialog.Show(Localize ("Select:MenuName"), Localize ("Choice:None"));
                        return false;
                    }

                    List<ObjectPicker.HeaderInfo> headers = new List<ObjectPicker.HeaderInfo>();
                    headers.Add(new ObjectPicker.HeaderInfo("NRaas.OpportunityControl.Choice:Header", "NRaas.OpportunityControl.Choice:Tooltip", 230));

                    List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();
                    foreach (Opportunity opportunity in allOpportunities)
                    {
                        try
                        {
                            // Here to catch exceptions prior to RowInfo creation
                            string name = opportunity.Name;

                            if (opportunity.IsCareer)
                            {
                                name = Localization.LocalizeString("Ui/Caption/HUD/CareerPanel:Career",new object[0]) + ": " + name;
                            }

                            ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(opportunity, new List<ObjectPicker.ColumnInfo>());

                            item.ColumnInfo.Add(new ObjectPicker.TextColumn(name));

                            rowInfo.Add(item);
                        }
                        catch
                        {
                        }
                    }

                    List<ObjectPicker.TabInfo> tabInfo = new List<ObjectPicker.TabInfo>();
                    tabInfo.Add(new ObjectPicker.TabInfo("shop_all_r2", Localization.LocalizeString("Ui/Caption/ObjectPicker:All", new object[0]), rowInfo));

                    List<ObjectPicker.RowInfo> list = NRaas.OpportunityControlSpace.ObjectPickerDialog.Show(Localize ("Choice:Title"), tabInfo, headers, 1);

                    if ((list == null) || (list.Count == 0)) return false;

                    Opportunity sel = list[0].Item as Opportunity;
                    if (sel == null) return false; ;

                    sim.OpportunityManager.AddOpportunityNow(sel.Guid, true, false);
                    return true;
                }
                catch (Exception exception)
                {
                    Exception(exception);
                }

                return false;
            }
        }

        public class RandomOpportunity : ImmediateInteraction<Sim, GameObject>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            public RandomOpportunity()
            { }

            // Nested Types
            [DoesntRequireTuning]
            private sealed class Definition : ImmediateInteractionDefinition<Sim, GameObject, RandomOpportunity>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[] { "NRaas", Localize("Root:MenuName") };
                }

                public override string GetInteractionName(Sim a, GameObject target, InteractionObjectPair interaction)
                {
                    return Localize ("Random:MenuName");
                }

                public override bool Test(Sim a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    Sim sim = target as Sim;
                    if (sim == null)
                    {
                        sim = a;
                    }

                    if (sim.OpportunityManager == null) return false;

                    return true;
                }
            }

            public override bool Run()
            {
                try
                {
                    Sim sim = Target as Sim;
                    if (sim == null)
                    {
                        sim = Actor;
                    }

                    List<Opportunity> allOpportunities = GetAllOpportunities(sim);
                    if (allOpportunities.Count == 0)
                    {
                        SimpleMessageDialog.Show(Localize ("Random:MenuName"), Localize ("Choice:None"));
                        return false;
                    }

                    Opportunity sel = RandomUtil.GetRandomObjectFromList (allOpportunities);
                    if (sel == null) return false;

                    sim.OpportunityManager.AddOpportunityNow(sel.Guid, true, false);
                    return true;
                }
                catch (Exception exception)
                {
                    Exception(exception);
                }

                return false;
            }
        }

        public class ResetAllOpportunities : ImmediateInteraction<Sim, GameObject>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            public ResetAllOpportunities()
            { }

            // Nested Types
            [DoesntRequireTuning]
            private sealed class Definition : ImmediateInteractionDefinition<Sim, GameObject, ResetAllOpportunities>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[] { "NRaas", Localize("Root:MenuName") };
                }

                public override string GetInteractionName(Sim a, GameObject target, InteractionObjectPair interaction)
                {
                    return Localize ("ResetAll:MenuName");
                }

                public override bool Test(Sim a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    Sim sim = target as Sim;
                    if (sim == null)
                    {
                        sim = a;
                    }

                    if (sim.OpportunityManager == null) return false;

                    return true;
                }
            }

            public override bool Run()
            {
                try
                {
                    if (!AcceptCancelDialog.Show (Localize ("ResetAll:Prompt")))
                    {
                        return false;
                    }

                    List<Sim> sims = new List<Sim>(Sims3.Gameplay.Queries.GetObjects<Sim>());
                    foreach (Sim sim in sims)
                    {
                        try
                        {
                            if (sim.mOpportunityManager != null)
                            {
                                sim.mOpportunityManager.CancelAllOpportunities();
                                sim.mOpportunityManager.TearDownLocationBasedOpportunities();
                            }
                        }
                        catch
                        {
                        }

                        AlarmManager.Global.RemoveAlarm(sim.mOpportunitiesAlarmHandle);
                        sim.mOpportunitiesAlarmHandle = AlarmHandle.kInvalidHandle;

                        sim.mOpportunityManager = null;

                        if ((sim.IsSelectable) && (sim.mOpportunityManager == null) && (sim.SimDescription.ChildOrAbove))
                        {
                            sim.mOpportunityManager = new OpportunityManager(sim);
                            sim.mOpportunityManager.SetupLocationBasedOpportunities();
                        }
                    }
                }
                catch (Exception exception)
                {
                    Exception(exception);
                }

                return false;
            }
        }

        public class Version : ImmediateInteraction<Sim, GameObject>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new VersionDefinition();

            public Version()
            { }

            // Nested Types
            [DoesntRequireTuning]
            private sealed class VersionDefinition : ImmediateInteractionDefinition<Sim, GameObject, Version>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[] { "NRaas", Localize("Root:MenuName") };
                }

                public override string GetInteractionName(Sim a, GameObject target, InteractionObjectPair interaction)
                {
                    return Localize("Version:MenuName");
                }

                public override bool Test(Sim a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    Sim sim = target as Sim;
                    if (sim == null)
                    {
                        sim = a;
                    }

                    if (sim.OpportunityManager == null) return false;

                    return true;
                }
            }

            public override bool Run()
            {
                /* Changes
                 * 
                 * 
                 */
                SimpleMessageDialog.Show(Localize("Version:MenuName"), Localize("Version:Prompt", new object[] { 12 }));
                return true;
            }
        }
    }
}
