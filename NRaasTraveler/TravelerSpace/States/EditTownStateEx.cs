using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.States
{
    public class EditTownStateEx : EditTownState
    {
        public EditTownStateEx()
        { }

        public override void Startup()
        {
            // From StateMachine:Startup
            mBaseCallFlag |= BaseCallFlag.kStartup;

            // From InWorldSubState
            while (sDelayNextStateStartupCount > 0x0)
            {
                SpeedTrap.Sleep(0x0);
            }
            EventTracker.SendEvent(new InWorldSubStateEvent(this, true));

            // From EditTownState
            if (SeasonsManager.Enabled)
            {
                SeasonsManager.OnEnterEditTown();
            }

            AudioManager.MusicMode = MusicMode.EditTown;
            UserToolUtils.UserToolGeneric(0xc8, new ResourceKey(0x0L, 0x0, 0x0));
            BorderTreatmentsController.Hide();
            GameUtils.Pause();
            base.PlaceLotWizardCheck();
            EditTownController.Load();
            Tutorialette.TriggerLesson(Lessons.EditTown, null);
            Tutorialette.TriggerLesson(Lessons.PlacingNewVenues, null);
            //Household.DestroyHouseholdsWithoutGuardians();
            CASExitLoadScreen.Close();
            InWorldSubState.EdgeScrollCheck();
            InWorldSubState.OpportunityDialogCheck();
        }
    }
}
