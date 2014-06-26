using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TaggerSpace.Options
{
    public class TagSim : OperationSettingOption<GameObject>, ISimOption
    {
        IGameObject mTarget;

        public override string GetTitlePrefix()
        {
            return "TagSim";
        }

        public override string Name
        {
            get
            {
                Sim target = mTarget as Sim;
                if (mTarget != null)
                {
                    if (!Tagger.Settings.mTaggedSims.Contains(target.SimDescription.SimDescriptionId))
                    {
                        return Tagger.Localize("TagSim:MenuName");
                    }
                    else
                    {
                        return Tagger.Localize("UntagSim:MenuName");
                    }
                }

                return "Debug: TagSim";
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mTarget = parameters.mTarget;
            if (mTarget == null) return false;

            if (!(mTarget is Sim)) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Sim target = mTarget as Sim;
            if (target != null)
            {
                if (Tagger.Settings.mTaggedSims.Contains(target.SimDescription.SimDescriptionId))
                {
                    Tagger.Settings.mTaggedSims.Remove(target.SimDescription.SimDescriptionId);
                }
                else
                {
                    Tagger.Settings.mTaggedSims.Add(target.SimDescription.SimDescriptionId);
                }

                Common.Notify(Common.Localize("General:Success"));
            }

            return OptionResult.SuccessClose;
        }
    }
}