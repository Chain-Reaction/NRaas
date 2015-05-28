using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
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
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.UI.Hud;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class CreateBabyWithEx : Hospital.CreateBabyWith, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Hospital, Hospital.CreateBabyWith.CreateBabyWithDefinition>(Singleton);
        }

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.GetTuning<Hospital, Hospital.CreateBabyWith.CreateBabyWithDefinition>();
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            Tunings.Inject<Hospital, Hospital.CreateBabyWith.CreateBabyWithDefinition, Definition>(false);            

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public class Definition : Hospital.CreateBabyWith.CreateBabyWithDefinition
        {
            public new bool customize;
            public new string name;
            public new string[] path;

            public Definition()
            {
                this.name = string.Empty;
            }

            public Definition(string parent, string child, bool custom)
            {
                this.name = string.Empty;
                this.name = child;
                this.path = new string[] { parent };
                this.customize = custom;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Hospital target, List<InteractionObjectPair> results)
            {
                string parent = Localization.LocalizeString(actor.IsFemale, "Gameplay/Objects/RabbitHoles/Hospital/CreateBabyWith:InteractionName", new object[] { Hospital.CreateBabyWith.kCostOfBabyMaking });
                results.Add(new InteractionObjectPair(new Definition(parent, Localization.LocalizeString(actor.IsFemale, "Gameplay/Objects/RabbitHoles/Hospital/CreateBabyWith:Customize", new object[0]), true), iop.Target));
                results.Add(new InteractionObjectPair(new Definition(parent, Localization.LocalizeString(actor.IsFemale, "Gameplay/Objects/RabbitHoles/Hospital/CreateBabyWith:Randomize", new object[0]), false), iop.Target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new CreateBabyWithEx();
                result.Init(ref parameters);
                return result;
            }

            public override string GetInteractionName(Sim a, Hospital target, InteractionObjectPair interaction)
            {
                if (this.name != string.Empty)
                {
                    return this.name;
                }
                return Localization.LocalizeString(a.IsFemale, "Gameplay/Objects/RabbitHoles/Hospital/CreateBabyWith:InteractionName", new object[] { Hospital.CreateBabyWith.kCostOfBabyMaking });
            }

            public override string[] GetPath(bool isFemale)
            {
                if (this.path != null)
                {
                    return this.path;
                }
                return new string[0];
            }

            public new List<Sim> GetSims(Sim actor)
            {
                List<Sim> list = new List<Sim>();
                foreach (Relationship relationship in actor.SocialComponent.Relationships)
                {
                    SimDescription otherSimDescription = relationship.GetOtherSimDescription(actor.SimDescription);
                    if ((((LTRData.Get(relationship.LTR.CurrentLTR).Score >= Hospital.CreateBabyWith.kLTRForBabyMaking) && (otherSimDescription.CreatedSim != null)) && (this.CanMakeBabyWith(actor, otherSimDescription.CreatedSim) && !otherSimDescription.CreatedSim.IsSleeping)) && !otherSimDescription.CreatedSim.IsAtWork)
                    {
                        list.Add(otherSimDescription.CreatedSim);
                    }
                }
                return list;
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                NumSelectableRows = 1;
                base.PopulateSimPicker(ref parameters, out listObjs, out headers, this.GetSims((Sim)parameters.Actor), false);
            }

            private new bool CanMakeBabyWith(Sim actor, Sim target)
            {
                BuffBetrayed.BuffInstanceBetrayed betrayed;
                if (actor == target)
                {
                    return false;
                }
                if (actor.SimDescription.Species != target.SimDescription.Species)
                {
                    return false;
                }
                if ((target.IsRobot || target.OccultManager.HasOccultType(OccultTypes.Mummy)) || (target.OccultManager.HasOccultType(OccultTypes.PlantSim) || HolographicProjectionSituation.IsSimHolographicallyProjected(target)))
                {
                    return false;
                }                
                if (target.OccultManager.HasOccultType(OccultTypes.TimeTraveler))
                {
                    return false;
                }
                if (target.Service is GrimReaper)
                {
                    return false;
                }
                if (BuffBetrayed.DoesSimFeelBetrayed(actor, target.SimDescription, out betrayed))
                {
                    return false;
                }
                if (!OccultImaginaryFriend.CanSimGetRomanticWithSim(actor, target))
                {
                    return false;
                }
                if ((target.CurrentInteraction != null) && (target.InteractionQueue.HasInteractionOfType(this) || target.InteractionQueue.HasInteractionOfType(Hospital.BeForcedToMakeBabyWith.Singleton)))
                {
                    return false;
                }
                GreyedOutTooltipCallback callback = null;
                string reason;
                if (!CommonSocials.CanGetRomantic(actor, target, false, true, true, ref callback, out reason))
                {
                    return false;
                }
                CASAgeGenderFlags flags = CASAgeGenderFlags.YoungAdult | CASAgeGenderFlags.Elder | CASAgeGenderFlags.Adult;
                if (Woohooer.Settings.AllowTeen(true))
                {
                    flags = flags | CASAgeGenderFlags.Teen;
                }
                return (((actor.SimDescription.Age & flags) != CASAgeGenderFlags.None) && ((target.SimDescription.Age & flags) != CASAgeGenderFlags.None));
            }

            public override bool Test(Sim a, Hospital target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if ((a.IsRobot || a.OccultManager.HasOccultType(OccultTypes.Mummy)) || a.OccultManager.HasOccultType(OccultTypes.PlantSim))
                {
                    return false;
                }
                if (a.FamilyFunds < Hospital.CreateBabyWith.kCostOfBabyMaking)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(LocalizationHelper.InsufficientFunds);
                    return false;
                }
                /*
                if (!a.SimDescription.Household.CanAddSpeciesToHousehold(CASAgeGenderFlags.None | CASAgeGenderFlags.Human, 1, true))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(a.IsFemale, "Gameplay/Objects/RabbitHoles/Hospital/CreateBabyWith:HouseholdFull", new object[0]));
                    return false;
                }
                 */
                if (this.GetSims(a).Count == 0)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(a.IsFemale, "Gameplay/Objects/RabbitHoles/Hospital/CreateBabyWith:NoFriendsNearBy", new object[0]));
                    return false;
                }
                if ((a.CurrentInteraction == null) || (!a.InteractionQueue.HasInteractionOfType(this) && !a.InteractionQueue.HasInteractionOfType(Hospital.BeForcedToMakeBabyWith.Singleton)))
                {
                    return true;
                }
                greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(a.IsFemale, "Gameplay/Objects/RabbitHoles/Hospital/CreateBabyWith:HaveToWait", new object[0]));
                return false;
            }
        }
    }
}
