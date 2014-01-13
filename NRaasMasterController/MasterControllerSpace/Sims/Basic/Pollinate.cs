using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class Pollinate : DualSimFromList, IBasicOption
    {
        GameObject mTarget;

        public override string GetTitlePrefix()
        {
            return "Pollinate";
        }

        protected override string GetTitleA()
        {
            return Common.Localize("Pollinate:Mother");
        }

        protected override string GetTitleB()
        {
            return Common.Localize("Pollinate:Father");
        }

        protected override int GetMaxSelectionA()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        public override string Name
        {
            get
            {
                string name = base.Name;

                Sim target = mTarget as Sim;
                if (target != null)
                {
                    string reason = null;
                    if (!Allow(target, ref reason))
                    {
                        name += ": " + reason;
                    }
                }

                return name;
            }
        }

        public override void Reset()
        {
            mTarget = null;

            base.Reset();
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mTarget = parameters.mTarget;

            return base.Allow(parameters);
        }

        protected override IEnumerable<CASAgeGenderFlags> GetSpeciesFilterB()
        {
            return null;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.CreatedSim == null) return false;

            if ((IsFirst) && (me.CreatedSim != mTarget))
            {
                string reason = null;
                if (!Allow(me.CreatedSim, ref reason))
                {
                    Common.DebugNotify("Reason: " + reason);
                    return false;
                }
            }

            return base.PrivateAllow(me);
        }

        protected override bool PrivateAllow(SimDescription a, SimDescription b)
        {
            //if (!base.PrivateAllow(a, b)) return false;

            if (a == null) return false;

            if (b == null) return false;

            if (a.CreatedSim != mTarget)
            {
                string reason = null;
                if (!Allow(a.CreatedSim, ref reason)) return false;
            }

            return true;
        }

        public static bool Allow(Sim woman, ref string reason)
        {
            if ((woman == null) || (woman.InteractionQueue == null))
            {
                reason = Common.Localize("Pollinate:Uninstantiated");
                return false;
            }
            else if (woman.LotHome == null)
            {
                reason = Common.Localize("Pollinate:Homeless", woman.IsFemale, new object[] { woman });
                return false;
            }
            else if (SimTypes.IsSpecial(woman.Household))
            {
                reason = Common.Localize("Pollinate:Service", woman.IsFemale, new object[] { woman });
                return false;
            }
            else if (woman.SimDescription.ChildOrBelow)
            {
                reason = Common.Localize("Pollinate:TooYoung", woman.IsFemale, new object[] { woman });
                return false;
            }
            else if (woman.SimDescription.IsPregnant)
            {
                reason = Common.Localize("Pollinate:AlreadyPregnant", woman.IsFemale, new object[] { woman });
                return false;
            }
            else if (woman.BuffManager.HasTransformBuff())
            {
                reason = Common.Localize("Pollinate:TransformBuff", woman.IsFemale, new object[] { woman });
                return false;
            }
            else if ((woman.SimDescription.AgingState == null) || (woman.SimDescription.AgingState.IsAgingInProgress()))
            {
                reason = Common.Localize("Pollinate:TooOld", woman.IsFemale, new object[] { woman });
                return false;
            }
            else if (!NRaas.MasterController.Settings.mAllowOverStuffed)
            {
                if (woman.IsHuman)
                {
                    if (CommonSpace.Helpers.Households.NumHumansIncludingPregnancy(woman.Household) >= 8)
                    {
                        reason = Common.Localize("Pollinate:TooMany", woman.IsFemale, new object[] { woman });
                        return false;
                    }
                }
                else
                {
                    if (CommonSpace.Helpers.Households.NumPetsIncludingPregnancy(woman.Household) >= 8)
                    {
                        reason = Common.Localize("Pollinate:TooMany", woman.IsFemale, new object[] { woman });
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool Perform(SimDescription a, SimDescription b, bool applyAll)
        {
            if (b == a)
            {
                return Perform(a, (SimDescription)null, applyAll);
            }
            else if (a == null)
            {
                if (b == null) return true;

                return Perform(b, a, applyAll);
            }

            Sim woman = a.CreatedSim;
            if (woman == null) return true;

            Sim man = null;
            if (b != null)
            {
                man = b.CreatedSim;
            }
            else
            {
                man = woman;
            }

            string reason = null;
            if (!Pollinate.Allow(woman, ref reason))
            {
                Common.Notify(reason);
                return true;
            }

            if (Pregnancies.Start(woman, man.SimDescription, false) != null)
            {
                if (!applyAll)
                {
                    SimpleMessageDialog.Show(Common.Localize("Pollinate:MenuName"), Common.Localize("Pollinate:Success", woman.IsFemale, new object[] { woman }));
                }
                else
                {
                    Common.Notify(Common.Localize("Pollinate:Success", woman.IsFemale, new object[] { woman }));
                }
            }
            return true;
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            return Perform(a, b, ApplyAll);
        }
    }
}
