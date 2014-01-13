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
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class CaptureEx : MinorPet.Capture, Common.IPreLoad
    {
        public new static readonly InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<MinorPet, MinorPet.Capture.Definition, Definition>(false);
        }

        public override bool Run()
        {
            try
            {
                if (!Actor.RouteToObjectRadialRangeAndCheckInUse(Target, Target.MinorPetTuning.CaptureRouteRange[0x0], Target.MinorPetTuning.CaptureRouteRange[0x1]))
                {
                    return false;
                }
                bool escaped = Target.Escaped;
                StandardEntry();
                EnterStateMachine("MinorPetCapture", "Enter", "x");
                SetActor("pet", Target);
                SetParameter("petType", Target.Data.MinorPetType);
                BeginCommodityUpdates();
                bool succeeded = escaped || RandomUtil.RandomChance01(Target.Data.CatchChance);
                if (succeeded)
                {
                    Target.FadeOut();
                    Target.StopBehaviorSMC();
                    AnimateSim("Capture");

                    // Custom
                    if (Actor.Inventory.TryToAdd(Target, false))
                    {
                        string str;
                        if (escaped)
                        {
                            str = Localization.LocalizeString(Actor.IsFemale, "Gameplay/Objects/MinorPets/Recapture:TNS", new object[] { Actor, Target.GetLocalizedName() });
                        }
                        else
                        {
                            Target.Captured = true;
                            string localizedPetType = MinorPet.GetLocalizedPetType(Target.Data.MinorPetType);
                            string localizedPetRarity = MinorPet.GetLocalizedPetRarity(Target.Data.Rarity);
                            str = Localization.LocalizeGenderString("Gameplay/Objects/MinorPets:CaptureTNS", new object[] { Actor, Actor, Target.LocalizedSpeciesName, localizedPetType, localizedPetRarity });
                        }

                        Actor.ShowTNSIfSelectable(str, StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, Actor.ObjectId);
                        EventTracker.SendEvent(new MinorPetEvent(EventTypeId.kCaughtMinorPet, Actor, Target, Target.Data.MinorPetType));
                        using (List<Sim>.Enumerator enumerator = Actor.Household.Sims.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                Sim current = enumerator.Current;
                                EventTracker.SendEvent(new Event(EventTypeId.kArkBuilder, Actor, Target));
                            }
                        }
                        if (Target is IBird)
                        {
                            Tutorialette.TriggerLesson(Lessons.Birds, null);
                        }
                        else
                        {
                            Tutorialette.TriggerLesson(Lessons.MinorPets, null);
                        }
                    }
                    else
                    {
                        mDestroyPet = true;
                    }
                }
                else
                {
                    Target.FadeOut();
                    mDestroyPet = true;
                    bool flag3 = RandomUtil.CoinFlip();
                    AnimateSim(flag3 ? "CaptureFailBad" : "CaptureFail");
                    if (flag3)
                    {
                        Actor.BuffManager.AddElement(BuffNames.Bitten, Origin.FromUncapturedMinorPet);
                    }
                }
                AnimateSim("Exit");
                EndCommodityUpdates(succeeded);
                StandardExit();
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

        private new class Definition : MinorPet.Capture.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CaptureEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, MinorPet target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(MinorPet.Capture.Singleton, target));
            }
        }
    }
}

