using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Seating;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class LoungeRead
    {
        [Tunable]
        protected static bool kInstantiator = false;

        [Persistable(false)]
        private static EventListener sBoughtObjectLister = null;

        static LoungeRead()
        {
            World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinishedHandler);
        }
        public LoungeRead()
        { }

        public static void AddInteractions(ChairLounge obj)
        {
            foreach (InteractionObjectPair pair in obj.Interactions)
            {
                if (pair.InteractionDefinition.GetType() == Lounge.Singleton.GetType())
                {
                    return;
                }
            }

            obj.AddInteraction(Lounge.Singleton);
        }

        public static void OnWorldLoadFinishedHandler(object sender, EventArgs e)
        {
            List<ChairLounge> others = new List<ChairLounge>(Sims3.Gameplay.Queries.GetObjects<ChairLounge>());
            foreach (ChairLounge obj in others)
            {
                AddInteractions(obj);
            }

            sBoughtObjectLister = EventTracker.AddListener(EventTypeId.kBoughtObject, new ProcessEventDelegate(OnObjectBought));
        }

        protected static ListenerAction OnObjectBought(Sims3.Gameplay.EventSystem.Event e)
        {
            if (e.Id == EventTypeId.kBoughtObject)
            {
                ChairLounge chair = e.TargetObject as ChairLounge;
                if (chair != null)
                {
                    AddInteractions(chair);
                }
            }

            return ListenerAction.Keep;
        }

        public class LoungingPostureEx : LoungingPosture
        {
            // Methods
            public LoungingPostureEx()
            { }
            public LoungingPostureEx(ChairLounge chair, Sim sim, StateMachineClient smc)
                : base(chair, sim, smc)
            { }

            public override float Satisfaction(CommodityKind condition, IGameObject target)
            {
                if (condition == CommodityKind.Relaxing)
                {
                    return 1f;
                }
                return base.Satisfaction(condition, target);
            }

            public override StateMachineClient GetPostureStateMachine(PostureInteractions interaction, Posture.GetStateMachineDelegate GetDefaultStateMachine)
            {
                switch (interaction)
                {
                    case PostureInteractions.ReadBook:
                        return base.CurrentStateMachine;
                }
                return base.GetPostureStateMachine(interaction, GetDefaultStateMachine);
            }
        }

        public class Lounge : ChairLounge.EnterRelaxing
        {
            // Fields
            public new static readonly InteractionDefinition Singleton = new Definition();

            public Lounge()
            { }

            // Methods
            public override bool Run()
            {
                if ((base.Actor.Posture != null) && (base.Actor.Posture.Container == base.Target))
                {
                    return true;
                }
                if (this.StartRelaxing())
                {
                    base.EndCommodityUpdates(true);
                    base.StandardExit(false, false);
                    base.Actor.BuffManager.AddElement(BuffNames.Comfy, base.Target.Tuning.ComfyBuffStrength, unchecked ((Origin) (-8359806666160896334L)));
                    return true;
                }
                return false;
            }

            protected new bool StartRelaxing()
            {
                if (!base.StartRelaxing()) return false;

                base.Actor.Posture = new LoungingPostureEx(base.Target, base.Actor, base.mCurrentStateMachine);
                return true;
            }

            // Nested Types
            private new class Definition : InteractionDefinition<Sim, ChairLounge, Lounge>
            {
                public override string GetInteractionName(Sim a, ChairLounge target, InteractionObjectPair interaction)
                {
                    return "Lounge";
                }

                // Methods
                public override bool Test(Sim a, ChairLounge target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return a.SimDescription.TeenOrAbove;
                }
            }
        }
    }
}
