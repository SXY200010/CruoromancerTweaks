using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;

namespace CruoromancerTweaks.Utils
{
    /// <summary>
    /// 通用工厂：
    /// 创建“每轮进行一次豁免，失败则造成属性伤害”的临时 Buff。
    /// 可用于：
    /// - 疾病恶化
    /// - 持续毒素
    /// - 腐蚀
    /// - 诅咒进程
    /// - 任意 Periodic Save → Effect 机制
    /// </summary>
    public static class PeriodicSavingThrowDamageBuffFactory
    {
        /// <summary>
        /// 创建一个周期性“豁免 → 失败造成属性伤害”的 Buff
        /// </summary>
        public static BlueprintBuff Create(
            string name,
            string guid,
            StatType stat,
            SavingThrowType saveType,
            DiceType diceType,
            int diceCount,
            ContextValue bonusValue = null,
            bool hiddenInUi = true)
        {
            var buff = BuffConfigurator.New(name, guid);

            if (hiddenInUi)
                buff.SetFlags(BlueprintBuff.Flags.HiddenInUi);

            buff.AddFactContextActions(
                newRound: CreateNewRoundAction(
                    stat,
                    saveType,
                    diceType,
                    diceCount,
                    bonusValue
                )
            );

            return buff.Configure();
        }

        /// <summary>
        /// 生成每轮触发的 Action 树
        /// </summary>
        private static ActionList CreateNewRoundAction(
            StatType stat,
            SavingThrowType saveType,
            DiceType diceType,
            int diceCount,
            ContextValue bonusValue)
        {
            return new ActionList
            {
                Actions = new GameAction[]
                {
                    new ContextActionSavingThrow
                    {
                        Type = saveType,
                        Actions = new ActionList
                        {
                            Actions = new GameAction[]
                            {
                                new ContextActionConditionalSaved
                                {
                                    Succeed = new ActionList(),

                                    Failed = new ActionList
                                    {
                                        Actions = new GameAction[]
                                        {
                                            new ContextActionDealDamage
                                            {
                                                DamageType = new DamageTypeDescription
                                                {
                                                    Type = DamageType.Direct
                                                },
                                                AbilityType = stat,
                                                Value = new ContextDiceValue
                                                {
                                                    DiceType = diceType,
                                                    DiceCountValue = diceCount,
                                                    BonusValue = bonusValue
                                                },
                                                m_Type = ContextActionDealDamage.Type.AbilityDamage
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// 快捷方法：创建一个固定数值的 Bonus ContextValue
        /// </summary>
        public static ContextValue Fixed(int value)
        {
            return new ContextValue
            {
                ValueType = ContextValueType.Simple,
                Value = value
            };
        }

        /// <summary>
        /// 快捷方法：使用 Rank 作为 Bonus（例如 CL scaling）
        /// </summary>
        public static ContextValue Rank()
        {
            return new ContextValue
            {
                ValueType = ContextValueType.Rank
            };
        }
    }
}
