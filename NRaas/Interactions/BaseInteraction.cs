using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;

namespace NRaas.CommonSpace.Interactions
{
    public class BaseInteractionTuning
    {
        [Tunable, TunableComment("Whether to always have the menu active in-game")]
        public static bool kDisplayAlways = true;
    }

    public abstract class BaseInteraction<OBJ> : ImmediateInteractionGameObjectHit<IActor, OBJ>, Common.IAddInteraction
        where OBJ : class, IGameObject
    {
        public abstract void AddInteraction(Common.InteractionInjectorList interactions);

        public override string GetInteractionName()
        {
            return Common.Localize("Root:MenuName");
        }
        protected virtual string GetInteractionName(IActor actor, OBJ target, GameObjectHit hit)
        {
            return GetInteractionName();
        }

        protected virtual bool Test(IActor actor, OBJ target, GameObjectHit hit, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return true;
        }

        public override Lot GetTargetLot()
        {
            Lot lot = Target as Lot;
            if (lot != null) return lot;

            return base.GetTargetLot();
        }

        [DoesntRequireTuning]
        public abstract class BaseDefinition<INTERACTION> : ImmediateInteractionDefinition<IActor, OBJ, INTERACTION>
            where INTERACTION : BaseInteraction<OBJ>, IImmediateInteraction, new()
        {
            protected string[] mPath = null;

            protected GameObjectHit mHit;

            public BaseDefinition()
                : this(new string[0])
            { }
            protected BaseDefinition(string[] path)
            {
                mPath = path;
            }

            public override string[] GetPath(bool isFemale)
            {
                return mPath;
            }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    mHit = parameters.Hit;

                    if (!BaseInteractionTuning.kDisplayAlways)
                    {
                        if (!Cheats.sTestingCheatsEnabled) return InteractionTestResult.Def_TestFailed;
                    }

                    if (Test(parameters.Actor, parameters.Target as OBJ, parameters.Autonomous, ref greyedOutTooltipCallback))
                    {
                        return InteractionTestResult.Pass;
                    }
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Common.Exception(parameters.Actor, parameters.Target, e);
                }

                return InteractionTestResult.Def_TestFailed;
            }
        }
    }
}
