using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace.Helpers;
using NRaas.HybridSpace.Interfaces;
using NRaas.HybridSpace.MagicControls;
using NRaas.HybridSpace.Proxies;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Interactions
{
    public class SpellcastingDuelEx : MagicWand.SpellcastingDuel, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        bool mWandXCreated;
        bool mWandYCreated;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, MagicWand.SpellcastingDuel.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, MagicWand.SpellcastingDuel.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                IMagicalDefinition definition = InteractionDefinition as IMagicalDefinition;

                bool succeeded = true;
                Actor.ClearExitReasons();
                if (mIsChallenger || (mChallengerSim == null))
                {
                    mIsChallenger = true;
                    mChallengerSim = Actor;
                    mDefenderSim = Target;
                    if (mDefenderSim == null)
                    {
                        return false;
                    }

                    EventTracker.SendEvent(EventTypeId.kChallengeToSpellcasting, mChallengerSim, mDefenderSim);
                    
                    mJig = GlobalFunctions.CreateObjectOutOfWorld("castSpellDuel_jig", ProductVersion.EP7) as SocialJigTwoPerson;
                    mJig.RegisterParticipants(mDefenderSim, mChallengerSim);
                    mJig.SetOpacity(0f, 0f);
                    
                    Vector3 position = mDefenderSim.Position;
                    Vector3 forwardVector = mDefenderSim.ForwardVector;
                    if (!GlobalFunctions.FindGoodLocationNearby(mJig, ref position, ref forwardVector))
                    {
                        return false;
                    }
                    
                    mJig.SetPosition(position);
                    mJig.SetForward(forwardVector);
                    mJig.AddToWorld();
                    if (!Actor.DoRoute(mJig.RouteToJigB(mChallengerSim)))
                    {
                        return false;
                    }
                    
                    mRouteComplete = true;
                    mInteractionPersonB = SingletonPersonB.CreateInstance(mChallengerSim, mDefenderSim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true) as MagicWand.SpellcastingDuel;
                    mInteractionPersonB.LinkedInteractionInstance = this;
                    mInteractionPersonB.mChallengerSim = mChallengerSim;
                    mInteractionPersonB.Jig = mJig;
                    mInteractionPersonB.mDefenderSim = mDefenderSim;
                    mInteractionPersonB.IsChallenger = false;
                    mInteractionPersonB.CancellableByPlayer = false;
                    mDefenderSim.InteractionQueue.AddNext(mInteractionPersonB);
                    PresetDuelResults();
                }
                else
                {
                    if (!Actor.DoRoute(mJig.RouteToJigA(mDefenderSim)))
                    {
                        return false;
                    }
                    mRouteComplete = true;
                }

                MagicControl controlX = MagicControl.GetBestControl(mChallengerSim, definition);
                if (controlX == null) return false;

                MagicControl controlY = MagicControl.GetBestControl(mDefenderSim, definition);
                if (controlY == null) return false;
 
                StandardEntry(true);
                BeginCommodityUpdates();
                if (mIsChallenger)
                {
                    while ((!mRouteComplete || !mInteractionPersonB.RouteComplete) && (mDefenderSim.InteractionQueue.HasInteraction(mInteractionPersonB) && !Actor.HasExitReason()))
                    {
                        SpeedTrap.Sleep(0x0);
                    }
                
                    if (mDefenderSim.HasExitReason() || mChallengerSim.HasExitReason())
                    {
                        mDefenderSim.AddExitReason(ExitReason.Canceled);
                        EndCommodityUpdates(false);
                        StandardExit();
                        return false;
                    }
                    
                    AcquireStateMachine("spellcastingDuel");
                    
                    SetActor("x", mChallengerSim);
                    SetActor("y", mDefenderSim);

                    mWandX = controlX.InitialPrep(mChallengerSim, definition, out mWandXCreated);
                    mWandY = controlY.InitialPrep(mDefenderSim, definition, out mWandYCreated);
                    if ((mWandX == null) || (mWandY == null))
                    {
                        return false;
                    }

                    mWandX.PrepareForUse(mChallengerSim);
                    if (mWandX is MagicHands)
                    {
                        SetParameter("noWandX", true);
                    }
                    else
                    {
                        SetParameter("noWandX", false);
                    }

                    mWandY.PrepareForUse(mDefenderSim);
                    if (mWandY is MagicHands)
                    {
                        SetParameter("noWandY", true);
                    }
                    else
                    {
                        SetParameter("noWandY", false);
                    }

                    SetActor("wandX", mWandX);
                    SetActor("wandY", mWandY);
                    
                    SetParameter("x:Age", Actor.SimDescription.Age);
                    SetParameter("y:Age", Actor.SimDescription.Age);
                    
                    EnterState("x", "Enter");
                    EnterState("y", "Enter");
                    
                    AddPersistentScriptEventHandler(0x65, CastingEffect);
                    AddPersistentScriptEventHandler(0x66, BlockingEffect);
                    AddPersistentScriptEventHandler(0x67, HitEffect);
                    AddPersistentScriptEventHandler(0x68, BlockingEffectTwo);
                    ReturnToIdle();
                    
                    for (int i = 0x0; i < mTotalRounds; i++)
                    {
                        mCurrResult = mRoundResults[i];
                        mCastType = (CastingType)RandomUtil.GetInt(0x4);
                
                        switch(mCurrResult)
                        {
                            case RoundResult.ChallengerHit:
                                AnimateJoinSims("XHit");
                                break;
                            case RoundResult.ChallengerDefend:
                                AnimateJoinSims("XDef");
                                break;
                            case RoundResult.DefenderHit:
                                AnimateJoinSims("YHit");
                                break;
                            case RoundResult.DefenderDefend:
                                AnimateJoinSims("YDef");
                                break;
                        }

                        ReturnToIdle();
                    }
                    
                    switch (mDuelResult)
                    {
                        case DuelResult.ChallengerWin:
                            AnimateJoinSims("XWin");
                            mChallengerSim.BuffManager.AddElement(BuffNames.WonASpellcastingDuel, Origin.FromSpellcastingDuel);
                            mDefenderSim.BuffManager.AddElement(BuffNames.LostASpellcastingDuel, Origin.FromSpellcastingDuel);
                            EventTracker.SendEvent(EventTypeId.kWonASpellcastingDuel, mChallengerSim);
                            break;
                        case DuelResult.DefenderWin:
                            AnimateJoinSims("YWin");
                            mDefenderSim.BuffManager.AddElement(BuffNames.WonASpellcastingDuel, Origin.FromSpellcastingDuel);
                            mChallengerSim.BuffManager.AddElement(BuffNames.LostASpellcastingDuel, Origin.FromSpellcastingDuel);
                            EventTracker.SendEvent(EventTypeId.kWonASpellcastingDuel, mDefenderSim);
                            break;
                        default:
                            AnimateJoinSims("Tie");
                            mChallengerSim.BuffManager.AddElement(BuffNames.DrawnASpellcastingDuel, Origin.FromSpellcastingDuel);
                            mDefenderSim.BuffManager.AddElement(BuffNames.DrawnASpellcastingDuel, Origin.FromSpellcastingDuel);
                            break;
                    }
                    mDefenderSim.AddExitReason(ExitReason.Finished);
                }
                else
                {
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                }

                EndCommodityUpdates(succeeded);
                StandardExit(true);
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

        public override void Cleanup()
        {
            base.Cleanup();

            if (mWandXCreated)
            {
                mWandX.Destroy();
                mWandX = null;
            }

            if (mWandYCreated)
            {
                mWandY.Destroy();
                mWandY = null;
            }
        }

        public new class Definition : MagicWand.SpellcastingDuel.Definition, IMagicalDefinition
        {
            static PersistedSettings.SpellSettings sSettings;

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SpellcastingDuelEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous;
            }

            public MagicControl IntendedControl
            {
                get { return WitchControl.sControl; }
            }

            public PersistedSettings.SpellSettings DefaultSettings
            {
                get
                {
                    return new PersistedSettings.SpellSettings(OccultTypes.Witch, MagicWand.kSpellLevels[0x8], CastFireBlastEx.kMotiveDrain, 0);
                }
            }

            public PersistedSettings.SpellSettings SpellSettings
            {
                get
                {
                    return sSettings;
                }
                set
                {
                    sSettings = value;
                }
            }
        }
    }
}
