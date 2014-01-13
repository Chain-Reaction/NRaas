using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public class Traits
    {
        public static string ProperName(TraitNames value, bool isFemale)
        {
            string name = null;

            Trait trait = TraitManager.GetTraitFromDictionary(value);
            if (trait != null)
            {
                name = trait.TraitName(isFemale);

                if (trait.IsHidden)
                {
                    string result;
                    if (Common.Localize("HiddenTrait:" + value, isFemale, new object[0], out result))
                    {
                        name = result;
                    }
                }
            }

            name += (Common.kDebugging ? " (" + value + ")" : "");

            return name;
        }

        public static bool IsObjectBaseReward(TraitNames trait)
        {
            switch (trait)
            {
                case TraitNames.MapToTheStars:
                case TraitNames.Teleporter:
                case TraitNames.CollectionHelper:
                case TraitNames.FoodReplicator:
                case TraitNames.BodySculptor:
                case TraitNames.MoodModifier:
                case TraitNames.CloneMe:
                case TraitNames.CloneMePet:
                case TraitNames.HoverBed:
                case TraitNames.Inheritance:
                case TraitNames.MotiveMobile:
                case TraitNames.YoungAgain:
                case TraitNames.YoungAgainPet:
                case TraitNames.ChangeLifetimeWish:
                case TraitNames.ChangeOfTaste:
                case TraitNames.BottomlessPetBowlPet:
                case TraitNames.BoxStallSuperPet:
                case TraitNames.PetHygienatorPet:
                case TraitNames.PetBedSuperPet:
                case TraitNames.SelfCleaningStallPet:
                case TraitNames.GenieLamp:
                case TraitNames.Aromainator:
                case TraitNames.FlyingVacuum:
                case TraitNames.PhilosophersStone:
                case TraitNames.CleanSlate:
                case TraitNames.WitchMagicHands:
                case TraitNames.MyBestFriend:
                case TraitNames.MidLifeCrisis:
                case TraitNames.MidlifeCrisisPet:
                case TraitNames.CloudRayGun:
                case TraitNames.WeatherMachine:
                case TraitNames.RebelInfluence:
                case TraitNames.SocialiteInfluence:
                case TraitNames.NerdInfluence:
                case TraitNames.SocialGroupTrait:
                case TraitNames.HonoraryDegree:
                case TraitNames.MermadicKelp:
                case TraitNames.HiddenIslandMap:
                case TraitNames.DreamPodHiddenTrait:
                    return true;
            }
            return false;
        }
    }
}

