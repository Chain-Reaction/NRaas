using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Status
{
    public class RelationshipStatus : SimFromList, IStatusOption
    {
        static Common.MethodStore sWoohooerGetChanceOfQuads = new Common.MethodStore("NRaasWoohooer", "NRaas.Woohooer", "GetChanceOfQuads", new Type[0]);

        public override string GetTitlePrefix()
        {
            return "RelationshipStatus";
        }

        protected override OptionResult RunResult
        {
            get { return OptionResult.SuccessRetain; }
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(MiniSimDescription me)
        {
            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            return Perform(me);
        }
        protected override bool Run(MiniSimDescription me, bool singleSelection)
        {
            return Perform(me);
        }

        public string GetDetails(IMiniSimDescription me)
        {
            string msg = PersonalStatus.GetHeader(me);

            SimDescription simDesc = me as SimDescription;
            MiniSimDescription miniDesc = me as MiniSimDescription;

            if (simDesc != null)
            {
                if (simDesc.Partner == null)
                {
                    msg += Common.Localize("Status:Single", me.IsFemale);
                }
                else if (simDesc.IsMarried)
                {
                    msg += Common.Localize("Status:Spouse", me.IsFemale, new object[] { simDesc.Partner });
                }
                else
                {
                    msg += Common.Localize("Status:Partner", me.IsFemale, new object[] { simDesc.Partner });
                }
            }
            else if (miniDesc != null)
            {
                if (!miniDesc.HasPartner)
                {
                    msg += Common.Localize("Status:Single", me.IsFemale);
                }
                else
                {
                    IMiniSimDescription partner = SimDescription.Find(miniDesc.PartnerId);
                    if (partner == null)
                    {
                        partner = MiniSimDescription.Find(miniDesc.PartnerId);
                    }

                    if (miniDesc.IsMarried)
                    {
                        if (partner == null)
                        {
                            msg += Common.Localize("Status:Spouse", me.IsFemale, new object[] { "" });
                        }
                        else
                        {
                            msg += Common.Localize("Status:Spouse", me.IsFemale, new object[] { partner });
                        }
                    }
                    else
                    {
                        if (partner == null)
                        {
                            msg += Common.Localize("Status:Partner", me.IsFemale, new object[] { "" });
                        }
                        else
                        {
                            msg += Common.Localize("Status:Partner", me.IsFemale, new object[] { partner });
                        }
                    }
                }
            }

            Genealogy genealogy = null;
            if (simDesc != null)
            {
                genealogy = simDesc.Genealogy;
            }
            else if (miniDesc != null)
            {
                genealogy = miniDesc.Genealogy;
            }

            if (genealogy != null)
            {
                msg += Common.Localize("Status:Children", me.IsFemale, new object[] { genealogy.Children.Count });
            }

            if (me.IsPregnant)
            {
                msg += Common.Localize("Status:Pregnant", me.IsFemale);

                if (simDesc != null)
                {
                    IMiniSimDescription father = SimDescription.Find(simDesc.Pregnancy.DadDescriptionId);
                    if (father == null)
                    {
                        father = MiniSimDescription.Find(simDesc.Pregnancy.DadDescriptionId);
                    }

                    if (father != null)
                    {
                        msg += Common.Localize("Status:BabyFather", me.IsFemale, new object[] { father });
                    }

                    msg += Common.Localize("Status:PregnantProgressed", me.IsFemale, new object[] { simDesc.Pregnancy.mHourOfPregnancy });

                    if (simDesc.Pregnancy.GetCurrentBabyGender() == CASAgeGenderFlags.Male)
                    {
                        msg += Common.Localize("Status:PregnantGenderMale", me.IsFemale);
                    }
                    else if (simDesc.Pregnancy.GetCurrentBabyGender() == CASAgeGenderFlags.Female)
                    {
                        msg += Common.Localize("Status:PregnantGenderFemale", me.IsFemale);
                    }

                    float twinChance = 0;
                    float tripletChance = 0;
                    float quadChance = 0;

                    if (simDesc.IsHuman)
                    {
                        float multipleBabiesMultiplier = Math.Min(simDesc.Pregnancy.mMultipleBabiesMultiplier, Pregnancy.kMaxBabyMultiplier);
                        if (simDesc.TraitManager != null)
                        {
                            if (simDesc.TraitManager.HasElement(TraitNames.WishedForLargeFamily))
                            {
                                multipleBabiesMultiplier = 1000f;
                            }
                            else if (simDesc.TraitManager.HasElement(TraitNames.FertilityTreatment))
                            {
                                multipleBabiesMultiplier *= TraitTuning.kFertilityMultipleBabiesMultiplier;
                            }
                            else if ((simDesc.CreatedSim != null) && (simDesc.CreatedSim.BuffManager != null) && simDesc.CreatedSim.BuffManager.HasElement(BuffNames.ATwinkleInTheEye))
                            {
                                multipleBabiesMultiplier *= TraitTuning.kFertilityMultipleBabiesMultiplier;
                            }
                        }

                        if (multipleBabiesMultiplier != 1000f)
                        {
                            SimDescription simFather = father as SimDescription;
                            if ((simFather != null) && (simFather.TraitManager != null))
                            {
                                if (simFather.TraitManager.HasElement(TraitNames.WishedForLargeFamily))
                                {
                                    multipleBabiesMultiplier = 1000f;
                                }
                                else if (simFather.TraitManager.HasElement(TraitNames.FertilityTreatment))
                                {
                                    multipleBabiesMultiplier *= TraitTuning.kFertilityMultipleBabiesMultiplier;
                                }
                                else if ((simFather.CreatedSim != null) && (simFather.CreatedSim.BuffManager != null) && simFather.CreatedSim.BuffManager.HasElement(BuffNames.ATwinkleInTheEye))
                                {
                                    multipleBabiesMultiplier *= TraitTuning.kFertilityMultipleBabiesMultiplier;
                                }
                            }
                        }

                        if (multipleBabiesMultiplier == 1000f)
                        {
                            twinChance = 1f;
                            tripletChance = 1f;

                            if (sWoohooerGetChanceOfQuads.Valid)
                            {
                                quadChance = 1f;
                            }
                        }
                        else
                        {
                            twinChance = Pregnancy.kChanceOfTwins * multipleBabiesMultiplier;
                            tripletChance = Pregnancy.kChanceOfTriplets * multipleBabiesMultiplier;
                            quadChance = sWoohooerGetChanceOfQuads.Invoke<float>(new object[0]) * multipleBabiesMultiplier;
                        }
                    }
                    else if (!simDesc.IsHorse)
                    {
                        float multiplier = 1f;

                        if (Common.AssemblyCheck.IsInstalled("NRaasWoohooer"))
                        {
                            multiplier = Math.Min(simDesc.Pregnancy.mMultipleBabiesMultiplier, Pregnancy.kMaxBabyMultiplier);

                            if ((simDesc.TraitManager != null) && (simDesc.HasTrait(TraitNames.FertilityTreatmentPet)))
                            {
                                multiplier += TraitTuning.kFertilityLargeLitterMultiplier;
                            }

                            SimDescription simFather = father as SimDescription;
                            if ((simFather != null) && (simFather.TraitManager != null) && (simFather.TraitManager.HasElement(TraitNames.FertilityTreatmentPet)))
                            {
                                multiplier += TraitTuning.kFertilityLargeLitterMultiplier;
                            }
                        }
                        else
                        {
                            if ((simDesc.TraitManager != null) && (simDesc.HasTrait(TraitNames.FertilityTreatmentPet)))
                            {
                                multiplier = TraitTuning.kFertilityLargeLitterMultiplier;
                            }

                            SimDescription simFather = father as SimDescription;
                            if ((simFather != null) && (simFather.TraitManager != null) && (simFather.TraitManager.HasElement(TraitNames.FertilityTreatmentPet)))
                            {
                                multiplier = TraitTuning.kFertilityLargeLitterMultiplier;
                            }
                        }

                        twinChance = PetPregnancy.kChanceOfTwoOffspring * multiplier;
                        tripletChance = PetPregnancy.kChanceOfThreeOffspring * multiplier;
                        quadChance = PetPregnancy.kChanceOfFourOffspring * multiplier;
                    }

                    msg += Common.Localize("Status:PregnancyChance", me.IsFemale, new object[] { (int)(twinChance * 100), (int)(tripletChance * 100), (int)(quadChance * 100) });
                }
            }

            List<IMiniRelationship> relations = new List<IMiniRelationship>();

            if (simDesc != null)
            {
                relations.AddRange(Relationship.GetMiniRelationships(simDesc));
            }
            else if (miniDesc != null)
            {
                foreach (MiniRelationship relation in miniDesc.mMiniRelationships)
                {
                    relations.Add(relation);
                }
            }

            int iRomanticInterests = 0, iFriends = 0, iEnemies = 0, iDislikes = 0;
            foreach (IMiniRelationship relation in relations)
            {
                if (relation == null) continue;

                if (relation.AreRomantic())
                {
                    iRomanticInterests++;
                }
                else if (relation.AreFriends())
                {
                    iFriends++;
                }
                else if ((relation.CurrentLTRLiking < -75) || (relation.AreEnemies()))
                {
                    iEnemies++;
                }
                else if (relation.CurrentLTRLiking < 0)
                {
                    iDislikes++;
                }
            }

            msg += Common.Localize("Status:KnownSims", me.IsFemale, new object[] { relations.Count, iRomanticInterests, iFriends, iDislikes, iEnemies });
            return msg;
        }

        protected bool Perform(IMiniSimDescription me)
        {
            SimpleMessageDialog.Show(Name, GetDetails(me));
            return true;
        }
    }
}
