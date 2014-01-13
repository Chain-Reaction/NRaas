using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Miscellaneous.Shopping;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
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
    public class ClothingPedestalPlanOutfitEx : ClothingPedestal.PlanOutfit, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<ClothingPedestal, ClothingPedestal.PlanOutfit.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<ClothingPedestal, ClothingPedestal.PlanOutfit.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                if (!Target.RouteToPedestal(Actor, true))
                {
                    return false;
                }

                Definition interactionDefinition = InteractionDefinition as Definition;
                if (Responder.Instance.OptionsModel.SaveGameInProgress)
                {
                    return false;
                }

                Target.DisplayAge = interactionDefinition.mAge;
                Target.DisplayGender = interactionDefinition.mGender;
                StandardEntry();
                BeginCommodityUpdates();
                EnterStateMachine("ShoppingPedestal", "Enter", "x");
                SetParameter("x:Age", Actor.SimDescription.Age);
                AnimateSim("Change Item");
                SimDescription simDesc = new SimDescription();
                if (simDesc == null)
                {
                    throw new Exception("ChangeOutfit:  sim doesn't have a description!");
                }

                Target.PedestalOutfitsSaveTo(simDesc);
                Household.CreateTouristHousehold();
                Household.TouristHousehold.AddTemporary(simDesc);
                try
                {
                    if (GameUtils.IsInstalled(ProductVersion.EP2))
                    {
                        new Sims.Stylist(Sims.CASBase.EditType.Mannequin, Target.DisplayCategory).Perform(new GameHitParameters<SimDescriptionObject>(Sim.ActiveActor, new SimDescriptionObject(simDesc), GameObjectHit.NoHit));
                    }
                    else
                    {
                        new Sims.Dresser(Sims.CASBase.EditType.Mannequin, Target.DisplayCategory).Perform(new GameHitParameters<SimDescriptionObject>(Sim.ActiveActor, new SimDescriptionObject(simDesc), GameObjectHit.NoHit));
                    }

                    while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                    {
                        SpeedTrap.Sleep(0x0);
                    }
                    Actor.InteractionQueue.CancelAllInteractionsByType(Singleton);
                }
                finally
                {
                    Household.TouristHousehold.RemoveTemporary(simDesc);
                }

                Target.PedestalOutfitsLoadFrom(simDesc, Actor);
                Target.ChangeOutfit();
                AnimateSim("Exit");
                EndCommodityUpdates(true);
                StandardExit();
                return true;
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

        public new class Definition : ClothingPedestal.PlanOutfit.Definition
        {
            public Definition()
            { }
            public Definition(string ageText, CASAgeGenderFlags age, string genderText, CASAgeGenderFlags gender)
                : base(ageText, age, genderText, gender)
            {}

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new ClothingPedestalPlanOutfitEx();
                na.Init(ref parameters);
                return na;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim sim, ClothingPedestal target, List<InteractionObjectPair> results)
            {
                string[] strArray = new string[] { "Child", "Teen", "YoungAdult", "Adult", "Elder" };
                CASAgeGenderFlags[] flagsArray = new CASAgeGenderFlags[] { CASAgeGenderFlags.Child, CASAgeGenderFlags.Teen, CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Adult, CASAgeGenderFlags.Elder };
                string[] strArray2 = new string[] { "Male", "Female" };
                CASAgeGenderFlags[] flagsArray2 = new CASAgeGenderFlags[] { CASAgeGenderFlags.None | CASAgeGenderFlags.Male, CASAgeGenderFlags.None | CASAgeGenderFlags.Female };
                for (int i = 0x0; i < flagsArray.Length; i++)
                {
                    for (int j = 0x0; j < flagsArray2.Length; j++)
                    {
                        results.Add(new InteractionObjectPair(new Definition(Localization.LocalizeString("UI/Feedback/CAS:" + strArray[i], new object[0x0]), flagsArray[i], Localization.LocalizeString("Ui/Tooltip/CAS/Basics:" + strArray2[j], new object[0x0]), flagsArray2[j]), iop.Target));
                    }
                }
            }

            public override string GetInteractionName(Sim actor, ClothingPedestal target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
