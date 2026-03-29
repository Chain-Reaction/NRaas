using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.ShoolessSpace.Interactions
{
    public class TakeBubbleBathEx : Bathtub.TakeBubbleBath, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Bathtub, Bathtub.TakeBubbleBath.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.CornerBathtub, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubClawfoot, Bathtub.TakeBubbleBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubModern, Bathtub.TakeBubbleBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubRectangle, Bathtub.TakeBubbleBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubShowerModern, Bathtub.TakeBubbleBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubValue, Bathtub.TakeBubbleBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubFuture, Bathtub.TakeBubbleBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubRomantic, Bathtub.TakeBubbleBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubRanch, Bathtub.TakeBubbleBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubDarkLux, Bathtub.TakeBubbleBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubShowerSeasonChic, Bathtub.TakeBubbleBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubShowerTILocal, Bathtub.TakeBubbleBath.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                return TakeBathEx.Perform(this);
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

        public new class Definition : Bathtub.TakeBubbleBath.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TakeBubbleBathEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Bathtub target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Bathtub target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous)
                {
                    if (a.SimDescription.IsRobot)
                    {
                        return false;
                    }

                    CommodityKind kind = CommodityKind.Hygiene;
                    if (a.SimDescription != null && a.SimDescription.IsMermaid)
                    {
                        kind = CommodityKind.MermaidDermalHydration;
                    }    

                    float value = a.Motives.GetValue(kind);
                    float desireYFromX = a.Autonomy.InteractionScorer.GetDesireYFromX(kind, value);
                    Desire desire = a.Autonomy.InteractionScorer.GetDesire(kind);
                    float num = ((desire != null) ? desire.MaxY : 0f);
                    if (num - desireYFromX <= 0f)
                    {
                        return false;
                    }
                }

                if (target.Repairable.Broken)
                {
                    return false;
                }

                return target.IsSlotted(typeof(BubbleBath));
            }
        }
    }
}


