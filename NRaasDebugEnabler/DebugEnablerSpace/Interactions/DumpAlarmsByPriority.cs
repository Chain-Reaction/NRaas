using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.DebugEnablerSpace.Interfaces;
using Sims3.Collections;
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
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Interactions
{
    public class DumpAlarmsByPriority : DebugEnablerInteraction<GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if ((obj is CityHall) || (obj is Sim))
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        public override bool Run()
        {
            try
            {
                List<AlarmManager> managers = new List<AlarmManager>();

                managers.Add (AlarmManager.Global);

                foreach (Lot lot in LotManager.AllLots)
                {
                    if (lot.mSavedData.mAlarmManager == null) continue;

                    managers.Add(lot.AlarmManager);
                }

                Dictionary<ReferenceWrapper, bool> lookup = new Dictionary<ReferenceWrapper, bool>();

                List<Pair<AlarmManager.Timer,int>> timers = new List<Pair<AlarmManager.Timer,int>>();

                foreach (AlarmManager manager in managers)
                {
                    int count = 0;
                    foreach (object item in manager.mTimerQueue)
                    {
                        AlarmManager.Timer timer = item as AlarmManager.Timer;
                        if (timer == null) continue;

                        if (lookup.ContainsKey(new ReferenceWrapper(timer))) continue;
                        lookup.Add(new ReferenceWrapper(timer), true);

                        timers.Add(new Pair<AlarmManager.Timer,int> (timer, count));

                        count++;
                    }
                }

                DumpAlarms.Dump(timers);
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<DumpAlarmsByPriority>
        {
            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("DumpAlarmsByPriority:MenuName");
            }
        }
    }
}
