using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    /*
     * Pets
     * 
     * AggressivePet
     * CluelessPet
     * FriendlyPet
     * GeniusPet
     * LazyPet
     * NeatPet
     * NoisyPet
     * PiggyPet
     * PlayfulPet
     * QuietPet
     * ShyPet
     * 
     * Cat
     * 
     * AdventurousPet
     * DestructivePet
     * HunterPet
     * HyperPet
     * IndependentPet
     * NonDestructivePet
     * ProudPet
     * SkittishPet
     * 
     * Dog
     * 
     * AdventurousPet
     * DestructivePet
     * HunterPet
     * HydrophobicPet
     * HyperPet
     * IndependentPet
     * LikesSwimmingPet
     * LoyalPet
     * NonDestructivePet
     * ProudPet
     * SkittishPet
     * 
     * Horse
     * 
     * AgilePet
     * BravePet
     * FastPet
     * HatesJumpingPet
     * NervousPet
     * ObedientPet
     * OrneryPet
     * UntrainedPet
     * 
     * */

    public abstract class TraitBaseScoring<SP> : HitMissScoring<SimDescription, SP, SP>
        where SP : SimScoringParameters
    {
        static Dictionary<TraitNames, TraitNames[]> sPetEquivalents = new Dictionary<TraitNames, TraitNames[]>();

        static Dictionary<TraitNames, TraitNames> sPlumbotEquivalents = new Dictionary<TraitNames, TraitNames>();
        static Dictionary<TraitNames, TraitNames> sPlumbotAntiEquivalents = new Dictionary<TraitNames, TraitNames>();

        TraitNames[] mTraits = null;

        TraitNames mAntiTrait = TraitNames.Unknown;

        static TraitBaseScoring()
        {
            sPlumbotEquivalents.Add(TraitNames.MeanSpirited, TraitNames.EvilChip);
            sPlumbotEquivalents.Add(TraitNames.Evil, TraitNames.EvilChip);
            sPlumbotEquivalents.Add(TraitNames.Friendly, TraitNames.FriendlyChip);
            sPlumbotEquivalents.Add(TraitNames.Artistic, TraitNames.ArtisticAlgorithmsChip);
            sPlumbotEquivalents.Add(TraitNames.NaturalCook, TraitNames.ChefChip);
            sPlumbotEquivalents.Add(TraitNames.BornToCook, TraitNames.ChefChip);
            sPlumbotEquivalents.Add(TraitNames.Neat, TraitNames.CleanerChip);
            sPlumbotEquivalents.Add(TraitNames.SuperNanny, TraitNames.RoboNannyChip);
            sPlumbotEquivalents.Add(TraitNames.FamilyOriented, TraitNames.RoboNannyChip);
            sPlumbotEquivalents.Add(TraitNames.Angler, TraitNames.FisherBotChip);
            sPlumbotEquivalents.Add(TraitNames.GreenThumb, TraitNames.GardenerChip);
            sPlumbotEquivalents.Add(TraitNames.GathererTrait, TraitNames.GardenerChip);
            sPlumbotEquivalents.Add(TraitNames.SuperGreenThumb, TraitNames.GardenerChip);
            sPlumbotEquivalents.Add(TraitNames.Rocker, TraitNames.MusicianChip);
            sPlumbotEquivalents.Add(TraitNames.Handy, TraitNames.HandiBotChip);
            sPlumbotEquivalents.Add(TraitNames.Flirty, TraitNames.CapacityToLoveChip);
            sPlumbotEquivalents.Add(TraitNames.Irresistible, TraitNames.CapacityToLoveChip);
            sPlumbotEquivalents.Add(TraitNames.GoodSenseOfHumor, TraitNames.HumorChip);
            sPlumbotEquivalents.Add(TraitNames.Ambitious, TraitNames.ProfessionalChip);
            sPlumbotEquivalents.Add(TraitNames.Vegetarian, TraitNames.SolarPoweredChip);
            sPlumbotEquivalents.Add(TraitNames.Genius, TraitNames.AbilityToLearnChip);
            sPlumbotEquivalents.Add(TraitNames.ComputerWhiz, TraitNames.RobotHiddenTrait);
            sPlumbotEquivalents.Add(TraitNames.Perfectionist, TraitNames.RobotHiddenTrait);
            sPlumbotEquivalents.Add(TraitNames.Coward, TraitNames.FearOfHumansChip);
            sPlumbotEquivalents.Add(TraitNames.Charismatic, TraitNames.HumanEmotionChip);
            sPlumbotEquivalents.Add(TraitNames.Childish, TraitNames.HumanEmotionChip);
            sPlumbotEquivalents.Add(TraitNames.Dramatic, TraitNames.HumanEmotionChip);
            sPlumbotEquivalents.Add(TraitNames.Neurotic, TraitNames.HumanEmotionChip);
            sPlumbotEquivalents.Add(TraitNames.HotHeaded, TraitNames.HumanEmotionChip);
            sPlumbotEquivalents.Add(TraitNames.OverEmotional, TraitNames.HumanEmotionChip);
            sPlumbotEquivalents.Add(TraitNames.Rebellious, TraitNames.SentienceChip);
            sPlumbotEquivalents.Add(TraitNames.Schmoozer, TraitNames.ProfessionalChip);
            sPlumbotEquivalents.Add(TraitNames.Virtuoso, TraitNames.MusicianChip);
            sPlumbotEquivalents.Add(TraitNames.Workaholic, TraitNames.EfficientChip);

            sPlumbotAntiEquivalents.Add(TraitNames.Slob, TraitNames.CleanerChip);
            sPlumbotAntiEquivalents.Add(TraitNames.Proper, TraitNames.SentienceChip);
            sPlumbotAntiEquivalents.Add(TraitNames.EasilyImpressed, TraitNames.SentienceChip);
            sPlumbotAntiEquivalents.Add(TraitNames.Brave, TraitNames.FearOfHumansChip);
            sPlumbotAntiEquivalents.Add(TraitNames.Good, TraitNames.EvilChip);
            sPlumbotAntiEquivalents.Add(TraitNames.NoSenseOfHumor, TraitNames.HumorChip);
            sPlumbotAntiEquivalents.Add(TraitNames.Unflirty, TraitNames.CapacityToLoveChip);
            sPlumbotAntiEquivalents.Add(TraitNames.NoJealousy, TraitNames.CapacityToLoveChip);
            sPlumbotAntiEquivalents.Add(TraitNames.Carefree, TraitNames.SentienceChip);

            sPetEquivalents.Add(TraitNames.MeanSpirited, new TraitNames[] { TraitNames.AggressivePet, TraitNames.MeanPet });
            sPetEquivalents.Add(TraitNames.AbsentMinded, new TraitNames[] { TraitNames.CluelessPet });
            sPetEquivalents.Add(TraitNames.Adventurous, new TraitNames[] { TraitNames.AdventurousPet });
            sPetEquivalents.Add(TraitNames.Athletic, new TraitNames[] { TraitNames.AgilePet, TraitNames.FastPet });
            sPetEquivalents.Add(TraitNames.Brave, new TraitNames[] { TraitNames.BravePet, TraitNames.FearlessFoalPet });
            sPetEquivalents.Add(TraitNames.Charismatic, new TraitNames[] { TraitNames.AlphaPet, TraitNames.FriendOfTheHerdPet });
            sPetEquivalents.Add(TraitNames.Childish, new TraitNames[] { TraitNames.PlayfulPet });
            sPetEquivalents.Add(TraitNames.CouchPotato, new TraitNames[] { TraitNames.LazyPet, TraitNames.LazyHorse });
            sPetEquivalents.Add(TraitNames.Clumsy, new TraitNames[] { TraitNames.HatesJumpingPet });
            sPetEquivalents.Add(TraitNames.Coward, new TraitNames[] { TraitNames.SkittishPet, TraitNames.NervousPet });
            sPetEquivalents.Add(TraitNames.Excitable, new TraitNames[] { TraitNames.HyperPet });
            sPetEquivalents.Add(TraitNames.FertilityTreatment, new TraitNames[] { TraitNames.FertilityTreatmentPet });
            sPetEquivalents.Add(TraitNames.Friendly, new TraitNames[] { TraitNames.FriendlyPet });
            sPetEquivalents.Add(TraitNames.Genius, new TraitNames[] { TraitNames.GeniusPet, TraitNames.SuperSmartPet });
            sPetEquivalents.Add(TraitNames.Good, new TraitNames[] { TraitNames.LoyalPet, TraitNames.GentlePet });
            sPetEquivalents.Add(TraitNames.Hydrophobic, new TraitNames[] { TraitNames.HydrophobicPet });
            sPetEquivalents.Add(TraitNames.Lucky, new TraitNames[] { TraitNames.LuckyMountPet });
            sPetEquivalents.Add(TraitNames.Loner, new TraitNames[] { TraitNames.IndependentPet });
            sPetEquivalents.Add(TraitNames.Neat, new TraitNames[] { TraitNames.NeatPet, TraitNames.NonDestructivePet });
            sPetEquivalents.Add(TraitNames.Shy, new TraitNames[] { TraitNames.ShyPet });
            sPetEquivalents.Add(TraitNames.Slob, new TraitNames[] { TraitNames.PiggyPet, TraitNames.DestructivePet });
            sPetEquivalents.Add(TraitNames.Snob, new TraitNames[] { TraitNames.ProudPet });
            sPetEquivalents.Add(TraitNames.SteelBladder, new TraitNames[] { TraitNames.SteelBladderPet });
        }

        public TraitBaseScoring()
        { }

        public override string ToString()
        {
            string traits = null;

            foreach (TraitNames trait in mTraits)
            {
                traits += "," + trait;
            }

            return base.ToString() + traits;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!base.Parse(row, ref error)) return false;

            if (!row.Exists("Trait"))
            {
                error = "Trait missing";
                return false;
            }
            else
            {
                ToTraitName converter = new ToTraitName();

                List<TraitNames> traits = converter.Convert(row.GetString("Trait"));
                if ((traits == null) || (traits.Count == 0))
                {
                    error = converter.mError;
                    return false;
                }

                if (!sPlumbotAntiEquivalents.TryGetValue(traits[0], out mAntiTrait))
                {
                    mAntiTrait = TraitNames.Unknown;
                }

                mTraits = traits.ToArray();
            }

            return true;
        }

        public override bool IsHit(SP parameters)
        {
            if ((parameters.Actor.IsEP11Bot) && (mAntiTrait != TraitNames.Unknown))
            {
                if (!parameters.Actor.TraitManager.HasElement(mAntiTrait))
                {
                    return true;
                }
            }

            return parameters.Actor.TraitManager.HasAnyElement(mTraits);
        }

        public class ToTraitName : StringToMultiList<TraitNames>
        {
            public string mError;

            protected override List<TraitNames> PrivateConvert(string value)
            {
                TraitNames trait;
                if (!ParserFunctions.TryParseEnum<TraitNames>(value, out trait, TraitNames.Unknown))
                {
                    mError = "Unknown Trait " + value;
                    return null;
                }

                List<TraitNames> results = new List<TraitNames>();
                results.Add(trait);

                TraitNames[] petTraits = null;
                if (sPetEquivalents.TryGetValue(trait, out petTraits))
                {
                    results.AddRange(petTraits);
                }

                TraitNames robotTrait = TraitNames.Unknown;
                if (sPlumbotEquivalents.TryGetValue(trait, out robotTrait))
                {
                    results.Add(robotTrait);
                }

                return results;
            }
        }

        public class Delegate
        {
            TraitManager mManager = null;

            public Delegate(SimDescription sim)
            {
                mManager = sim.TraitManager;
            }

            public int Score(TraitBaseScoring<SP> scoring, SP parameters)
            {
                bool success = false;

                if ((mManager.mSimDescription.IsEP11Bot) && (scoring.mAntiTrait != TraitNames.Unknown))
                {
                    if (!parameters.Actor.TraitManager.HasElement(scoring.mAntiTrait))
                    {
                        success = true;
                    }
                }

                if (!success)
                {
                    success = mManager.HasAnyElement(scoring.mTraits);
                }

                return scoring.Score(success, parameters);
            }
        }
    }
}

