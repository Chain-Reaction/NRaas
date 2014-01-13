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

namespace NRaas.WoohooerSpace.Proxies
{
    public class PetPregnancyProxy : PetPregnancy
    {
        public PetPregnancyProxy(Pregnancy pregnancy)
        {
            PregnancyProxy.CopyPregnancy(this, pregnancy);
        }

        public override int GetNumForBirth(SimDescription dadDescription, Random pregoRandom, int numSimMembers, int numPetMembers)
        {
            try
            {
                int desiredNumChilderen = 0x1;
                SimDescription simDescription = Mom.SimDescription;
                if (simDescription.Species != CASAgeGenderFlags.Horse)
                {
                    double num2 = pregoRandom.NextDouble();
                    float multiplier = Math.Min(mMultipleBabiesMultiplier, kMaxBabyMultiplier);

                    if (mMom.HasTrait(TraitNames.FertilityTreatmentPet))
                    {
                        multiplier += TraitTuning.kFertilityLargeLitterMultiplier;
                    }

                    if ((dadDescription != null) && dadDescription.HasTrait(TraitNames.FertilityTreatmentPet))
                    {
                        multiplier += TraitTuning.kFertilityLargeLitterMultiplier;
                    }

                    if (num2 < (kChanceOfTwoOffspring * multiplier))
                    {
                        desiredNumChilderen++;
                        if (num2 < (kChanceOfThreeOffspring * multiplier))
                        {
                            desiredNumChilderen++;
                            if (num2 < (kChanceOfFourOffspring * multiplier))
                            {
                                desiredNumChilderen++;
                            }
                        }
                    }
                }

                return desiredNumChilderen;

                // Greater than Eight check
                //return Household.GetAllowableNumChildren(simDescription, desiredNumChilderen, numSimMembers, numPetMembers);
            }
            catch (Exception e)
            {
                Common.Exception(Mom.SimDescription, dadDescription, e);
                return 1;
            }
        }
    }
}
