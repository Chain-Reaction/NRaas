using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class GoToJail : PoliceStation.GoToJail, Common.IPreLoad
    {
        // Fields
        public static readonly new InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<PoliceStation, PoliceStation.GoToJail.Definition, Definition>(false);
        }

        public override bool InRabbitHole()
        {
            try
            {
                LotManager.SetAutoGameSpeed();
                StartStages();

                EventTracker.SendEvent(new DisgracefulActionEvent(EventTypeId.kSimCommittedDisgracefulAction, Actor, DisgracefulActionType.Arrested));

                DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new Interaction<Sim, PoliceStation>.InsideLoopFunction(LoopDel), null);
                return true;
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

        // Nested Types
        public new class Definition : InteractionDefinition<Sim, PoliceStation, GoToJail>
        {
            // Methods
            public Definition()
            { }

            public override bool Test(Sim a, PoliceStation target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }

        public class Installer : ExpansionInstaller<Main>
        {
            protected override bool PrivateInstall(Main main, bool initial)
            {
                if (initial)
                {
                    GoToJailScenario.OnPushInteraction += OnInteraction;
                }

                return true;
            }

            public static InteractionDefinition OnInteraction()
            {
                return Singleton;
            }
        }
    }
}

