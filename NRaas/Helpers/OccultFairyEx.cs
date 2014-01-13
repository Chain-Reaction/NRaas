using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Core;
using Sims3.SimIFace;
using System;

namespace NRaas.CommonSpace.Helpers
{
    public class OccultFairyEx
    {
        public static void ShowHumanAndHideTrueForm(OccultFairy ths)
        {
            string msg = null;

            try
            {
                SimRoutingComponent simRoutingComponent = ths.mOwningSim.SimRoutingComponent;
                if (simRoutingComponent != null)
                {
                    simRoutingComponent.EnableDynamicFootprint();
                }
                ths.mOwningSim.SetHiddenFlags(HiddenFlags.Nothing);

                msg += "A";

                // Custom
                if (ths.mWings != null)
                {
                    ths.mWings.Start();
                }
                else
                {
                    ths.GrantWings();
                }

                msg += "B";

                if (ths.mOwningSim.TraitManager.HasElement(TraitNames.FairyQueen))
                {
                    ths.AddHoveringFairies();
                }

                msg += "C";

                if (ths.mOwningSim.IsActiveSim)
                {
                    PlumbBob.ShowPlumbBob();
                }

                msg += "D";

                if (ths.mVfxTrueFairyForm != null)
                {
                    ths.mVfxTrueFairyForm.Stop();
                }

                msg += "E";

                ths.CleanupFairyTrueFormFx();
            }
            catch (Exception e)
            {
                Common.Exception(ths.mOwningSim, null, msg, e);
            }
        }
    }
}
