using NRaas.CareerSpace.Situations;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Services;
using Sims3.SimIFace;
using System;

namespace NRaas.CareerSpace.Interactions
{
    public class WitnessedMurderCall : Phone.Call
    {
        public override void Init(ref InteractionInstanceParameters parameters)
        {
            if (parameters.Actor is Sim)
            {
                parameters.Priority = new InteractionPriority(InteractionPriorityLevel.High, 2f);
                base.Init(ref parameters);
            }
        }

        public override Phone.Call.DialBehavior GetDialBehavior()
        {
            if (base.GetDialBehavior() != Phone.Call.DialBehavior.DoNotPick)
            {
                return Phone.Call.DialBehavior.CallPolice;
            }
            return Phone.Call.DialBehavior.DoNotPick;
        }

        public override Phone.Call.ConversationBehavior OnCallConnected()
        {
            try
            {
                Police instance = Police.Instance;
                if (instance != null)
                {
                    Definition definition = InteractionDefinition as Definition;
                    if (definition != null)
                    {
                        SimArrestSituationEx.Create(definition.mMurderer);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }

            return Phone.Call.ConversationBehavior.NoBehavior;
        }

        public class Definition : Phone.Call.CallDefinition<WitnessedMurderCall>
        {
            public readonly Sim mMurderer;

            public Definition(Sim murderer)
            {
                mMurderer = murderer;
            }

            public override string GetInteractionName(Sim actor, Phone target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(Phone.CallPolice.Singleton, target));
            }

            public override bool Test(Sim a, Phone target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }
    }
}
