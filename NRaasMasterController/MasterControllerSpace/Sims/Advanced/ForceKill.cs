using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced
{
    public class ForceKill : SimFromList, IAdvancedOption
    {
        SimDescription.DeathType mDeathType = SimDescription.DeathType.OldAge;

        public override string GetTitlePrefix()
        {
            return "ForceKill";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                if (!AcceptCancelDialog.Show(Common.Localize("ForceKill:Prompt", me.IsFemale, new object[] { me })))
                {
                    return false;
                }

                Occult.Item choice = new CommonSelection<Occult.Item>(Name, me.FullName, Occult.GetDeathOptions()).SelectSingle();
                if (choice == null) return false;

                mDeathType = choice.Value.mDeathType;
            }

            return Perform(me, mDeathType);
        }

        public static bool Perform(SimDescription me, SimDescription.DeathType deathType)
        {
            if (me == null) return false;

            Sim createdSim = me.CreatedSim;
            if (createdSim == PlumbBob.SelectedActor)
            {
                IntroTutorial.ForceExitTutorial();
                LotManager.SelectNextSim();
            }

            if (createdSim != null)
            {
                createdSim = Households.Reset.ResetSim(createdSim, false);

                if ((createdSim != null) && (createdSim.BuffManager != null))
                {
                    createdSim.BuffManager.RemoveAllElements();
                }
            }

            Urnstone urnstone = Urnstones.CreateGrave(me, deathType, true, true);
            if (urnstone == null)
            {
                SimpleMessageDialog.Show(Common.Localize("ForceKill:MenuName"), Common.Localize("ForceKill:Error"));
                return false;
            }

            if (createdSim != null)
            {
                if ((createdSim.Autonomy != null) &&
                    (createdSim.Autonomy.SituationComponent != null))
                {
                    List<Situation> situations = new List<Situation>(createdSim.Autonomy.SituationComponent.Situations);
                    foreach (Situation situation in situations)
                    {
                        situation.Exit();
                    }
                }

                if (createdSim.LotCurrent != null)
                {
                    Lot lotCurrent = createdSim.LotCurrent;
                    lotCurrent.LastDiedSim = me;
                    lotCurrent.NumDeathsOnLot++;
                }

                if (createdSim.InteractionQueue != null)
                {
                    createdSim.InteractionQueue.CancelAllInteractions();
                }

                urnstone.GhostCleanup(createdSim, true);

                createdSim.Destroy();
            }

            return true;
        }
    }
}
