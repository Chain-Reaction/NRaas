using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.SimIFace.CAS;

namespace NRaas.CommonSpace.Helpers
{
    public class OccultGenieEx
    {
        public static void OnFreedFromLamp(OccultGenie ths, Sim freer, Sim genie, bool addToFreerHousehold)
        {
            genie.SimDescription.MotivesDontDecay = false;
            genie.SimDescription.Marryable = true;
            genie.SimDescription.AgingEnabled = true;
            ths.mLamp = null;
            if (addToFreerHousehold /*&& freer.Household.CanAddSpeciesToHousehold(CASAgeGenderFlags.Human)*/)
            {
                freer.Household.AddSim(genie);
                freer.Household.AddOutfitToWardrobe(genie.CurrentOutfit.Key);
            }
            EventTracker.SendEvent(EventTypeId.kFreeGenie, freer);
        }

        public static void OnRemoval(OccultGenie ths, SimDescription simDes, bool alterOutfit)
        {
            if (ths.mGenieMagicPoints != null)
            {
                ths.mGenieMagicPoints.RestorePoints();
            }

            OutfitCategories everyday = OutfitCategories.Everyday;
            int index = 0x0;
            Sim createdSim = simDes.CreatedSim;
            if (createdSim != null)
            {
                createdSim.SetOpacity(1f, 0f);
                createdSim.Motives.RestoreDecays();
                everyday = createdSim.CurrentOutfitCategory;
                index = createdSim.CurrentOutfitIndex;
            }

            simDes.MotivesDontDecay = false;
            simDes.Marryable = true;
            simDes.AgingEnabled = true;

            if (simDes.TraitManager.HasElement(TraitNames.ImmuneToFire))
            {
                simDes.TraitManager.RemoveElement(TraitNames.ImmuneToFire);
            }
            if (simDes.TraitManager.HasElement(TraitNames.GenieHiddenTrait))
            {
                simDes.TraitManager.RemoveElement(TraitNames.GenieHiddenTrait);
            }

            if (alterOutfit)
            {
                simDes.SetSkinToneForAllOutfits(simDes.Age, CASSkinTones.NoSkinTone | CASSkinTones.HumanSkinTone, ths.mOldSkinToneIndex);
                if (createdSim != null)
                {
                    Sim.SwitchOutfitHelper helper = new Sim.SwitchOutfitHelper(createdSim, everyday, index);
                    helper.Start();
                    helper.Wait(false);
                    helper.ChangeOutfit();
                    helper.Dispose();
                }
            }
        }
    }
}
