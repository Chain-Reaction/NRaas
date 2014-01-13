using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
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

namespace NRaas.CommonSpace.Options
{
    public class InteractionDefinitionOptionList : InteractionOptionList<IInteractionDefinitionOption, GameObject>, IInteractionDefinitionOption
    {
        List<IInteractionDefinitionOption> mOptions = new List<IInteractionDefinitionOption>();

        [Persistable(false)]
        Dictionary<string, InteractionDefinitionOptionList> mLookup = new Dictionary<string, InteractionDefinitionOptionList>();
     
        public InteractionDefinitionOptionList(string name)
            : base(name)
        { }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public InteractionDefinitionOptionList GetOption(string name)
        {
            if (name == null) return null;

            InteractionDefinitionOptionList option = null;
            if (!mLookup.TryGetValue(name, out option))
            {
                return null;
            }

            return option;
        }

        public override List<IInteractionDefinitionOption> GetOptions()
        {
            return mOptions;
        }

        public void Add(IInteractionDefinitionOption option)
        {
            if (option is InteractionDefinitionOptionList)
            {
                mLookup.Add(option.Name, option as InteractionDefinitionOptionList);
            }

            mOptions.Add(option);
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return true;
        }

        public static InteractionDefinitionOptionList CreateNesting(IActor actor, GameObject target, GameObjectHit hit, List<InteractionDefinition> definitions)
        {
            List<InteractionObjectPair> pairs = new List<InteractionObjectPair>();

            foreach (InteractionDefinition definition in definitions)
            {
                pairs.Add(new InteractionObjectPair(definition, target));
            }

            return CreateNesting(actor, hit, pairs);
        }
        public static InteractionDefinitionOptionList CreateNesting(IActor actor, GameObjectHit hit, List<InteractionObjectPair> pairs)
        {
            InteractionDefinitionOptionList primary = new InteractionDefinitionOptionList("");

            foreach (InteractionObjectPair pair in pairs)
            {
                GreyedOutTooltipCallback greyedOutTooltipCallback = null;

                InteractionInstanceParameters parameters = new InteractionInstanceParameters(pair, actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true);
                parameters.mGameObjectHit = hit;

                InteractionTestResult result = pair.InteractionDefinition.Test(ref parameters, ref greyedOutTooltipCallback);
                if (!IUtil.IsPass(result)) continue;

                string[] paths = pair.InteractionDefinition.GetPath(actor.IsFemale);

                List<string> fullPath = new List<string>();

                if (paths != null)
                {
                    fullPath.AddRange(paths);
                }

                if (pair is IopWithPrependedPath)
                {
                    fullPath.Insert(0, (pair as IopWithPrependedPath).PrependPath);
                }

                InteractionDefinitionOptionList list = primary;

                foreach (string path in fullPath)
                {
                    string lookup = path;
                    if (lookup == null)
                    {
                        lookup = "";
                    }

                    InteractionDefinitionOptionList nextList = list.GetOption(lookup);
                    if (nextList == null)
                    {
                        nextList = new InteractionDefinitionOptionList(lookup);

                        list.Add(nextList);
                    }

                    list = nextList;
                }

                list.Add(new InteractionDefinitionOption(pair, actor));
            }

            return primary;
        }

        public static OptionResult Perform(IActor actor, GameObjectHit hit, List<InteractionObjectPair> pairs)
        {
            return Perform(actor, hit, pairs, VersionStamp.sPopupMenuStyle);
        }
        public static OptionResult Perform(IActor actor, GameObjectHit hit, List<InteractionObjectPair> pairs, bool popupStyle)
        {
            if (popupStyle)
            {
                Sims3.Gameplay.UI.PieMenu.TestAndBringUpPieMenu(actor, new InteractionDefinitionOptionList.UIMouseEventArgsEx(), hit, pairs, InteractionMenuTypes.Normal);
                return OptionResult.SuccessClose;
            }
            else
            {
                InteractionDefinitionOptionList primary = InteractionDefinitionOptionList.CreateNesting(actor, hit, pairs);

                return primary.Perform(new GameHitParameters<GameObject>(actor, actor as GameObject, hit));
            }
        }

        public class UIMouseEventArgsEx : UIMouseEventArgs
        {
            public UIMouseEventArgsEx()
            {
                Vector2 position = UIManager.GetCursorPosition();

                mF1 = position.x;
                mF2 = position.y;
            }
        }
    }
}
