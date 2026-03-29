using Sims3.Gameplay;
using Sims3.SimIFace;
using Sims3.UI;

namespace NRaas.OverwatchSpace.Startup
{
    public class GPUCheck : StartupOption
    {
        public GPUCheck()
        { }

        public override void OnStartupApp()
        {
            if (!GameStates.IsInMainMenuState)
            {
                Common.Sleep();
            }

            if(!DeviceConfig.IsCardRecognized())
            {
                SimpleMessageDialog.Show(Common.Localize("GPUWarning:MessageTitle"), Common.Localize("GPUWarning:MessageBody"), ModalDialog.PauseMode.PauseTask);
            }
        }
    }
}
