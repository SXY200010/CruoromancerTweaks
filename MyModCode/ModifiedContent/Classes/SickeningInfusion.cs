using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.ElementsSystem;
using Kingmaker.Enums;
using Kingmaker.ResourceLinks;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Abilities.Components.CasterCheckers;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using System;
using System.Linq;

namespace CruoromancerTweaks.ModifiedContent.Classes
{
    internal class SickeningInfusion
    {
        private static readonly string Description = "SickeningInfusion.Description";
        private static readonly string Name = "SickeningInfusion.Name";
        private static readonly string CooldownName = "SickeningInfusionCooldown.Name";

        private const string SickeningInfusionCooldownBuffGuid = "6A79F5D8AE3245D2B2C7F1A959B20ACE";

        private const string WizardClassGuid = "ba34257984f4c41408ce1dc2004e342e";

        private const string SickeningInfusionFreeAbilityGuid = "D51362FA8BE24AE8900C6768EE794284";

        public static void Configure()
        {
            BlueprintAbility ability = BlueprintTool.Get<BlueprintAbility>("18929b7e343c49efaafa88f798c11be8");
            BlueprintAbility falseLife = BlueprintTool.Get<BlueprintAbility>("7a5b5bf845779a941a67251539545762");
            BlueprintBuff sickeningInfusionBuff = BlueprintTool.Get<BlueprintBuff>("bc7bc79049ae49a4a3b20ea1d4f706b1");

            AbilityConfigurator.For(ability)
            .SetDescription(Description)
            .SetDescriptionShort(Description)
            .SetAnimation(falseLife.Animation)
            .SetHasFastAnimation(true)
            .Configure();

            ability.AnimationStyle = falseLife.AnimationStyle;

            BlueprintBuff cooldownBuff =
                BuffConfigurator.New(
                        "SickeningInfusionCooldown",
                        SickeningInfusionCooldownBuffGuid
                    )
                    .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                    .SetStacking(StackingType.Replace)
                    .SetDisplayName(CooldownName)
                    .Configure();

            AbilityConfigurator.For(ability)
                .EditComponent<AbilityCasterHasNoFacts>(c =>
                {
                    c.m_Facts = new BlueprintUnitFactReference[]
                    {
                        cooldownBuff.ToReference<BlueprintUnitFactReference>()
                    };
                })
                .EditComponent<AbilityEffectRunAction>(c =>
                {
                    var list = c.Actions.Actions.ToList();

                    list.Add(new ContextActionOnContextCaster
                    {
                        Actions = new ActionList
                        {
                            Actions = new GameAction[]
                            {
                                new ContextActionApplyBuff
                                {
                                    m_Buff = cooldownBuff.ToReference<BlueprintBuffReference>(),
                                    ToCaster = true,
                                    DurationValue = new ContextDurationValue
                                    {
                                        DiceType = DiceType.Zero,
                                        DiceCountValue = 0,
                                        BonusValue = 1,
                                        Rate = DurationRate.Rounds
                                    }
                                }
                            }
                        }
                    });
                    c.Actions.Actions = list.ToArray();
                })
                .SetCanTargetSelf(true)
                .AddComponent<AbilitySpawnFx>(c =>
                {
                    c.PrefabLink = new PrefabLink
                    {
                        AssetId = "e93261ee4c3ea474e923f6a645a3384f"
                    };

                    c.Time = AbilitySpawnFxTime.OnApplyEffect;
                    c.Anchor = AbilitySpawnFxAnchor.Caster;

                    c.OrientationMode = AbilitySpawnFxOrientation.Copy;
                    c.OrientationAnchor = AbilitySpawnFxAnchor.None;
                    c.PositionAnchor = AbilitySpawnFxAnchor.None;
                })
               .Configure();

            BuffConfigurator.For(sickeningInfusionBuff)
                .AddContextRankConfig(
                    ContextRankConfigs
                        .ClassLevel(new[] { WizardClassGuid })
                        .WithDiv2Progression()
                )
                .EditComponents<AddAbilityUseTrigger>(
                    c =>
                    {
                        for (int i = 0; i < c.Action.Actions.Length; i++)
                        {
                            if (
                                c.Action.Actions[i] is ContextActionApplyBuff apply &&
                                apply.m_Buff != null &&
                                apply.m_Buff.Guid == "4e42460798665fd4cb9173ffa7ada323") 
                            {                                
                                apply.DurationValue = new ContextDurationValue
                                {
                                    Rate = DurationRate.Rounds,
                                    DiceType = DiceType.One,
                                    DiceCountValue = new ContextValue
                                    {
                                        ValueType = ContextValueType.Rank,
                                        ValueRank = AbilityRankType.Default
                                    },
                                    BonusValue = new ContextValue
                                    {
                                        ValueType = ContextValueType.Simple,
                                        Value = 1
                                    }
                                };

                                c.Action.Actions[i] = new Conditional
                                {
                                    ConditionsChecker = new ConditionsChecker
                                    {
                                        Operation = Operation.And,
                                        Conditions = new Condition[]
                                        {
                                            new ContextConditionIsEnemy()
                                        }
                                    },
                                    IfTrue = new ActionList
                                    {
                                        Actions = new GameAction[] { apply }
                                    }
                                };
                            }
                        }
                    },
                    _ => true
                )
                .Configure();


            //BuffConfigurator.For(sickeningInfusionBuff)


            Type[] componentTypes = ability.ComponentsArray
            .Select(c => c.GetType())
            .Distinct()
            .ToArray();

            BlueprintAbility sickeningInfusionFree =
                AbilityConfigurator.New(
                        "SickeningInfusionFreeAbility",
                        SickeningInfusionFreeAbilityGuid
                )
                .CopyFrom(ability, componentTypes)
                .SetDisplayName(Name)
                .SetActionType(UnitCommand.CommandType.Free)
                .Configure();
            sickeningInfusionFree.m_AllElements = ability.ElementsArray;
        }
    }
}
