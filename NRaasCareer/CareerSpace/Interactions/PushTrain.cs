using NRaas.CareerSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;
using Sims3.UI;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Interactions
{
    public class PushTrain : TrainingDummy.PushTrain, Common.IPreLoad, Common.IAddInteraction
    {
        public new static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<TrainingDummy, TrainingDummy.PushTrain.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<TrainingDummy>(Singleton);
        }

        public override bool Run()
        {
            bool flag = false;
            Sim selectedObject = GetSelectedObject() as Sim;
            if (selectedObject != null)
            {
                InteractionInstance entry = TrainingDummy.Practice.Singleton.CreateInstance(Target, selectedObject, Actor.InheritedPriority(), Autonomous, true);
                InteractionInstance instance = MartialArtsTrain.Singleton.CreateInstance(Target, Actor, Actor.InheritedPriority(), Autonomous, true);
                entry.LinkedInteractionInstance = instance;
                flag = selectedObject.InteractionQueue.Add(entry) && Actor.InteractionQueue.PushAsContinuation(instance, true);
            }
            return flag;
        }

        protected new class Definition : InteractionDefinition<Sim, TrainingDummy, PushTrain>
        {
            public override string GetInteractionName(Sim actor, TrainingDummy target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(TrainingDummy.PushTrain.Singleton, target));
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                NumSelectableRows = 0x1;
                List<Sim> validCandidates = TrainingDummy.PushTrain.GetValidCandidates(parameters.Actor as Sim, parameters.Target as TrainingDummy);
                base.PopulateSimPicker(ref parameters, out listObjs, out headers, validCandidates, false);
            }

            public override bool Test(Sim a, TrainingDummy target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (a.SkillManager.GetSkillLevel(SkillNames.MartialArts) >= 7)
                {
                    return false;
                }

                if (SkillBasedCareerBooter.GetSkillBasedCareer(a, SkillNames.MartialArts) == null)
                {
                    return false;
                }

                GreyedOutTooltipCallback callback = null;
                if (target.mTrainee == null)
                {
                    if (TrainingDummy.PushTrain.GetValidCandidates(a, target).Count != 0x0)
                    {
                        return true;
                    }
                    if (callback == null)
                    {
                        callback = delegate
                        {
                            return Common.LocalizeEAString(a.IsFemale, "Gameplay/Objects/HobbiesSkills/TrainingDummy/Train:NoOneToTrain", new object[0x0]);
                        };
                    }
                    greyedOutTooltipCallback = callback;
                }

                return false;
            }
        }
    }
}
