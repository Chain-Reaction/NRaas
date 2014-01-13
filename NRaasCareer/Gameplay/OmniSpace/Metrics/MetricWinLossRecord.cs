using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.Gameplay.OmniSpace.Metrics
{
    [Persistable]
    public class MetricWinLossRecord : Sims3.Gameplay.Careers.ProSports.MetricWinLossRecord
    {
        public MetricWinLossRecord(XmlDbRow row, int metricNumber)
            : base(row, metricNumber)
        { }

        public override float FindMetricValue(Career career)
        {
            ProSports sports = OmniCareer.Career<ProSports>(career);
            if (sports != null)
            {
                return (float)(sports.mWinRecord - sports.mLossRecord);
            }
            return 0f;
        }

        private static string LocalizeString(SimDescription sim, string name, params object[] parameters)
        {
            return OmniCareer.LocalizeString(sim, "MetricPrepareForGame:" + name, "Gameplay/Careers/ProSports/MetricPrepareForGame:" + name, parameters);
        }

        public override string MetricDescription(Career career)
        {
            ProSports sports = OmniCareer.Career<ProSports>(career);
            if (sports != null)
            {
                return OmniCareer.LocalizeString(career.OwnerDescription, "DescriptionWinLossRecord", "Gameplay/Careers/Metrics:DescriptionWinLossRecord", new object[] { sports.GamesTillSeasonEnds, sports.GamesWon, sports.GamesLost, sports.GamesWonTotal, sports.GamesLostTotal });
            }
            return "";
        }

        public override string MetricTitle(Career career)
        {
            return OmniCareer.LocalizeString(career.OwnerDescription, "TitleWinLossRecord", "Gameplay/Careers/Metrics:TitleWinLossRecord", new object[0]);
        }
    }
}
