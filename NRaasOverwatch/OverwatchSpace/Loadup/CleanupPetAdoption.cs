using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Utilities;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupPetAdoption : DelayedLoadupOption
    {
        public CleanupPetAdoption()
        { }

        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupPetAdoption");

            bool reset = Overwatch.Settings.mStopPetAdoption;

            if (PetAdoptions.Cleanup(Overwatch.Log))
            {
                reset = true;
            }
   
            if (reset)
            {
                PetAdoptions.Stop(Overwatch.Log);
            }
        }
    }
}
