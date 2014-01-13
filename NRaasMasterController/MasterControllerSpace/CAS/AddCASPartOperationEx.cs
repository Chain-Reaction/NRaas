using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.CAS.CAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.CAS
{
    public class AddCASPartOperationEx : CASLogic.AddCASPartOperation
    {
        public AddCASPartOperationEx(CASPart part, bool randomizeDesign)
            : base(part, randomizeDesign)
        {}
        public AddCASPartOperationEx(CASPart part, string presetStr)
            : base(part, presetStr)
        { }

        public override void Initialize()
        {
            CASLogic singleton = CASLogic.GetSingleton();
            if (((singleton.Age & mPart.Age) != CASAgeGenderFlags.None) && ((singleton.Gender & mPart.Gender) != CASAgeGenderFlags.None))
            {
                // Custom
                ReplacePart(singleton, mPart, mPreset);

                if (mRandomizeDesign)
                {
                    singleton.RandomizeDesign(mPart);
                }
                if (mBlend != null)
                {
                    if (mBlend is CompoundBlend)
                    {
                        base.AddStep(new CASLogic.TriggerCompoundBlendUpdateStep());
                    }

                    singleton.SetBlendAmount(mBlend, mBlendAmount, false);
                }
            }
            else
            {
                CASLogic.CASOperationStack.Instance.RequestUndo();
                CASLogic.CASOperationStack.Instance.RequestClearRedo();
            }
        }

        private static void ReplacePart(CASLogic ths, CASPart newPart, string preset)
        {
            List<CASPart> wornParts = ths.mBuilder.GetWornParts(new BodyTypes[] { newPart.BodyType });

            foreach (CASPart worn in wornParts)
            {
                if (newPart.Key == worn.Key)
                {
                    CASLogic.CASOperationStack.Instance.Active.AddStep(new CASLogic.ApplyPresetStringToPartStep(newPart, preset));
                    return;
                }
            }

            ths.RequestAddPart(newPart, preset);
        } 
    }
}
