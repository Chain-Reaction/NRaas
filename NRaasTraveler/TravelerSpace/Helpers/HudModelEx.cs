using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.Helpers
{
    public class HudModelEx
    {
        static CommodityKind[] kMotives = new CommodityKind[] { CommodityKind.Hunger, CommodityKind.Bladder, CommodityKind.Energy, CommodityKind.Social, CommodityKind.Hygiene, CommodityKind.Fun };
        static CommodityKind[] kVampireMotives = new CommodityKind[] { CommodityKind.VampireThirst, CommodityKind.Bladder, CommodityKind.Energy, CommodityKind.Social, CommodityKind.Hygiene, CommodityKind.Fun };
        static CommodityKind[] kAlienMotives = new CommodityKind[] { CommodityKind.Hunger, CommodityKind.Bladder, CommodityKind.AlienBrainPower, CommodityKind.Social, CommodityKind.Hygiene, CommodityKind.Fun };
        static CommodityKind[] kDogMotives = new CommodityKind[] { CommodityKind.Hunger, CommodityKind.Bladder, CommodityKind.Energy, CommodityKind.Social, CommodityKind.DogDestruction, CommodityKind.Fun };
        static CommodityKind[] kCatMotives = new CommodityKind[] { CommodityKind.Hunger, CommodityKind.Bladder, CommodityKind.Energy, CommodityKind.Social, CommodityKind.CatScratch, CommodityKind.Fun };
        static CommodityKind[] kHorseMotives = new CommodityKind[] { CommodityKind.Hunger, CommodityKind.Bladder, CommodityKind.Energy, CommodityKind.Social, CommodityKind.HorseExercise, CommodityKind.HorseThirst };

        public static CommodityKind GetMotive(Sim sim, uint motive)
        {
            return GetMotives(sim)[motive];
        }

        public static CommodityKind[] GetMotives(Sim sim)
        {
            if (sim != null)
            {
                if (sim.IsPet)
                {
                    if (sim.IsADogSpecies)
                    {
                        return kDogMotives;
                    }

                    if (sim.IsCat)
                    {
                        return kCatMotives;
                    }

                    if (sim.IsHorse)
                    {
                        return kHorseMotives;
                    }
                }

                if (sim.SimDescription.YoungAdultOrAbove && sim.SimDescription.IsVampire)
                {
                    return kVampireMotives;
                }

                if (sim.SimDescription.IsAlienEvolved)
                {
                    return kAlienMotives;
                }
            }
            return kMotives;
        }
    }
}
