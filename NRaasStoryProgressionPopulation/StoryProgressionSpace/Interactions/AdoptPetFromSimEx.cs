using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class AdoptPetFromSimEx : SocialInteraction, Common.IPreLoad, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, PetAdoption.AdoptPetFromSim.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        private bool FinishAdoption(Sim pet)
        {
            if (pet != null)
            {
                pet.InteractionQueue.CancelAllInteractions();
                pet.SetObjectToReset();

                SpeedTrap.Sleep();

                pet.UpdateBlockTime();
                Actor.ShowTNSAndPlayStingIfSelectable("sting_pet_adopt", TNSNames.AdoptLitterTNS, Target, pet, null, null, pet.IsFemale, false, new object[] { pet });

                if (pet == Sim.ActiveActor)
                {
                    LotManager.SelectNextSim();
                }

                if (pet.Household != null)
                {
                    pet.Household.Remove(pet.SimDescription);
                }
                Actor.Household.Add(pet.SimDescription);

                Relationships.CheckAddHumanParentFlagOnAdoption(Actor.SimDescription, pet.SimDescription);

                if (Actor.Partner != null)
                {
                    Relationships.CheckAddHumanParentFlagOnAdoption(Actor.Partner, pet.SimDescription);
                }

                InteractionInstance entry = new SocialInteractionA.Definition("Talk To Pet", new string[0x0], null, false).CreateInstance(pet, Actor, GetPriority(), false, false);
                
                List<Sim> list = new List<Sim>();
                list.Add(pet);
                GoHome home = GoHome.Singleton.CreateInstance(Actor.LotHome, Actor, GetPriority(), false, false) as GoHome;
                home.SimFollowers = list;

                Actor.InteractionQueue.AddNext(home);                
                return Actor.InteractionQueue.AddNext(entry);
            }
            return false;
        }

        public override bool Run()
        {
            try
            {
                if (!SafeToSync())
                {
                    Common.DebugNotify("SafeToSync Fail");
                    return false;
                }

                SocialJig = GlobalFunctions.CreateObjectOutOfWorld("SocialJigTwoPerson") as SocialJigTwoPerson;

                if (!BeginSocialInteraction(new SocialInteractionB.Definition(null, Common.LocalizeEAString("Gameplay/Actors/Sim/AdoptPetFromSim:InteractionName"), false), true, false))
                {
                    Common.DebugNotify("BeginSocialInteraction Fail");
                    return false;
                }

                StandardEntry();
                EnterStateMachine("social_greet", "enter", "x", "y");
                SetParameter("GreetType", GreetType.HandShake);
                AnimateJoinSims("accept");
                FinishLinkedInteraction(true);
                StandardExit();
                WaitForSyncComplete();

                Sim pet = GetSelectedObject() as Sim;
                if (pet == null)
                {
                    Common.DebugNotify("Pet Fail");
                    return false;
                }

                return FinishAdoption(pet);
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

        public static List<Sim> GetPotentials(Sim actor, Sim target)
        {
            Lot lotHome = actor.LotHome;

            bool allowHorses = ((lotHome == null) || (!lotHome.HasVirtualResidentialSlots));

            List<Sim> results = new List<Sim>();

            if (target.Household != null)
            {
                foreach (Sim sim in Households.AllPets(target.Household))
                {
                    if (!sim.SimDescription.ChildOrBelow) continue;

                    if (sim.IsHorse)
                    {
                        if (!allowHorses) continue;
                    }

                    results.Add(sim);
                }
            }

            return results;
        }

        public class Definition : SocialInteraction.SocialInteractionDefinition<AdoptPetFromSimEx>
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(PetAdoption.AdoptPetFromSim.Singleton, target));
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                try
                {
                    NumSelectableRows = 0x1;
                    PopulateSimPicker(ref parameters, out listObjs, out headers, GetPotentials(parameters.Actor as Sim, parameters.Target as Sim), false);
                }
                catch (Exception e)
                {
                    listObjs = null;
                    headers = null;
                    NumSelectableRows = 0x0;

                    Common.Exception(parameters.Actor as Sim, parameters.Target as Sim, e);
                }
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!target.IsHuman) return false;

                    if (a.Household == target.Household) return false;

                    if (!target.SimDescription.TeenOrAbove) return false;

                    return (GetPotentials(a, target).Count > 0);
                }
                catch (Exception e)
                {
                    Common.Exception(a, target, e);
                    return false;
                }
            }
        }
    }
}

