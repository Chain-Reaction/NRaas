using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Reflection;

namespace NRaas.CareerSpace.Helpers
{
    public class AfterschoolActivityData
    {
        public delegate InteractionInstance.InsideLoopFunction LoopFunc(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity);

        public AfterschoolActivity mActivity;

        public MethodInfo mPreLoop;

        public MethodInfo mLoop;

        public MethodInfo mPostLoop;

        public CASAgeGenderFlags mAges;

        public CASAgeGenderFlags mGenders;

        public AfterschoolActivityData()
        { }

        public bool IsChild
        {
            get
            {
                return ((mAges & CASAgeGenderFlags.Child) == CASAgeGenderFlags.Child);
            }
        }

        public bool IsTeen
        {
            get
            {
                return ((mAges & CASAgeGenderFlags.Teen) == CASAgeGenderFlags.Teen);
            }
        }

        public bool IsValidFor(SimDescription sim)
        {
            if ((mGenders & sim.Gender) != sim.Gender) return false;

            if ((mAges & sim.Age) != sim.Age) return false;

            GreyedOutTooltipCallback greyedOutTooltipCallback = null;
            if (!AfterschoolActivityEx.MeetsCommonAfterschoolActivityRequirements(sim, mActivity.CurrentActivityType, ref greyedOutTooltipCallback)) return false;

            return true;
        }

        public void PerformPreLoop(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
        {
            try
            {
                if (mPreLoop != null)
                {
                    mPreLoop.Invoke(null, new object[] { interaction, activity });
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(interaction.Actor, interaction.Target, e);
            }
        }

        public InteractionInstance.InsideLoopFunction LoopDelegate(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
        {
            try
            {
                if (mLoop != null)
                {
                    InteractionInstance.InsideLoopFunction del = (InteractionInstance.InsideLoopFunction)mLoop.Invoke(null, new object[] { interaction, activity });
                    if (del != null)
                    {
                        return del;
                    }
                }

                return interaction.AfterschoolActivityLoopDelegate;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(interaction.Actor, interaction.Target, e);
                return null;
            }
        }

        public void PerformPostLoop(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
        {
            try
            {
                if (mPostLoop != null)
                {
                    mPostLoop.Invoke(null, new object[] { interaction, activity });
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(interaction.Actor, interaction.Target, e);
            }
        }

        [Persistable]
        public abstract class LoopProxy
        {
            protected GoToSchoolInRabbitHole mInteraction;

            protected AfterschoolActivity mActivity;

            public LoopProxy()
            {}
            public LoopProxy(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
            {
                mInteraction = interaction;
                mActivity = activity;
            }

            public void Perform(StateMachineClient smc, InteractionInstance.LoopData loopData)
            {
                try
                {
                    PrivatePerform(smc, loopData);
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Common.Exception(mInteraction.Actor, mInteraction.Target, e);
                }
            }

            protected abstract void PrivatePerform(StateMachineClient smc, InteractionInstance.LoopData loopData);
        }
    }
}
