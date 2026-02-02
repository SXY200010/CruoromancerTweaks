using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using CruoromancerTweaks.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.ElementsSystem;
using Kingmaker.Enums;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.Abilities.Components.CasterCheckers;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruoromancerTweaks.ModifiedContent.Spells.Necromancy
{
    internal class NecromancyLevel1
    {
        private static readonly string CauseFearDescription = "CauseFear.Description";
        private static readonly string RayOfSickeningDescription = "RayOfSickening.Description";

        public static void Configure()
        {
            BlueprintAbility causeFear = BlueprintTool.Get<BlueprintAbility>("bd81a3931aa285a4f9844585b5d97e51");
            BlueprintAbility rayOfSickening = BlueprintTool.Get<BlueprintAbility>("fa3078b9976a5b24caf92e20ee9c0f54");
            BlueprintAbility rayOfEnfeeblement = BlueprintTool.Get<BlueprintAbility>("450af0402422b0b4980d9c2175869612");
            BlueprintBuff shaken = BlueprintTool.Get<BlueprintBuff>("25ec6cb6ab1845c48a95f9c20b034220");
            BlueprintBuff sickened = BlueprintTool.Get<BlueprintBuff>("4e42460798665fd4cb9173ffa7ada323");
            BlueprintBuff nauseated = BlueprintTool.Get<BlueprintBuff>("956331dba5125ef48afe41875a00ca0e");

            AbilityConfigurator.For(causeFear)
                .SetDescription(CauseFearDescription)
                .EditComponent<AbilityEffectRunAction>(c =>
                {
                    foreach (var rootAction in c.Actions.Actions)
                    {
                        ActionTreeUtils.Walk(rootAction, a =>
                        {
                            if (a is ContextActionConditionalSaved saved)
                            {
                                foreach (var succeed in saved.Succeed.Actions)
                                {
                                    if (succeed is ContextActionApplyBuff apply &&
                                        apply.m_Buff?.Guid == shaken.AssetGuid)
                                    {
                                        apply.DurationValue = new ContextDurationValue
                                        {
                                            Rate = DurationRate.Rounds,
                                            DiceType = DiceType.D4,
                                            DiceCountValue = 1,
                                            BonusValue = 0
                                        };
                                    }
                                }
                            }
                        });
                    }
                })
                .Configure();

            AbilityConfigurator.For(rayOfEnfeeblement)
                .AddToAvailableMetamagic(Metamagic.Empower)
                .AddToAvailableMetamagic(Metamagic.Maximize)
                .AddToAvailableMetamagic(Metamagic.Intensified)
                .Configure();

            AbilityConfigurator.For(rayOfSickening)
                .SetDescription(RayOfSickeningDescription)
                .EditComponent<AbilityEffectRunAction>(c =>
                {
                    foreach (var action in c.Actions.Actions)
                    {
                        if (action is ContextActionConditionalSaved saved)
                        {
                            saved.Failed = new ActionList
                            {
                                Actions = new GameAction[]
                                {
                                    new Conditional
                                    {
                                        ConditionsChecker = new ConditionsChecker
                                        {
                                            Conditions = new Condition[]
                                            {
                                                new ContextConditionHasFact
                                                {
                                                    m_Fact = sickened
                                                        .ToReference<BlueprintUnitFactReference>(),
                                                    Not = false
                                                }
                                            }
                                        },

                                        IfTrue = new ActionList
                                        {
                                            Actions = new GameAction[]
                                            {
                                                new ContextActionApplyBuff
                                                {
                                                    m_Buff = nauseated.ToReference<BlueprintBuffReference>(),

                                                    DurationValue = new ContextDurationValue
                                                    {
                                                        Rate = DurationRate.Minutes,
                                                        DiceType = DiceType.Zero,
                                                        DiceCountValue = 0,
                                                        BonusValue = new ContextValue
                                                        {
                                                            ValueType = ContextValueType.Rank
                                                        }
                                                    }
                                                }
                                            }
                                        },

                                        IfFalse = new ActionList
                                        {
                                            Actions = new GameAction[]
                                            {
                                                new ContextActionApplyBuff
                                                {
                                                    m_Buff = sickened.ToReference<BlueprintBuffReference>(),

                                                    DurationValue = new ContextDurationValue
                                                    {
                                                        Rate = DurationRate.Minutes,
                                                        DiceType = DiceType.Zero,
                                                        DiceCountValue = 0,
                                                        BonusValue = new ContextValue
                                                        {
                                                            ValueType = ContextValueType.Rank
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            };
                        }
                    }
                })
                .Configure();
        }

    }
}
