using NRaas.CommonSpace.Interactions;
using NRaas.PortraitPanelSpace.Dialogs;
using NRaas.PortraitPanelSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.PortraitPanelSpace.Interactions
{
    public class SimInteraction : ListedInteraction<ISimOption, Sim>
    {
        public static InteractionDefinition Singleton = new CommonDefinition<SimInteraction>();

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddCustom(new CustomInjector());
        }

        protected override bool Test(IActor actor, Sim target, GameObjectHit hit, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (PortraitPanel.Settings.mShowCycleButton)
            {
                if (!PortraitPanel.Settings.mShowSimMenu) return false;
            }

            return base.Test(actor, target, hit, ref greyedOutTooltipCallback);
        }

        public class CustomInjector : Common.InteractionInjector<Sim>
        {
            public CustomInjector()
                : base(Singleton)
            { }

            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
                if (!base.Perform(obj, definition, existing)) return false;

                Sim sim = obj as Sim;
                if ((sim != null) && (sim.InteractionQueue != null))
                {
                    sim.InteractionQueue.QueueChanged += new QueueWatch(sim).OnQueueChanged;
                }

                return true;
            }
        }

        public class QueueWatch
        {
            Sim mSim;

            InteractionInstance mCurrentInteraction;

            public QueueWatch(Sim sim)
            {
                mSim = sim;
            }

            public void OnQueueChanged()
            {
                InteractionInstance current = mSim.CurrentInteraction;

                if (!object.ReferenceEquals(mCurrentInteraction, current))
                {
                    mCurrentInteraction = current;
                    SkewerEx.QueueChanged();
                }
            }
        }
    }
}
