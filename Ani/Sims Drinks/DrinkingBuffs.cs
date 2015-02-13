using Sims3.SimIFace;
using Sims3.Gameplay.ActorSystems;
using Sims3.UI;
using Sims3.Gameplay.Actors;

namespace Alcohol
{
    class DrinkingBuffs
    {
        public static BuffNames sDrunkBuzzed = (BuffNames)ResourceUtils.HashString64("ani_Buzzed");
        public static BuffNames sDrunkTipsy = (BuffNames)ResourceUtils.HashString64("ani_Tipsy");
        public static BuffNames sDrunkDrunk = (BuffNames)ResourceUtils.HashString64("ani_Drunk");
        public static BuffNames sHangover = (BuffNames)ResourceUtils.HashString64("ani_Hangover");
        public static BuffNames sCounter = (BuffNames)ResourceUtils.HashString64("ani_DrunkCounter"); 

        public static void Add(Sim sim)
        {
            if (sim != null && sim.BuffManager != null)
            {
                BuffNames buzzed = DrinkingBuffs.sDrunkBuzzed;
                BuffNames tipsy = DrinkingBuffs.sDrunkTipsy;
                BuffNames drunk = DrinkingBuffs.sDrunkDrunk;
                BuffNames hangover = DrinkingBuffs.sHangover;
                BuffNames counter = DrinkingBuffs.sCounter;

                Origin origin = Origin.FromJuice;

                DrunkState state = DrunkState.None;

                //The counter
                if (!sim.BuffManager.HasElement(counter))
                    sim.BuffManager.AddElement(counter, origin);
                else
                {
                    sim.BuffManager.RemoveElement(counter);
                    sim.BuffManager.AddElement(counter, origin);
                }

                //Give moodlets
                if (sim.BuffManager.HasElement(buzzed))
                {
                    state = DrunkState.Buzzed;
                }
                else if (sim.BuffManager.HasElement(tipsy))
                {
                    state = DrunkState.Tipsy;
                }
                else if (sim.BuffManager.HasElement(drunk))
                {
                    state = DrunkState.Drunk;
                }else if(sim.BuffManager.HasElement(hangover))
                {
                    if (sim.BuffManager.GetElement(hangover).BuffOrigin == origin)
                        state = DrunkState.Hangover;
                }
                               
                switch (state)
                {
                    case DrunkState.None:

                        sim.BuffManager.AddElement(buzzed, origin);
                        break;
                    case DrunkState.Buzzed:
                        sim.BuffManager.RemoveElement(buzzed);
                        sim.BuffManager.AddElement(tipsy, origin);
                        break;
                    case DrunkState.Tipsy:
                        sim.BuffManager.RemoveElement(tipsy);
                        sim.BuffManager.AddElement(drunk, origin);
                        break;
                    case DrunkState.Drunk:
                        sim.BuffManager.RemoveElement(drunk);
                        sim.BuffManager.AddElement(drunk, origin);
                        break;
                    case DrunkState.Hangover:
                        sim.BuffManager.RemoveElement(hangover);
                        sim.BuffManager.AddElement(drunk, origin);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
