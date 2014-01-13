using NRaas.CommonSpace.Booters;
using NRaas.CareerSpace.Careers;
using NRaas.CareerSpace.Helpers;
using NRaas.CareerSpace.Interfaces;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Booters
{
    public class HomemakerDiscountObjectsBooter : BooterHelper.TableBooter, IHomemakerBooter
    {
        static Dictionary<Type, bool> sData = new Dictionary<Type, bool>();

        public HomemakerDiscountObjectsBooter()
            : base("DiscountObjects", "NRaas.Homemaker", true)
        { }

        public static bool IsObject(Type type)
        {
            foreach (Type objType in sData.Keys)
            {
                if (objType.IsAssignableFrom(type))
                {
                    return true;
                }
            }

            return false;
        }

        public override BooterHelper.BootFile GetBootFile(string reference, string name, bool primary)
        {
            return new BooterHelper.DataBootFile(reference, name, primary);
        }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            Type type = row.GetClassType("Type");
            if (type == null)
            {
                BooterLogger.AddError("Invalid Type: " + row.GetString("Type"));
                return;
            }

            sData.Add(type, true);

            BooterLogger.AddTrace(" Discount Type: " + row.GetString("Type"));
        }
    }
}
