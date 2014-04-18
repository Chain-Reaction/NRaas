using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TravelerSpace.Interactions
{
    public class ScheduleFireworkShowEx : CityHall.ScheduleFireworkShow, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<CityHall, CityHall.ScheduleFireworkShow.ScheduleFireworkDefinition, ScheduleFireworkDefinition>(false);            

            sOldSingleton = Singleton;
            Singleton = new ScheduleFireworkDefinition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<CityHall, CityHall.ScheduleFireworkShow.ScheduleFireworkDefinition>(Singleton);
        }

        public new class ScheduleFireworkDefinition : CityHall.ScheduleFireworkShow.ScheduleFireworkDefinition
        {
            public ScheduleFireworkDefinition()            
            {
                this.name = string.Empty;
                this.value = -1f;
            }

            public ScheduleFireworkDefinition(string parent, string child, float time)                
            {
                this.name = string.Empty;
                this.value = -1f;
                this.name = child;
                this.path = new string[] { parent };
                this.value = time;
            }

            public override string GetInteractionName(Sim actor, CityHall target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, CityHall target, List<InteractionObjectPair> results)
            {                
                string parent = Localization.LocalizeString(actor.IsFemale, "Gameplay/Objects/RabbitHoles/FutureCityHall/ScheduleFireworkShow:InteractionName", new object[] { CityHall.ScheduleFireworkShow.kCostOfFireworkShow }) + Localization.Ellipsis;
                float hour = SimClock.CurrentTime().Hour;
                for (int i = 0; i < CityHall.ScheduleFireworkShow.kTimeOptionsForFireworkShow.Length; i++)
                {
                    if ((CityHall.ScheduleFireworkShow.kTimeOptionsForFireworkShow[i] - 1f) > hour)
                    {
                        results.Add(new InteractionObjectPair(new ScheduleFireworkDefinition(parent, Localization.LocalizeString(actor.IsFemale, "Gameplay/Objects/RabbitHoles/FutureCityHall/ScheduleFireworkShow:Time", new object[] { SimClockUtils.GetText(CityHall.ScheduleFireworkShow.kTimeOptionsForFireworkShow[i]) }), CityHall.ScheduleFireworkShow.kTimeOptionsForFireworkShow[i]), iop.Target));
                    }
                }
            }

            public override string[] GetPath(bool isFemale)
            {
                if (this.path != null)
                {
                    return this.path;
                }
                return new string[0];
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new ScheduleFireworkShowEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim a, CityHall target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {                
                if (!GameUtils.IsInstalled(ProductVersion.EP11))
                {
                    return false;
                }
                if (a.FamilyFunds < CityHall.ScheduleFireworkShow.kCostOfFireworkShow)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(LocalizationHelper.InsufficientFunds);
                    return false;
                }
                if (target.mFireworkShowScheduled)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(a.IsFemale, "Gameplay/Objects/RabbitHoles/FutureCityHall/ScheduleFireworkShow:AlreadyScheduled", new object[] { target.mFireworkSchedulerName, SimClockUtils.GetText(target.mFireworkScheduledTime) }));
                    return false;
                }
                return true;
            }
        }
    }
}
