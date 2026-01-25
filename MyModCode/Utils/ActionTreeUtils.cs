using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruoromancerTweaks.Utils
{
    //用来处理多重套娃情况的。可以搜索某一个组件内的树中的所有特定类型组件，具体到某一个组件可以使用if。
    //示例：搜索AbilityEffectRunAction组件内的Actions树中的succeed时应用战栗buff的ContextActionConditionalSaved组件。
    //AbilityConfigurator.For(causeFear)
    //在bilityEffectRunAction组件内的Actions树中搜索
    //.EditComponent<AbilityEffectRunAction>(c =>
    //{
    //    foreach (var rootAction in c.Actions.Actions)
    //    {
    //        ActionTreeUtils.Walk(rootAction, a =>
    //        {
    //            //找到ContextActionConditionalSaved组件
    //            if (a is ContextActionConditionalSaved saved)
    //            {
    //                //在succeed分支中搜索
    //                foreach (var succeed in saved.Succeed.Actions)
    //                {
    //                    //找到ContextActionApplyBuff组件并且是战栗buff
    //                    if (succeed is ContextActionApplyBuff apply &&
    //                        apply.m_Buff?.Guid == shaken.AssetGuid)
    //                    {
    //                        apply.DurationValue = new ContextDurationValue
    //                        {
    //                            Rate = DurationRate.Rounds,
    //                            DiceType = DiceType.D4,
    //                            DiceCountValue = 1,
    //                            BonusValue = 0
    //                        };
    //                    }
    //                }
    //            }
    //        });
    //    }
    //})
    //.Configure();
    public static class ActionTreeUtils
    {
        public static void Walk(GameAction action, Action<GameAction> visitor)
        {
            if (action == null) return;

            visitor?.Invoke(action);

            switch (action)
            {
                case Conditional cond:
                    WalkList(cond.IfTrue, visitor);
                    WalkList(cond.IfFalse, visitor);
                    break;

                case ContextActionSavingThrow save:
                    WalkList(save.Actions, visitor);
                    break;

                case ContextActionConditionalSaved saved:
                    WalkList(saved.Succeed, visitor);
                    WalkList(saved.Failed, visitor);
                    break;
            }
        }

        public static void WalkList(ActionList list, Action<GameAction> visitor)
        {
            if (list?.Actions == null) return;

            foreach (var action in list.Actions)
                Walk(action, visitor);
        }
    }
}
