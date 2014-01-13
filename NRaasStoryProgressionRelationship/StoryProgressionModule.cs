using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.CAS;
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
        static StoryProgressionModule()
        { }

        public StoryProgressionModule()
        { }

        // Externalized to Woohooer
        public static string AllowAffair(SimDescription actor, SimDescription target, bool autonomous)
        {
            try
            {
                if (StoryProgression.Main == null) return null;

                if (!ManagerRomance.IsAffair(actor, target)) return null;

                if (actor.Partner != null)
                {
                    if (StoryProgression.Main.GetValue<ChanceOfAdulteryOption, int>(actor) == 0)
                    {
                        return "ChanceOfAdultery";
                    }

                    if (StoryProgression.Main.GetValue<ChanceOfLiaisonOption, int>(target) == 0)
                    {
                        return "ChanceOfLiaison";
                    }
                }

                if (target.Partner != null)
                {
                    if (StoryProgression.Main.GetValue<ChanceOfAdulteryOption, int>(target) == 0)
                    {
                        return "ChanceOfAdultery";
                    }

                    if (StoryProgression.Main.GetValue<ChanceOfLiaisonOption, int>(actor) == 0)
                    {
                        return "ChanceOfLiaison";
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return null;
            }
        }
    }
}
