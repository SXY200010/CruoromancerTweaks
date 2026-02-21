using BlueprintCore.Utils;
using CruoromancerTweaks.Utils;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;

namespace CruoromancerTweaks.ModifiedContent
{
    /// <summary>
    /// 只负责创建 5 个疾病恶化 Buff
    /// 不做自动绑定，不做系统逻辑
    /// </summary>
    public static class ContagionAggravationBuffs
    {
        public static BlueprintBuff BlindingSicknessAggravation;
        public static BlueprintBuff BubonicPlagueAggravation;
        public static BlueprintBuff MindFireAggravation;
        public static BlueprintBuff BrainFeverAggravation;
        public static BlueprintBuff ShakesAggravation;

        public static void Configure()
        {
            // 目盲症：1d4 STR
            BlindingSicknessAggravation =
                PeriodicSavingThrowDamageBuffFactory.Create(
                    name: "BlindingSicknessAggravationBuff",
                    guid: "60512B58-674E-4E7B-9B6F-B35EEC02268A",
                    stat: StatType.Strength,
                    saveType: SavingThrowType.Fortitude,
                    diceType: DiceType.D4,
                    diceCount: 1,
                    bonusValue: PeriodicSavingThrowDamageBuffFactory.Fixed(0)
                );

            // 黑死病：1d4 CON
            BubonicPlagueAggravation =
                PeriodicSavingThrowDamageBuffFactory.Create(
                    name: "BubonicPlagueAggravationBuff",
                    guid: "3542C225-FBD7-4CD0-A531-14D15B7BF00B",
                    stat: StatType.Constitution,
                    saveType: SavingThrowType.Fortitude,
                    diceType: DiceType.D4,
                    diceCount: 1,
                     bonusValue: PeriodicSavingThrowDamageBuffFactory.Fixed(0)
                );

            // 失心疯：1d6 WIS
            MindFireAggravation =
                PeriodicSavingThrowDamageBuffFactory.Create(
                    name: "MindFireAggravationBuff",
                    guid: "7638B3A6-6CEC-45FE-9572-B481411A5111",
                    stat: StatType.Wisdom,
                    saveType: SavingThrowType.Fortitude,
                    diceType: DiceType.D6,
                    diceCount: 1,
                    bonusValue: PeriodicSavingThrowDamageBuffFactory.Fixed(0)
                );

            // 脑热症：1d4 INT
            BrainFeverAggravation =
                PeriodicSavingThrowDamageBuffFactory.Create(
                    name: "BrainFeverAggravationBuff",
                    guid: "8AF2C46F-F245-498E-9037-EA29CF029740",
                    stat: StatType.Intelligence,
                    saveType: SavingThrowType.Fortitude,
                    diceType: DiceType.D4,
                    diceCount: 1,
                    bonusValue: PeriodicSavingThrowDamageBuffFactory.Fixed(0)
                );

            // 寒颤症：1d8 DEX
            ShakesAggravation =
                PeriodicSavingThrowDamageBuffFactory.Create(
                    name: "ShakesAggravationBuff",
                    guid: "37E09E37-E3C5-49D9-9608-BBB06623FE77",
                    stat: StatType.Dexterity,
                    saveType: SavingThrowType.Fortitude,
                    diceType: DiceType.D8,
                    diceCount: 1,
                    bonusValue: PeriodicSavingThrowDamageBuffFactory.Fixed(0)
                );
        }
    }
}
