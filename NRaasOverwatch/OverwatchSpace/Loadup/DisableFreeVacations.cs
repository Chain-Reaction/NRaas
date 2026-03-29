using Sims3.Gameplay.Situations;

namespace NRaas.OverwatchSpace.Loadup
{
    public class DisableFreeVacations : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            ParentsLeavingTownSituation.kChanceForCheckingForParentsLeaving = 0;
        }
    }
}
