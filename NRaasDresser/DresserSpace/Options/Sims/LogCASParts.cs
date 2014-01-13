using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.DresserSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.DresserSpace.Options.Sims
{
    public class LogCASParts : OperationSettingOption<Sim>, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "LogCASParts";
        }

        protected override OptionResult Run(GameHitParameters< Sim> parameters)
        {
            SimOutfit outfit = new SimOutfit(CASUtils.GetOutfitInGameObject(parameters.mTarget.Proxy.ObjectId));

            string msg = "";
            foreach (CASPart part in outfit.Parts)
            {
                msg += CASParts.PartToString(part);
            }

            Common.WriteLog(msg);

            SimpleMessageDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Success", parameters.mTarget.IsFemale, new object[] { parameters.mTarget }));

            return OptionResult.SuccessClose;
        }
    }
}
