using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class CommoditiesEx
    {
        public static string GetMotiveLocalizedName(CommodityKind kind)
        {
            switch (kind)
            {
                case CommodityKind.Hunger:
                case CommodityKind.Energy:
                case CommodityKind.Hygiene:
                case CommodityKind.Fun:
                case CommodityKind.Social:
                case CommodityKind.Bladder:
                case CommodityKind.AlienBrainPower:
                    return Common.LocalizeEAString("Ui/Caption/HUD/MotivesPanel:Motive" + kind);
                case CommodityKind.DogDestruction:
                    return Common.LocalizeEAString("Ui/Caption/HUD/MotivesPanel:MotiveDestructionDog");
                case CommodityKind.CatScratch:
                    return Common.LocalizeEAString("Ui/Caption/HUD/MotivesPanel:MotiveScratchCat");
                case CommodityKind.HorseThirst:
                    return Common.Localize("Species:Horse") + " " + Common.LocalizeEAString("Ui/Caption/HUD/MotivesPanel:MotiveThirstHorse");
                case CommodityKind.HorseExercise:
                    return Common.LocalizeEAString("Ui/Caption/HUD/MotivesPanel:MotiveExerciseHorse");
                case CommodityKind.VampireThirst:
                    return OccultManager.GetSingularOccultName(OccultTypesAll.Vampire) + " " + Common.LocalizeEAString("Ui/Caption/HUD/MotivesPanel:MotiveHungerVampire");
            }

            string result;
            if (Common.Localize("Commodity:" + kind.ToString(), false, new object[0], out result))
            {
                return result;
            }
            else
            {
                return kind.ToString();
            }
        }
    }
}

