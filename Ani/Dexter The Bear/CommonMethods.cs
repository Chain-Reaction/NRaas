using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Utilities;

namespace TheKnifeProject
{
    public class CommonMethods
    {
        #region Localization
        public static string LocalizeString(string name, params object[] parameters)
        {
            return Localization.LocalizeString("Dex:" + name, parameters);
        }
        #endregion
    }
}
