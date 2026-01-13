using BlueprintCore.Blueprints.Configurators;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.ElementsSystem;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Mechanics.Properties;
using System.Linq;

namespace CruoromancerTweaks.ModifiedContent.Classes
{
    internal class CommandingInfusion
    {
        private static readonly string Description = "CommandingInfusion.Description";

        private const string CommandingInfusionLv10BuffGuid = "0485AC5BD2C64019AC0E5A233A12A684";
        private const string CommandingInfusionLv10AllyBuffGuid = "29D13CDF9C3347FFBD9553EA8A28D841";
        private const string CommandingInfusionLv10AreaGuid = "36E4CB147D244ED9B3EABC4FB18018B1";
        private const string CommandingInfusionLv15BuffGuid = "7D9461259987457AB8B02232E1B8F3FF";
        private const string CommandingInfusionLv15AreaGuid = "CB7B1A233D27420B986B112B0B1341DD";
        private const string CommandingInfusionLv15AllyBuffGuid = "76C41DFD03CD463EA2DDB4C70D18B843";
        public static void Configure()
        {
            BlueprintBuff commandingInfusionAllyBuff = BlueprintTool.Get<BlueprintBuff>("cd72077182894e4e935662a16fe0051b");
            BlueprintBuff commandingInfusionBuff = BlueprintTool.Get<BlueprintBuff>("406711ebdd8f4f419e95c9b4778b5927");
            BlueprintFeature commandingInfusionFeature = BlueprintTool.Get<BlueprintFeature>("52ab0f623a2446758c0f22fedc2bda70");
            BlueprintAbility commandingInfusionAbility = BlueprintTool.Get<BlueprintAbility>("6489492a34994d00926e8802c3f2b4cf");
            BlueprintAbilityAreaEffect CommandingInfusionArea = BlueprintTool.Get<BlueprintAbilityAreaEffect>("aa55fa6be38a469d934ca46d8fab6d6a");

            BlueprintFeature commandingInfusionCheckFeature = BlueprintTool.Get<BlueprintFeature>("9e201f3ef56a4c76a55b24b0ab53262c");
            BlueprintUnit devourerSummoned = BlueprintTool.Get<BlueprintUnit>("a1acea7cda5e46d8b8d0549be25e0acb");
            BlueprintUnit graveknightSummoned = BlueprintTool.Get<BlueprintUnit>("7237a32613fe55e479d1141682f2bbd4");
            BlueprintUnit livingArmorTankSummoned = BlueprintTool.Get<BlueprintUnit>("41338e460cea53742842f41a89cbb2b3");
            BlueprintUnit shadowGreaterAdvancedSummoned = BlueprintTool.Get<BlueprintUnit>("6ea333f52fd2421a9892ef9be6fbb03b");
            BlueprintFeature undeadType = BlueprintTool.Get<BlueprintFeature>("734a29b693e9ec346ba2951b27987e33");
            //BlueprintUnit[] commandingInfusionUnits = new BlueprintUnit[]
            //{
            //    devourerSummoned,
            //    graveknightSummoned,
            //    livingArmorTankSummoned,
            //    shadowGreaterAdvancedSummoned
            //};

            //增强加值改成天赋加值
            BuffConfigurator.For(commandingInfusionAllyBuff)
                .EditComponents<AddContextStatBonus>(
                    edit: c => c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Inherent,
                    predicate: c => true
                )
                .Configure();

            //给召唤生物添加可以被强化的特性
            //foreach (BlueprintUnit unit in commandingInfusionUnits)
            //{
            //    UnitConfigurator.For(unit)
            //        .AddComponent<AddFacts>(c =>
            //        {
            //            c.m_Facts = c.m_Facts.AddToArray(commandingInfusionCheckFeature.ToReference<BlueprintUnitFactReference>());
            //        })
            //        .Configure();
            //}

            //创建10级增益buff
            BlueprintBuff commandingInfusionLv10AllyBuff =
                BuffConfigurator.New("CommandingInfusionLv10AllyBuff", CommandingInfusionLv10AllyBuffGuid)
                    .CopyFrom(commandingInfusionAllyBuff)
                    .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                    .SetStacking(StackingType.Stack)
                    .Configure();


            //创建15级增益buff
            BlueprintBuff commandingInfusionLv15AllyBuff =
                BuffConfigurator.New("CommandingInfusionLv15AllyBuff", CommandingInfusionLv15AllyBuffGuid)
                    .CopyFrom(commandingInfusionAllyBuff)
                    .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                    .SetStacking(StackingType.Stack)
                    .AddContextRankConfig(
                        ContextRankConfigs.CasterLevel()
                            .WithMultiplyByModifierProgression(3)
                    )
                    .AddTemporaryHitPointsFromAbilityValue(
                         value: new ContextValue
                         {
                             ValueType = ContextValueType.Rank
                         }
                    )
                    .Configure();

            //更新描述
            AbilityConfigurator.For(commandingInfusionAbility)
                .SetDescription(Description)
                 .EditComponent<AbilityEffectRunAction>(c =>
                 {
                     foreach (var action in c.Actions.Actions.OfType<ContextActionApplyBuff>())
                     {
                         if (action.m_Buff?.Get() == commandingInfusionBuff)
                         {
                             action.DurationValue = new ContextDurationValue
                             {
                                 Rate = DurationRate.Minutes,
                                 DiceType = DiceType.Zero,
                                 DiceCountValue = 0,
                                 BonusValue = new ContextValue
                                 {
                                     ValueType = ContextValueType.CasterProperty,
                                     Property = UnitProperty.Level
                                 }
                             };
                         }
                     }
                 })
                .Configure();

            BuffConfigurator.For(commandingInfusionBuff)
                .SetDescription(Description)
                .Configure();

            FeatureConfigurator.For(commandingInfusionFeature)
                .SetDescription(Description)
                .Configure();

            AbilityAreaEffectConfigurator.For(CommandingInfusionArea)
                 .EditComponent<AbilityAreaEffectRunAction>(c =>
                 {
                     c.UnitEnter = new ActionList
                     {
                         Actions = new GameAction[]
                        {
                            new Conditional
                            {
                                Owner = CommandingInfusionArea,
                                ConditionsChecker = new ConditionsChecker
                                {
                                    Conditions = new Condition[]
                                    {
                                        new ContextConditionIsAlly(),
                                        new ContextConditionHasFact
                                        {
                                            m_Fact = undeadType.ToReference<BlueprintUnitFactReference>()
                                        },
                                        new ContextConditionHasFact
                                        {
                                            m_Fact = commandingInfusionAllyBuff.ToReference<BlueprintUnitFactReference>(),
                                            Not = true
                                        }
                                    }
                                },
                                IfTrue = new ActionList
                                {
                                    Actions = new GameAction[]
                                    {
                                        new ContextActionApplyBuff
                                        {
                                            m_Buff = commandingInfusionAllyBuff.ToReference<BlueprintBuffReference>(),
                                            AsChild = true,
                                            Permanent = true
                                        },

                                        // Lv >= 10
                                        new Conditional
                                        {
                                            ConditionsChecker = new ConditionsChecker
                                            {
                                                Conditions = new Condition[]
                                                {
                                                    new ContextConditionCompare
                                                    {
                                                        m_Type = ContextConditionCompare.Type.GreaterOrEqual,
                                                        CheckValue = new ContextValue
                                                        {
                                                            ValueType = ContextValueType.CasterProperty,
                                                            Property = UnitProperty.Level
                                                        },
                                                        TargetValue = new ContextValue
                                                        {
                                                            ValueType = ContextValueType.Simple,
                                                            Value = 10
                                                        }
                                                    }
                                                }
                                            },
                                            IfTrue = new ActionList
                                            {
                                                Actions = new GameAction[]
                                                {
                                                    new ContextActionApplyBuff
                                                    {
                                                        m_Buff = commandingInfusionLv10AllyBuff.ToReference<BlueprintBuffReference>(),
                                                        AsChild = true,
                                                        Permanent = true
                                                    }
                                                }
                                            }
                                        },

                                        // Lv >= 15
                                        new Conditional
                                        {
                                            ConditionsChecker = new ConditionsChecker
                                            {
                                                Conditions = new Condition[]
                                                {
                                                    new ContextConditionCompare
                                                    {
                                                        m_Type = ContextConditionCompare.Type.GreaterOrEqual,
                                                        CheckValue = new ContextValue
                                                        {
                                                            ValueType = ContextValueType.CasterProperty,
                                                            Property = UnitProperty.Level
                                                        },
                                                        TargetValue = new ContextValue
                                                        {
                                                            ValueType = ContextValueType.Simple,
                                                            Value = 15
                                                        }
                                                    }
                                                }
                                            },
                                            IfTrue = new ActionList
                                            {
                                                Actions = new GameAction[]
                                                {
                                                    new ContextActionApplyBuff
                                                    {
                                                        m_Buff = commandingInfusionLv15AllyBuff.ToReference<BlueprintBuffReference>(),
                                                        AsChild = true,
                                                        Permanent = true
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                     };

                     c.UnitExit = new ActionList
                     {
                         Actions = new GameAction[]
                        {
                            new Conditional
                            {
                                ConditionsChecker = new ConditionsChecker
                                {
                                    Conditions = new Condition[]
                                    {
                                        new ContextConditionIsAlly()
                                    }
                                },
                                IfTrue = new ActionList
                                {
                                    Actions = new GameAction[]
                                    {
                                        new ContextActionRemoveBuff
                                        {
                                            m_Buff = commandingInfusionAllyBuff.ToReference<BlueprintBuffReference>()
                                        },
                                        new ContextActionRemoveBuff
                                        {
                                            m_Buff = commandingInfusionLv10AllyBuff.ToReference<BlueprintBuffReference>()
                                        },
                                        new ContextActionRemoveBuff
                                        {
                                            m_Buff = commandingInfusionLv15AllyBuff.ToReference<BlueprintBuffReference>()
                                        }
                                    }
                                }
                            }
                        }
                     };
                 })
                .Configure();
        }
    }
}
