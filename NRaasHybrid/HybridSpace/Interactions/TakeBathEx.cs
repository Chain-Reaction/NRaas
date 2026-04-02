using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.SimIFace;

namespace NRaas.HybridSpace.Interactions
{
    public class TakeBathEx : Bathtub.TakeBath, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.CornerBathtub, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubClawfoot, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubModern, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubRectangle, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubShowerModern, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubValue, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubFuture, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubRomantic, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubRanch, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubDarkLux, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubShowerSeasonChic, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubShowerTILocal, Bathtub.TakeBath.Definition, Definition>(false);

            if (!Common.AssemblyCheck.IsInstalled("NRaasShooless"))
            {
                sOldSingleton = Singleton;
                Singleton = new Definition();
            }
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            if (!Common.AssemblyCheck.IsInstalled("NRaasShooless"))
            {
                interactions.Replace<Bathtub, Bathtub.TakeBath.Definition>(Singleton);
            }
        }

        public new class Definition : Bathtub.TakeBath.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TakeBathEx();
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

                if (target.Repairable != null && target.Repairable.Broken)
                {
                    greyedOutTooltipCallback = delegate
                    {
                        return Bathtub.LocalizeString(a.IsFemale, "CannotBathe", (object)a);
                    };
                    return false;
                }

                if (target.IsCatPlaying())
                {
                    greyedOutTooltipCallback = delegate
                    {
                        return Bathtub.LocalizeString(a.IsFemale, "CatNeedShoo", (object)a);
                    };
                    return false;
                }

                return true;
            }
        }
    }
}
