using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class MaleHaveBabyHomeEx : AlienUtils.MaleHaveBabyHome, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, AlienUtils.MaleHaveBabyHome.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, AlienUtils.MaleHaveBabyHome.Definition, Definition>(false);

            sOldSingleton = MaleHaveBabyHomeSingleton;
            MaleHaveBabyHomeSingleton = new Definition();
        }

        public override void GetNewBorns()
        {
            mNewborns = new List<Sim>();
            Sims3.Gameplay.Gameflow.Singleton.DisableSave(this, "Gameplay/ActorSystems/Pregnancy:DisableSave");
            bool keepBaby = false;

            //if (Actor.Household.NumActorMembersCountingPregnancy < 0x8)
            {
                // Custom
                if (SimTypes.IsSelectable(Actor))
                {
                    keepBaby = TwoButtonDialog.Show(AlienUtils.LocalizeString("MalePregnancyConfirmationDialog", new object[] { Actor }), AlienUtils.LocalizeString("MalePregnancyConfirmationDialogAccept", new object[0x0]), AlienUtils.LocalizeString("MalePregnancyConfirmationDialogReject", new object[0x0]));
                }
                else
                {
                    keepBaby = true;
                }
            }
            /*else
            {
                Actor.ShowTNSIfSelectable(AlienUtils.LocalizeString("MalePregnancyHouseholdFullTNS", new object[] { Actor }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, Actor.ObjectId);
            }*/

            CASAgeGenderFlags gender = RandomUtil.CoinFlip() ? CASAgeGenderFlags.Male : CASAgeGenderFlags.Female;
            SimDescription description2 = SimDescription.Find(this.AlienParentID);
            SimDescription newSim = Genetics.MakeAlien(CASAgeGenderFlags.Baby, gender, GameUtils.GetCurrentWorld(), 1f, false);
            newSim.LastName = Actor.LastName;
            if ((keepBaby) && (SimTypes.IsSelectable(Actor)))
            {
                newSim.FirstName = string.Empty;
            }

            Genetics.AssignTraits(newSim, null, Actor.SimDescription, (keepBaby) && (SimTypes.IsSelectable(Actor)), (float)Actor.MoodManager.MoodValue, new Random());
            Actor.Genealogy.AddChild(newSim.Genealogy);
            if (description2 != null)
            {
                description2.Genealogy.AddChild(newSim.Genealogy);
            }

            if (keepBaby)
            {
                Actor.Household.Add(newSim);
            }
            else
            {
                Household.AlienHousehold.Add(newSim);
            }

            Sim babyToHide = newSim.Instantiate(Vector3.Empty);
            babyToHide.GreetSimOnLot(Actor.LotCurrent);
            babyToHide.SetPosition(Actor.Position);
            Pregnancy.TotallyHideBaby(babyToHide);
            mNewborns.Add(babyToHide);
        }

        public override void Cleanup()
        {
            try
            {
                base.Cleanup();

                // EA is not calling the Pregnancy:PregnancyComplete() function for Alien pregnancies, do so now
                if ((mNewborns != null) && (mNewborns.Count > 0))
                {
                    Pregnancy pregnancy = Actor.SimDescription.Pregnancy;

                    try
                    {
                        Actor.SimDescription.Pregnancy = null;

                        Pregnancy fakePregnancy = new Pregnancy(Actor, null);

                        fakePregnancy.PregnancyComplete(mNewborns, null);
                    }
                    finally
                    {
                        Actor.SimDescription.Pregnancy = pregnancy;
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
        }

        public override bool Run()
        {
            return HaveBabyHomeEx.Run(this);
        }

        public new class Definition : AlienUtils.MaleHaveBabyHome.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new MaleHaveBabyHomeEx();
                result.Init(ref parameters);
                return result;
            }

            public override string GetInteractionName(Sim actor, Lot target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
