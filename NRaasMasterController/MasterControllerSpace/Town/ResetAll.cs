using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Town
{
    public class ResetAll : OptionItem, ITownOption
    {
        public override string GetTitlePrefix()
        {
            return "ResetAll";
        }

        protected static bool IsPartOfActiveLot(IGameObject obj, Lot activeLot)
        {
            if (MasterController.Settings.mResetEverythingOnActive) return false;

            if (obj.LotCurrent == activeLot) return true;

            Sim sim = obj as Sim;
            if (sim != null)
            {
                if (sim.LotHome == activeLot) return true;
            }

            return false;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (AcceptCancelDialog.Show(Common.Localize("ResetAll:Prompt")))
            {
                GameflowEx.Pause();

                Lot activeLot = null;
                if (Household.ActiveHousehold != null)
                {
                    activeLot = Household.ActiveHousehold.LotHome;
                }

                Dictionary<IGameObject, bool> objs = new Dictionary<IGameObject, bool>();

                try
                {
                    ProgressDialog.Show(Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/Global:Processing", new object[0x0]), false);

                    List<IGameObject> local = new List<IGameObject>(Sims3.Gameplay.Queries.GetObjects<IGameObject>());
                    foreach (IGameObject obj in local)
                    {
                        if (IsPartOfActiveLot(obj, activeLot)) continue;

                        if (Households.Reset.ResetObject(obj, true))
                        {
                            objs[obj] = true;
                        }
                    }

                    List<IGameObject> global = new List<IGameObject>(Sims3.Gameplay.Queries.GetGlobalObjects<IGameObject>());
                    foreach (IGameObject obj in global)
                    {
                        if (IsPartOfActiveLot(obj, activeLot)) continue;

                        if (Households.Reset.ResetObject(obj, true))
                        {
                            objs[obj] = true;
                        }
                    }

                    foreach (Lot lot in LotManager.AllLots)
                    {
                        if (!MasterController.Settings.mResetEverythingOnActive)
                        {
                            if (lot == activeLot) continue;
                        }

                        Households.Reset.ResetLot(lot, true);
                        objs[lot] = true;
                    }

                    foreach (Situation situation in new List<Situation>(Situation.sAllSituations))
                    {
                        if (!MasterController.Settings.mResetEverythingOnActive)
                        {
                            if (situation.Lot == activeLot) continue;
                        }

                        // Ignore the Butler situation, as exiting it will end the butler service
                        if (situation is ButlerSituation) continue;

                        try
                        {
                            Common.DebugNotify(situation.GetType().ToString());

                            situation.Exit();
                        }
                        catch (Exception exception)
                        {
                            Common.Exception(situation.Lot, exception);
                        }
                    }
                }
                finally
                {
                    ProgressDialog.Close();
                }

                SimpleMessageDialog.Show(Name, Common.Localize("ResetAll:Result", false, new object[] { objs.Count }));
            }
            return OptionResult.SuccessClose;
        }
    }
}
