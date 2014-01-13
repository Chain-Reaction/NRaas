using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.ShoolessSpace.Interactions
{
    public class TeleportToWorkOrSchoolEx : RoutineMachine.TeleportToWorkOrSchool, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<RoutineMachine, RoutineMachine.TeleportToWorkOrSchool.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<RoutineMachine, RoutineMachine.TeleportToWorkOrSchool.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool DoTeleport(IGameObject gameObject)
        {
            if ((!(InteractionDefinition is Definition) || (Target == null)) || (Actor == null))
            {
                return false;
            }

            GameObject destinationObject = null;
            bool flag = IsForOccupation(Actor);
            if ((flag && (Actor != null)) && (Actor.Occupation != null))
            {
                destinationObject = Actor.Occupation.OfficeLocation;
            }
            else if ((IsForSchool(Actor) && (Actor.School != null)) && (Actor.School.CareerLoc != null))
            {
                // Custom
                destinationObject = Actor.School.CareerLoc.Owner.RabbitHoleProxy;
            }

            if (destinationObject == null)
            {
                return false;
            }

            bool flag2 = Target.Teleport(Actor, destinationObject);
            if ((flag2 && flag) && ((Actor.Occupation.HoursUntilWork > kEarlyBonusHoursEarlyToGetBonus) && (Actor.Occupation.HoursUntilWork < 12f)))
            {
                Target.AddAlarm(kEarlyNoticeDelayInMinutes, TimeUnit.Minutes, WorkEarlyBonus, "Get to work early bonus", AlarmType.DeleteOnReset);
            }
            if (flag2 && (destinationObject is RabbitHole))
            {
                InteractionInstance entry = (flag ? WorkInRabbitHole.Singleton : GoToSchoolInRabbitHole.Singleton).CreateInstance(destinationObject, Actor, Actor.InheritedPriority(), false, true);
                return Actor.InteractionQueue.AddNext(entry);
            }

            return flag2;
        }

        public override bool Run()
        {
            try
            {
                return TeleportBaseEx.Run(this);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : RoutineMachine.TeleportToWorkOrSchool.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TeleportToWorkOrSchoolEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, RoutineMachine target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}


