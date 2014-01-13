using NRaas.StoryProgressionSpace.Helpers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class StoryProgressionModule
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        static StoryProgressionModule()
        { }

        public StoryProgressionModule()
        { }

        // Externalized to Mover
        public static int GetLotCost(Lot lot)
        {
            try
            {
                if (StoryProgression.Main != null)
                {
                    return StoryProgression.Main.Lots.GetLotCost(lot);
                }
            }
            catch (Exception e)
            {
                Common.Exception(lot, e);
            }

            return lot.Cost;
        }

        // Externalized to Mover
        public static bool PresetRentalLotHome(Lot lot, Household house)
        {
            try
            {
                if (StoryProgression.Main != null)
                {
                    RentalHelper.SetLotHome(lot, house);
                }
            }
            catch (Exception e)
            {
                Common.Exception(lot, e);
            }

            return true;
        }
    }
}
