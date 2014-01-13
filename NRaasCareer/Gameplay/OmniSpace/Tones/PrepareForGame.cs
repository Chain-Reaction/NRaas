using NRaas.Gameplay.Careers;
using NRaas.Gameplay.OmniSpace.Metrics;
using NRaas.Gameplay.Tones;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.Gameplay.OmniSpace.Tones
{
    [Persistable]
    public class PrepareForGame : CareerToneEx
    {
        // Methods
        public override void OnTimePassed(InteractionInstance interactionInstance, float totalTime, float deltaTime)
        {
            base.OnTimePassed(interactionInstance, totalTime, deltaTime);

            ProSports career = OmniCareer.Career<ProSports>(base.Career);
            if ((career != null) && (career.CurLevel != null))
            {
                career.mPreparationForGame = Math.Min((float)100f, (float)(career.mPreparationForGame + ((deltaTime * ProSports.PrepareForGame.kPrepareForGamePerSimHour) / 60f)));
                this.OnProgressChanged(this);
            }
        }

        public override bool Test(InteractionInstance ii, out StringDelegate reason)
        {
            if (!base.Test(ii, out reason)) return false;

            return (base.Career as ProSports).HasWinLossRecordMetric();
        }

        // Properties
        public override bool IsProgressTone
        {
            get
            {
                return true;
            }
        }

        public override float Progress
        {
            get
            {
                ProSports career = OmniCareer.Career<ProSports> (base.Career);
                if ((career != null) && (career.CurLevel != null))
                {
                    return career.mPreparationForGame;
                }
                return 0f;
            }
        }
    }
}
