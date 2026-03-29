using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.SimIFace;

namespace NRaas.HybridSpace.Interactions
{
    public class TakeShowerEx : Shower.TakeShower, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerBasic, Shower.TakeShower.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerGen, Shower.TakeShower.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerHETech, Shower.TakeShower.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerLoft, Shower.TakeShower.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerModern, Shower.TakeShower.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubShowerModern, Shower.TakeShower.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerFuture, Shower.TakeShower.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerRanch, Shower.TakeShower.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerRomantic, Shower.TakeShower.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubShowerSeasonChic, Shower.TakeShower.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubShowerTILocal, Shower.TakeShower.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.ShowerCheap, Shower.TakeShower.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.ShowerExpensive, Shower.TakeShower.Definition, Definition>(false);
            Tunings.Inject<IShowerable, Shower.TakeShower.Definition, Definition>(false);

            if (!Common.AssemblyCheck.IsInstalled("NRaasShooless") && !Common.AssemblyCheck.IsInstalled("NRaasWoohooer"))
            {
                sOldSingleton = Singleton;
                Singleton = new Definition();
            }
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            if (!Common.AssemblyCheck.IsInstalled("NRaasShooless") && !Common.AssemblyCheck.IsInstalled("NRaasWoohooer"))
            {
                interactions.Replace<Bathtub, Bathtub.TakeBubbleBath.Definition>(Singleton);
            }
        }

        public new class Definition : Shower.TakeShower.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TakeShowerEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, IShowerable target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, IShowerable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous && ((a.Autonomy.Motives.GetValue(CommodityKind.Hygiene) >= 100f && a.SimDescription != null && !a.SimDescription.IsMermaid) || (a.SimDescription != null && a.SimDescription.IsMermaid && a.Autonomy.Motives.GetValue(CommodityKind.MermaidDermalHydration) >= 100f) || a.SimDescription.IsRobot))
                {
                    return false;
                }

                ShowerTub showerTub = target as ShowerTub;
                if (showerTub != null && showerTub.IsCatPlaying())
                {
                    greyedOutTooltipCallback = delegate
                    {
                        return LocalizeString(a.IsFemale, "CatNeedShoo", a);
                    };
                    return false;
                }

                return !target.Repairable.Broken;
            }
        }
    }
}
