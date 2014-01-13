using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Checks
{
    public class CheckPicnicTable : Check<PicnicTable>
    {
        protected override bool PrePerform(PicnicTable obj, bool postLoad)
        {
            bool repair = false;
            if (obj.SeatingGroup != null)
            {
                if (obj.mInsideSeats == null)
                {
                    repair = true;
                }
                else
                {
                    foreach (PartData seat in obj.mInsideSeats)
                    {
                        if (seat == null)
                        {
                            repair = true;
                            break;
                        }
                    }
                }
            }

            if (repair)
            {
                obj.OnCreation();

                LogCorrection("Picnic Table Seats Repaired: " + ErrorTrap.GetName(obj));
            }

            return true;
        }

        protected override bool PostPerform(PicnicTable obj, bool postLoad)
        {
            return base.PostPerform(obj, postLoad);
        }
    }
}
