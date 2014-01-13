using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class NumberOfParents : SelectionTestableOptionList<NumberOfParents.Item, NumberOfParents.Choice, NumberOfParents.Choice>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.NumberOfParents";
        }

        public enum Choice : int
        {
            None = 0,
            OneMother,
            OneFather,
            TwoMothers,
            TwoFathers,
            Both,
        }

        public class Item : TestableOption<Choice, Choice>
        {
            protected static bool GetType(IMiniSimDescription me, out Choice type)
            {
                type = Choice.None;

                try
                {
                    if (me.CASGenealogy == null) return false;

                    if (me.CASGenealogy.IParents == null) return false;

                    int male = 0;
                    int female = 0;

                    foreach (IGenealogy parent in me.CASGenealogy.IParents)
                    {
                        if (parent.IMiniSimDescription == null) continue;

                        if (parent.IMiniSimDescription.IsFemale)
                        {
                            female++;
                        }
                        else
                        {
                            male++;
                        }
                    }

                    int count = female + male;
                    if (count == 0)
                    {
                        type = Choice.None;
                    }
                    else if (count == 2)
                    {
                        switch (female)
                        {
                            case 0:
                                type = Choice.TwoFathers;
                                break;
                            case 1:
                                type = Choice.Both;
                                break;
                            case 2:
                                type = Choice.TwoMothers;
                                break;
                        }
                    }
                    else
                    {
                        if (female == 1)
                        {
                            type = Choice.OneMother;
                        }
                        else
                        {
                            type = Choice.OneFather;
                        }
                    }

                    return true;
                }
                catch
                {
                    // Sim may not have a valid celebrity manager
                    return false;
                }
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<Choice, Choice> results)
            {
                Choice type = Choice.None;
                if (!GetType(me, out type)) return false;

                results[type] = type;
                return true;
            }
            public override bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<Choice, Choice> results)
            {
                Choice type = Choice.None;
                if (!GetType(me, out type)) return false;

                results[type] = type;
                return true;
            }

            public override void SetValue(Choice value, Choice storeType)
            {
                mValue = value;

                mName = Common.Localize("ParentType:" + value);
            }
        }
    }
}
