using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class CatchInsectEx : InsectJig.CatchInsect, Common.IPreLoad
    {
        public new static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<InsectJig, InsectJig.CatchInsect.Definition, Definition>(false);
        }

        public override bool Run()
        {
            try
            {
                InsectData data;
                Route r = Actor.RoutingComponent.CreateRoute();
                r.PlanToPointRadialRange(Target.Position, 0f, InsectJig.sMaxCatchRange, RouteDistancePreference.PreferNearestToRouteDestination, RouteOrientationPreference.TowardsObject, Target.LotCurrent.LotId, null);

                try
                {
                    if (!Actor.RoutingComponent.DoRoute(r) || Target.InUse)
                    {
                        return false;
                    }
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Common.DebugException(Actor, Target, e);
                    return false;
                }

                mDummyIkJig = GlobalFunctions.CreateObjectOutOfWorld("SocialJigOnePerson") as SocialJigOnePerson;
                mDummyIkJig.SetForward(Actor.ForwardVector);
                mDummyIkJig.SetPosition(Actor.Position);
                mDummyIkJig.SetHiddenFlags(~HiddenFlags.Nothing);
                mDummyIkJig.AddToWorld();

                StandardEntry();

                BeginCommodityUpdates();
                bool succeeded = false;

                try
                {
                    if (InsectData.sData.TryGetValue(Target.InsectType, out data))
                    {
                        mTempInsect = Sims3.Gameplay.Objects.Insect.Insect.Create(Target.InsectType);
                        string jazzInsectParameterName = data.GetJazzInsectParameterName();
                        string jazzEnterStateName = data.GetJazzEnterStateName();
                        float catchChance = data.CatchChance;
                        InsectRarity rarity = data.Rarity;
                        data = null;
                        mCurrentStateMachine = InsectData.AcquireInitAndEnterStateMachine(Actor, mTempInsect, mDummyIkJig, jazzInsectParameterName, jazzEnterStateName);
                        if (mCurrentStateMachine != null)
                        {
                            Collecting collecting = Actor.SkillManager.AddElement(SkillNames.Collecting) as Collecting;
                            if (collecting.IsBeetleCollector())
                            {
                                succeeded = true;
                            }
                            else
                            {
                                succeeded = RandomUtil.RandomChance01(catchChance);
                            }
                            AnimateSim(succeeded ? "ExitSuccess" : "ExitFailure");
                            if (succeeded)
                            {
                                InsectTerrarium.Create(Target, Actor);
                            }
                            else
                            {
                                ReactionTypes reactionType = RandomUtil.CoinFlip() ? ReactionTypes.Annoyed : (RandomUtil.CoinFlip() ? ReactionTypes.Shrug : ReactionTypes.Angry);
                                Actor.PlayReaction(reactionType, ReactionSpeed.NowOrLater);
                            }
                        }
                    }
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }

                StandardExit();
                DestroyObject(Target);
                return succeeded;
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

        private new class Definition : InsectJig.CatchInsect.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CatchInsectEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim a, InsectJig target, InteractionObjectPair interaction)
            {
                return InsectJig.CatchInsect.LocalizeString("Catch", new object[0x0]);
            }
        }
    }
}

