using Sims3.SimIFace;
using Sims3.UI;

namespace NRaas.OverwatchSpace.Loadup
{
    public class FPSCheck : Common.IDelayedWorldLoadFinished
    {
        public void OnDelayedWorldLoadFinished()
        {
            if (GameUtils.GetCurrentFps() >= Overwatch.Settings.mFPSCap)
            {
                SimpleMessageDialog.Show(Common.Localize("FramerateWarning:MessageTitle"), Common.Localize("FramerateWarning:MessageBody"), ModalDialog.PauseMode.PauseTask);
            }
        }
    }
}
