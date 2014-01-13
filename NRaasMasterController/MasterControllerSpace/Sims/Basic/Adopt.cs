using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class Adopt : SimFromList, IBasicOption
    {
        AdoptReturnStructure mAdoptionParams;

        public override string GetTitlePrefix()
        {
            return "Adopt";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.LotHome == null) return false;

            return base.PrivateAllow(me);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                mAdoptionParams = AdoptionDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt", me.IsFemale, new object[] { me }));
                if (mAdoptionParams.mCancelled) return false;
            }

            List<SimDescription> residents = new List<SimDescription>();

            foreach (SimDescription sim in SimListing.GetResidents(true).Values)
            {
                if (!sim.IsHuman) continue;

                residents.Add(sim);
            }

            SimDescription dad = RandomUtil.GetRandomObjectFromList(residents);
            SimDescription mom = RandomUtil.GetRandomObjectFromList(residents);

            SimDescription newKid = null;

            if ((dad != null) && (mom != null))
            {
                if (dad.CelebrityManager == null)
                {
                    dad.Fixup();
                }

                if (mom.CelebrityManager == null)
                {
                    mom.Fixup();
                }

                newKid = Genetics.MakeDescendant(dad, mom, mAdoptionParams.mAge, mAdoptionParams.mIsFemale ? CASAgeGenderFlags.Female : CASAgeGenderFlags.Male, 100, new Random(), false, false, true);
            }
            else
            {
                SimUtils.SimCreationSpec spec = new SimUtils.SimCreationSpec();
                spec.Gender = mAdoptionParams.mIsFemale ? CASAgeGenderFlags.Female : CASAgeGenderFlags.Male;
                spec.Age = mAdoptionParams.mAge;
                spec.Normalize();
                newKid = spec.Instantiate();
            }

            if (newKid == null)
            {
                Common.Notify(Common.Localize(GetTitlePrefix() + ":Failure"));
                return false;
            }

            string genderName = null;
            if (newKid.IsFemale)
            {
                genderName = Common.Localize("BabyGender:Female");
            }
            else
            {
                genderName = Common.Localize("BabyGender:Male");
            }

            string name = StringInputDialog.Show(Name, Common.Localize("InstaBaby:NamePrompt", newKid.IsFemale, new object[0]), newKid.FirstName);
            if (!string.IsNullOrEmpty(name))
            {
                newKid.FirstName = name;
            }

            newKid.LastName = me.LastName;

            me.Household.Add(newKid);

            newKid.WasAdopted = true;

            Sim adoptedChild = Instantiation.Perform(newKid, null);
            if (adoptedChild != null)
            {
                ResetSimTask.UpdateInterface(adoptedChild);

                SocialWorkerAdoptionSituation.InstantiateNewKid instantiateNewKid = new SocialWorkerAdoptionSituation.InstantiateNewKid();

                instantiateNewKid.AssignTraits(adoptedChild);

                instantiateNewKid.GiveImaginaryFriendDoll(newKid);

                me.Genealogy.AddChild(newKid.Genealogy);

                if (me.CreatedSim != null)
                {
                    ActiveTopic.AddToSim(me.CreatedSim, "Recently Had Baby");

                    EventTracker.SendEvent(EventTypeId.kAdoptedChild, me.CreatedSim, adoptedChild);
                    EventTracker.SendEvent(EventTypeId.kNewOffspring, me.CreatedSim, adoptedChild);
                    EventTracker.SendEvent(EventTypeId.kParentAdded, adoptedChild, me.CreatedSim);
                }

                MidlifeCrisisManager.OnHadChild(me);

                Genealogy spouse = me.Genealogy.Spouse;
                if (spouse != null)
                {
                    spouse.AddChild(newKid.Genealogy);

                    SimDescription spouseDesc = spouse.SimDescription;
                    if (spouseDesc != null)
                    {
                        MidlifeCrisisManager.OnHadChild(spouseDesc);

                        if (spouseDesc.CreatedSim != null)
                        {
                            ActiveTopic.AddToSim(spouseDesc.CreatedSim, "Recently Had Baby");

                            EventTracker.SendEvent(EventTypeId.kAdoptedChild, spouseDesc.CreatedSim, adoptedChild);
                            EventTracker.SendEvent(EventTypeId.kNewOffspring, spouseDesc.CreatedSim, adoptedChild);
                            EventTracker.SendEvent(EventTypeId.kParentAdded, adoptedChild, spouseDesc.CreatedSim);
                        }
                    }
                }

                EventTracker.SendEvent(EventTypeId.kChildBornOrAdopted, null, adoptedChild);
            }

            return true;
        }
    }
}
