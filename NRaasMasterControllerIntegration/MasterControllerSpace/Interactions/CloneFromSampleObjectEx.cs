using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class CloneFromSampleObjectEx : ScientificSample.CloneFromSample, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<ScientificSample, ScientificSample.CloneFromSample.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<ScientificSample, ScientificSample.CloneFromSample.Definition>(Singleton);
        }

        public override bool Run()
        {
            if (!base.Actor.RouteToObjectRadiusAndCheckInUse(base.Target, 0.7f))
            {
                return false;
            }
            base.Actor.PlaySoloAnimation("a2o_object_genericSwipe_x", true);
            base.Target.FadeOut(true);
            if (!base.Actor.Inventory.TryToAdd(base.Target))
            {
                base.Target.SetOpacity(1f, 0f);
                base.Actor.AddExitReason(ExitReason.FailedToStart);
                return false;
            }
            base.Actor.RemoveExitReason(ExitReason.ObjectStateChanged);
            return this.RunFromInventory();
        }

        public override bool RunFromInventory()
        {
            if (this.mResearchStation == null)
            {
                IResearchStation[] objects = Sims3.Gameplay.Queries.GetObjects<IResearchStation>(base.Actor.LotCurrent);
                List<IResearchStation> list = base.Actor.LotCurrent.GetObjects<IResearchStation>(p => !p.Repairable.Broken);
                if (objects.Length < 1)
                {
                    base.Actor.AddExitReason(ExitReason.FailedToStart);
                    return false;
                }
                if (list.Count < 1)
                {
                    base.Actor.AddExitReason(ExitReason.FailedToStart);
                    return false;
                }
                this.mResearchStation = objects[0];
            }
            if (!base.Actor.RouteToSlotAndCheckInUse(this.mResearchStation, Slot.RoutingSlot_0))
            {
                return false;
            }
            GameObject mResearchStation = this.mResearchStation as GameObject;
            if (mResearchStation != null)
            {
                mResearchStation.EnableFootprintAndPushSims(ScientificSample.kScienceStationFootprintHash, base.Actor);
            }
            this.mVialsToUse = new List<ScientificSample>();
            base.Target.SetGeometryState(ScientificSample.kGeoStateInUse);
            base.Target.CreateClonesForInteraction(this.mVialsToUse, base.Actor);
            base.StandardEntry();
            base.BeginCommodityUpdates();
            if (base.Actor.TraitManager.HasElement(TraitNames.AntiTV))
            {
                base.Actor.BuffManager.AddElementPaused(BuffNames.AntiTV, Origin.FromScienceEquipment);
            }
            base.EnterStateMachine("ScienceStation", "EnterScienceStation", "x");
            base.AddOneShotScriptEventHandler(0x65, new SacsEventHandler(this.OnPlaceTargetOnTray));
            base.AddOneShotScriptEventHandler(0x66, new SacsEventHandler(this.OnRemoveTargetOnTray));
            base.AddOneShotScriptEventHandler(0x6f, new SacsEventHandler(this.OnStartStationLaser));
            base.AddOneShotScriptEventHandler(0xde, new SacsEventHandler(this.OnStopStationLaser));
            base.AddOneShotScriptEventHandler(0x20c, new SacsEventHandler(this.OnCloneSpawn));
            base.SetActor("researchStation", this.mResearchStation);
            base.Actor.Inventory.TryToRemove(base.Target);
            base.AnimateSim("PerformExperiment");
            bool succeeded = this.DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached | ExitReason.Canceled), new InteractionInstance.InsideLoopFunction(this.LoopDel), base.mCurrentStateMachine);
            if (base.Target.ScientificSampleType == ScientificSample.SampleType.Dna)
            {
                // custom
                this.DetermineHumanOutcomeEx(succeeded);
            }
            else
            {
                this.DetermineObjectOutcome(succeeded);
            }
            base.EndCommodityUpdates(succeeded);
            base.StandardExit();
            if (this.mCloneSim != null)
            {
                if (this.mCloneSim.Household != base.Actor.Household)
                {
                    this.mCloneSim.SimDescription.FirstName = StringInputDialog.Show(Localization.LocalizeString("Gameplay/Objects/RabbitHoles/ScienceLab:NameCloneTitle", new object[0]), Localization.LocalizeString("Gameplay/Objects/RabbitHoles/ScienceLab:NameCloneDesc", new object[0]), string.Empty, CASBasics.GetMaxNameLength(), StringInputDialog.Validation.SimNameText);
                    CarryingChildPosture posture = base.Actor.Posture as CarryingChildPosture;
                    if (posture != null)
                    {
                        base.Actor.InteractionQueue.AddNext(PutDownChild.Singleton.CreateInstance(posture.Child, base.Actor, new InteractionPriority(InteractionPriorityLevel.High), base.Autonomous, false));
                    }
                    SocialWorkerChildAbuse.Instance.MakeServiceRequest(base.Actor.LotCurrent, true, this.mCloneSim.ObjectId, true);
                }
                else
                {
                    this.mCloneSim.SimDescription.FirstName = StringInputDialog.Show(Localization.LocalizeString("Gameplay/Objects/RabbitHoles/ScienceLab:NameCloneTitle", new object[0]), Localization.LocalizeString("Gameplay/Objects/RabbitHoles/ScienceLab:NameCloneDesc", new object[0]), string.Empty, CASBasics.GetMaxNameLength(), StringInputDialog.Validation.SimNameText);
                    ScientificSample.DnaSampleSubject subject = base.Target.Subject as ScientificSample.DnaSampleSubject;
                    if (subject != null)
                    {
                        this.mCloneSim.SimDescription.LastName = subject.Subject.LastName;
                    }
                    else
                    {
                        this.mCloneSim.SimDescription.LastName = base.Actor.LastName;
                    }
                }
                ChildUtils.FinishObjectInteractionWithChild(this, this.mCloneSim);
            }
            return succeeded;
        }

        public void DetermineHumanOutcomeEx(bool succeeded)
        {
            this.DeleteVialsInCentrifuge();
            ScientificSample.DnaSampleSubject subject = base.Target.Subject as ScientificSample.DnaSampleSubject;
            bool flag = false;
            ScienceSkill element = (ScienceSkill)base.Actor.SkillManager.GetElement(SkillNames.Science);
            if (succeeded)
            {
                bool flag3;
                bool flag2 = RandomUtil.RandomChance01(kSuccessChance);
                if (flag2)
                {
                    flag3 = RandomUtil.RandomChance01(kEpicSuccessChance);
                }
                else
                {
                    flag3 = RandomUtil.RandomChance01(kEpicFailureChance);
                }
                if (((subject == null) /*|| !base.Actor.Household.CanAddSpeciesToHousehold(CASAgeGenderFlags.None | CASAgeGenderFlags.Human)*/) || GameUtils.IsUniversityWorld())
                {
                    flag2 = false;
                    flag3 = true;
                }
                if (flag2)
                {
                    string name = string.Empty;
                    if (flag3)
                    {
                        this.CreateBaby();
                        SetupPerfectCloneChildTraits(this.mCloneSim.SimDescription, this.mCloneSim, subject.Subject);
                        name = "SimCloneEpicSuccess";
                    }
                    else
                    {
                        this.CreateBaby();
                        SetupCloneTraitsFromArray(this.mCloneSim.SimDescription, this.mCloneSim, subject.Subject, kGoodCloneTraitNames);
                        name = "SimCloneSuccess";
                    }
                    if (element != null)
                    {
                        element.AddObjectsCreated();
                    }
                    flag = true;
                    base.Actor.ShowTNSIfSelectable(LocalizeString(base.Actor.IsFemale, name, new object[] { base.Actor, subject.Subject }), StyledNotification.NotificationStyle.kGameMessagePositive);
                    EventTracker.SendEvent(new SimDescriptionTargetEvent(EventTypeId.kClonedSim, base.Actor, subject.Subject));
                }
                else if (flag3)
                {
                    if (this.mCloneSim != null)
                    {
                        this.mCloneSim.Destroy();
                        this.mCloneSim = null;
                        this.mReplacementObject = null;
                    }
                    base.Actor.BuffManager.AddElement(BuffNames.MinorSetback, Origin.FromFailedExperiment);
                    BuffSinged.SingeViaInteraction(this, Origin.FromFailedExperiment);
                    base.AnimateSim("ExperimentRareEpicFail");
                    base.Actor.ShowTNSIfSelectable(LocalizeString(base.Actor.IsFemale, "SimCloneEpicFail", new object[] { base.Actor, subject.Subject }), StyledNotification.NotificationStyle.kGameMessageNegative);
                    if (element != null)
                    {
                        element.AddEpicFail();
                    }
                }
                else
                {
                    this.CreateBaby();
                    SetupCloneTraitsFromArray(this.mCloneSim.SimDescription, this.mCloneSim, subject.Subject, kBadCloneTraitNames);
                    flag = true;
                    VisualEffect.FireOneShotEffect("ep9ScienceCloneBaby_main", this.mResearchStation, unchecked((Slot)(-1474234202)), VisualEffect.TransitionType.HardTransition);
                    base.Actor.ShowTNSIfSelectable(LocalizeString(base.Actor.IsFemale, "SimCloneFail", new object[] { base.Actor, subject.Subject }), StyledNotification.NotificationStyle.kGameMessageNegative);
                    EventTracker.SendEvent(new SimDescriptionTargetEvent(EventTypeId.kClonedSim, base.Actor, subject.Subject));
                    if (element != null)
                    {
                        element.AddObjectsCreated();
                    }
                }
            }
            else
            {
                if (this.mCloneSim != null)
                {
                    this.mCloneSim.Destroy();
                    this.mCloneSim = null;
                    this.mReplacementObject = null;
                }
                base.Actor.BuffManager.AddElement(BuffNames.Mourning, unchecked((Origin)(-8359806666160896151L)));
                BuffSinged.SingeViaInteraction(this, Origin.FromFailedExperiment);
                base.AnimateSim("ExperimentRareEpicFail");
                base.Actor.ShowTNSIfSelectable(LocalizeString(base.Actor.IsFemale, "SimCloneEpicFail", new object[] { base.Actor, subject.Subject }), StyledNotification.NotificationStyle.kGameMessageNegative);
                if (element != null)
                {
                    element.AddEpicFail();
                }
            }
            if (flag)
            {
                base.SetActorAndEnter("y", this.mCloneSim, "NewBabyCloneEnter");
                base.AnimateJoinSims("CloneBabySuccessExit");
                ChildUtils.CarryChild(base.Actor, this.mCloneSim, false);
                this.mReplacementObject = null;
                this.mCloneSim.EnableInteractions(InteractionsDisabledType.All);
            }
        }

        public new class Definition : ScientificSample.CloneFromSample.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CloneFromSampleObjectEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, ScientificSample target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, ScientificSample target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                ScienceSkill element = (ScienceSkill)a.SkillManager.GetElement(SkillNames.Science);
                if ((element == null) || (element.SkillLevel < ScientificSample.CloneFromSample.kMinScienceSkillLevel))
                {
                    return false;
                }
                IResearchStation[] objects = Sims3.Gameplay.Queries.GetObjects<IResearchStation>(a.LotCurrent);
                List<IResearchStation> list = a.LotCurrent.GetObjects<IResearchStation>(p => !p.Repairable.Broken);
                if (objects.Length == 0)
                {
                    greyedOutTooltipCallback = new GreyedOutTooltipCallback(target.NoStationDisabledText);
                    return false;
                }
                if (list.Count < 1)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(a.IsFemale, "Gameplay/Objects/HobbiesSkills/ScienceResearchStation:IsBroken", new object[0]));
                    return false;
                }
                if (target.SubjectIsTooLarge())
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(ScientificSample.CloneFromSample.LocalizeString(a.IsFemale, "SubjectTooLarge", new object[0]));
                    return false;
                }
                if ((target.ScientificSampleType == ScientificSample.SampleType.Dna) && GameUtils.IsUniversityWorld())
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(a.IsFemale, "Gameplay/Actors/Sim:InteractionUnavailable", new object[0]));
                    return false;
                }
                /*
                if ((target.ScientificSampleType == ScientificSample.SampleType.Dna) && !a.Household.CanAddSpeciesToHousehold(CASAgeGenderFlags.None | CASAgeGenderFlags.Human))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(a.IsFemale, "UI/Feedback/CAS:ErrorMsg4", new object[0]));
                    return false;
                }
                 */
                return true;
            }
        }
    }
}