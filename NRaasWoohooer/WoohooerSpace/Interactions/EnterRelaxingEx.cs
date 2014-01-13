using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class EnterRelaxingEx : EnterRelaxing, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;
        
        public static InteractionDefinition WoohooSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Bed, EnterRelaxing.Definition, Definition>(false);

            InteractionTuning tuning = Tunings.GetTuning<Bed, BedRelax.Definition>();
            if (tuning != null)
            {
                tuning.AddFlags(InteractionTuning.FlagField.DisallowAutonomous);
            }

            sOldSingleton = Singleton;

            Singleton = new Definition(false);
            WoohooSingleton = new Definition(true);
        }

        private new void MaybePushRelax()
        {
            if (!ExitImmediately && Actor.InteractionQueue.QueueEmptyAfter(this))
            {
                LowerPriority();
                Actor.InteractionQueue.Add(BedRelax.Singleton.CreateInstance(Target, Actor, GetPriority(), false, true));
                RaisePriority();
            }
        }

        public override bool Run()
        {
            try
            {
                if (Route())
                {
                    if (!Target.CanBeUsedAsBed)
                    {
                        return false;
                    }

                    HeartShapedBed target = Target as HeartShapedBed;
                    if (((target != null) && target.IsVibrating) && !Target.IsActorUsingMe(Actor))
                    {
                        target.SetVibration(Actor, false);
                    }

                    StandardEntry(false);

                    Definition definition = InteractionDefinition as Definition;

                    try
                    {
                        if (ShouldChangeIntoSleepwear())
                        {
                            GameObject container = mEntryPart.Container;
                            if (!container.IsActorUsingMe(Actor))
                            {
                                container.AddToUseList(Actor);
                            }

                            Actor.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToBed);
                        }
                        else if (definition.mIsWoohoo)
                        {
                            GameObject container = mEntryPart.Container;
                            if (!container.IsActorUsingMe(Actor))
                            {
                                container.AddToUseList(Actor);
                            }

                            if (!container.IsOutside)
                            {
                                if (Woohooer.Settings.mNakedOutfitBed)
                                {
                                    Actor.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToBathe);

                                    Woohooer.Settings.AddChange(Actor);
                                }
                                else if (Woohooer.Settings.mChangeForBedWoohoo)
                                {
                                    Actor.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToBed);
                                }
                            }
                        }
                    }
                    catch (ResetException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        Common.Exception(Actor, Target, e);
                    }

                    Target.AddBuffs(Actor);
                    if (Actor.Posture is RelaxingPosture)
                    {
                        MaybePushRelax();
                        DoCommodityUpdates();
                        StandardExit(false, false);
                        return true;
                    }
                    else if (mEntryPart.RelaxOnBed(Actor, mEntryStateName))
                    {
                        if (Actor.HasExitReason(ExitReason.UserCanceled))
                        {
                            Actor.AddExitReason(ExitReason.CancelledByPosture);
                            StandardExit(false, false);
                            return false;
                        }
                        DoCommodityUpdates();
                        
                        MaybePushRelax();

                        StandardExit(false, false);
                        return true;
                    }

                    StandardExit(false, false);

                    if (ExitImmediately)
                    {
                        Actor.Posture.CancelPosture(Actor);
                    }
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }

            return false;
        }

        public new class Definition : EnterRelaxing.Definition
        {
            public readonly bool mIsWoohoo;

            public Definition()
            {
                mIsWoohoo = false;
            }
            public Definition(bool woohoo)
            { 
                mIsWoohoo = woohoo;
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new EnterRelaxingEx();
                result.Init(ref parameters);

                return result;
            }

            public override string GetInteractionName(Sim actor, Bed target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
