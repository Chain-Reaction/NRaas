using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Utilities;
using Sims3.UI;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;

namespace Alcohol
{
    class TrayInteractions : BarTray
    {
       #region Call For Drinks
        public class CallForDrinks : ImmediateInteraction<Sim, BarTray>
        {
            // Fields

            public static readonly InteractionDefinition Singleton = new Definition();

            private const string sLocalizationKey = "CallForDrinks:";

            // Methods

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString(sLocalizationKey + name, parameters);
            }

            public override bool Run()
            {
                List<Sim> selectedSims = new List<Sim>();
                List<object> selectedObjects = base.SelectedObjects;
                if (selectedObjects != null)
                {
                    foreach (object obj2 in selectedObjects)
                    {
                        selectedSims.Add(obj2 as Sim);
                    }
                }


                //Loop through the sims and add them the coffee drinking interaction
                foreach (Sim sim in selectedSims)
                {
                    InteractionInstance ii = BarTray.GetDrink.Singleton.CreateInstance(base.Target, sim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true);
                    sim.InteractionQueue.Add(ii);
                }


                return true;

            }

            // Nested Types

            private sealed class Definition : ImmediateInteractionDefinition<Sim, BarTray, TrayInteractions.CallForDrinks>
            {

                // Methods
                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 4;
                    Sim actor = parameters.Actor as Sim;
                    List<Sim> sims = new List<Sim>();

                    foreach (Sim sim in parameters.Target.LotCurrent.GetSims())
                    {
                        if (sim.SimDescription.TeenOrAbove)
                        {
                            sims.Add(sim);
                        }
                    }

                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                }

                public override string GetInteractionName(Sim a, BarTray target, InteractionObjectPair interaction)
                {

                    return TrayInteractions.CallForDrinks.LocalizeString("CallForCoffee", new object[0]);

                }

                public override bool Test(Sim a, BarTray target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return a.SimDescription.TeenOrAbove;
                }

            }
        }
       #endregion
    }
      
}
