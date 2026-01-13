using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.ElementsSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.CasterCheckers;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using System.Collections.Generic;

namespace CruoromancerTweaks.ModifiedContent.Classes
{
    internal class ProfaneInfusion
    {
        private static readonly string Description = "ProfaneInfusion.Description";
        private static readonly string Name = "ProfaneInfusion.Name";
        private static readonly string CooldownName = "SickeningInfusionCooldown.Name";
        private static readonly string Icon = "assets/icons/turnundead.png";

        private const string ProfaneInfusionFeatureGuid = "856776AC66A44E6381D303E223E48BC1";
        private const string ProfaneInfusionAbilityGuid = "FFAB8FD6374244E29381D29C05004233";
        private const string ProfaneInfusionBuffGuid = "0DBB86E41CAD4010BFD4619CC288EC75";
        private const string ProfaneInfusionLv20BuffGuid = "012F7DE40AE64CBB9157851117742B89";
        private const string ProfaneInfusionCooldownBuffGuid = "2615F6BD0B1D4D72B289A05937D6229E";
        public static void Configure()
        {
            BlueprintFeature perfectInfusion = BlueprintTool.Get<BlueprintFeature>("74686a88b35946219aa72455c50a15b0");

            BlueprintBuff cooldownBuff =
                BuffConfigurator.New(
                        "ProfaneInfusionCooldown",
                        ProfaneInfusionCooldownBuffGuid
                    )
                    .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                    .SetStacking(StackingType.Replace)
                    .SetDisplayName(CooldownName)
                    .Configure();

            BlueprintBuff profaneInfusionBuff =
                BuffConfigurator.New("ProfaneInfusionBuff", ProfaneInfusionBuffGuid)
                .SetDescription(Description)
                .SetDisplayName(Name)
                .SetIcon(Icon)
                .AddIncreaseSpellSchoolCasterLevel(
                    bonusLevel: 2,
                    school: SpellSchool.Necromancy,
                    descriptor: null
                )
                .Configure();

            BlueprintBuff profaneInfusionLv20Buff =
                BuffConfigurator.New("ProfaneInfusionLv20Buff", ProfaneInfusionLv20BuffGuid)
                .SetDescription(Description)
                .SetDisplayName(Name)
                .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                .AddIncreaseSpellSchoolCasterLevel(
                    bonusLevel: 2,
                    school: SpellSchool.Necromancy,
                    descriptor: null
                )
                .Configure();

            BlueprintAbility profaneInfusionAbility =
                AbilityConfigurator.New("ProfaneInfusionAbility", ProfaneInfusionAbilityGuid)
                .SetDisplayName(Name)
                .SetDescription(Description)
                .SetActionType(UnitCommand.CommandType.Free)
                .SetIcon(Icon)
                .AddComponent<AbilityCasterHasNoFacts>(c =>
                {
                    c.m_Facts = new BlueprintUnitFactReference[]
                    {
                           cooldownBuff.ToReference<BlueprintUnitFactReference>()
                    };
                })
                .AddComponent<AbilityEffectRunAction>(c =>
                {
                    c.Actions = new ActionList
                    {
                        Actions = new GameAction[]
                        {
                            new ContextActionOnContextCaster
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
                            },
                            new ContextActionOnContextCaster
                            {
                                Actions = new ActionList
                                {
                                    Actions = new GameAction[]
                                    {
                                        new ContextActionApplyBuff
                                        {
                                            m_Buff = profaneInfusionBuff.ToReference<BlueprintBuffReference>(),
                                            ToCaster = true,
                                            DurationValue = new ContextDurationValue
                                            {
                                                DiceType = DiceType.Zero,
                                                DiceCountValue = 0,
                                                BonusValue = 2,
                                                Rate = DurationRate.Rounds
                                            }
                                        }
                                    }
                                }
                            },

                            new Conditional
                            {
                                ConditionsChecker = new ConditionsChecker
                                {
                                    Operation = Operation.And,
                                    Conditions = new Condition[]
                                    {
                                        new ContextConditionCasterHasFact
                                        {
                                            m_Fact = perfectInfusion.ToReference<BlueprintUnitFactReference>(),
                                            Not = false
                                        }
                                    }
                                },

                                IfTrue = new ActionList
                                {
                                    Actions = new GameAction[]
                                    {
                                        new ContextActionOnContextCaster
                                        {
                                            Actions = new ActionList
                                            {
                                                Actions = new GameAction[]
                                                {
                                                    new ContextActionApplyBuff
                                                    {
                                                        m_Buff = profaneInfusionLv20Buff.ToReference<BlueprintBuffReference>(),
                                                        ToCaster = true,
                                                        DurationValue = new ContextDurationValue
                                                        {
                                                            DiceType = DiceType.Zero,
                                                            DiceCountValue = 0,
                                                            BonusValue = 2,
                                                            Rate = DurationRate.Rounds
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },

                                IfFalse = new ActionList
                                {
                                    Actions = new GameAction[]
                                    {
                                        new ContextActionOnContextCaster
                                        {
                                            Actions = new ActionList
                                            {
                                                Actions = new GameAction[]
                                                {
                                                    new ContextActionDealDamage
                                                    {
                                                        Value = new ContextDiceValue
                                                        {
                                                            DiceType = DiceType.D4,
                                                            DiceCountValue = 1,
                                                            BonusValue = new ContextValue
                                                            {
                                                                ValueType = ContextValueType.Rank
                                                            }
                                                        },
                                                        DamageType = new DamageTypeDescription
                                                        {
                                                            Type = DamageType.Untyped
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    };
                })

                //.AddComponent<AbilityExecuteActionOnCast>(c =>
                //{
                //    c.Actions = new ActionList
                //    {
                //        Actions = new GameAction[]
                //        {
                //            new Conditional
                //            {
                //                ConditionsChecker = new ConditionsChecker
                //                {
                //                    Operation = Operation.And,
                //                    Conditions = new Condition[]
                //                    {
                //                        new ContextConditionCasterHasFact
                //                        {
                //                            m_Fact = perfectInfusion.ToReference<BlueprintUnitFactReference>(),
                //                            Not = false
                //                        }
                //                    }
                //                },

                //                IfTrue = new ActionList
                //                {
                //                    Actions = new GameAction[]
                //                    {
                //                        new ContextActionOnContextCaster
                //                        {
                //                            Actions = new ActionList
                //                            {
                //                                Actions = new GameAction[]
                //                                {
                //                                    new ContextActionApplyBuff
                //                                    {
                //                                        m_Buff = profaneInfusionLv20Buff.ToReference<BlueprintBuffReference>(),
                //                                        ToCaster = true,
                //                                        DurationValue = new ContextDurationValue
                //                                        {
                //                                            DiceType = DiceType.Zero,
                //                                            DiceCountValue = 0,
                //                                            BonusValue = 2,
                //                                            Rate = DurationRate.Rounds
                //                                        }
                //                                    }
                //                                }
                //                            }
                //                        }
                //                    }
                //                },

                //                IfFalse = new ActionList
                //                {
                //                    Actions = new GameAction[]
                //                    {
                //                        new ContextActionOnContextCaster
                //                        {
                //                            Actions = new ActionList
                //                            {
                //                                Actions = new GameAction[]
                //                                {
                //                                    new ContextActionDealDamage
                //                                    {
                //                                        Value = new ContextDiceValue
                //                                        {
                //                                            DiceType = DiceType.D4,
                //                                            DiceCountValue = 1,
                //                                            BonusValue = new ContextValue
                //                                            {
                //                                                ValueType = ContextValueType.Rank
                //                                            }
                //                                        },
                //                                        DamageType = new DamageTypeDescription
                //                                        {
                //                                            Type = DamageType.Untyped
                //                                        }
                //                                    }
                //                                }
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    };
                //})
                .Configure();

            BlueprintFeature profaneInfusionFeature =
                FeatureConfigurator.New("ProfaneInfusionFeature", ProfaneInfusionFeatureGuid)
                .SetDisplayName(Name)
                .SetDescription(Description)
                .SetIcon(Icon)
                .AddFacts(new List<Blueprint<BlueprintUnitFactReference>>
                {
                    profaneInfusionAbility
                })
                .Configure();

        }
    }
}
