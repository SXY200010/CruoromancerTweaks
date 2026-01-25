using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.ElementsSystem;
using Kingmaker.Enums;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
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
    internal class Necromancy
    {
        private const string TurnUndeadNecromancyGuid = "71b8898b1d26d654b9a3eeac87e3e2f8";
        private const string UndeadTouchGuid = "52CBAAC3AFF340329BFE6D9B405A2568";
        private const string UndeadTouchBuffGuid = "2FF79C42E2C445C48982F8F511CE7E72";
        private const string UndeadTouchResourceGuid = "ABDFA3B5E84549E39D2F6D6C2F0B2D18";
        private const string WizardClassGuid = "ba34257984f4c41408ce1dc2004e342e";
        private const string DeadSightAreaGuid = "A0B966FE43FA4A53BFA291863EC23DB6";
        private const string DeadSightArea2Guid = "DC6C129F82DC422EA35267AA60089911";
        private const string DeadSightSelfBuffGuid = "CA59AB38FE754DED8175CECFA3790A6E";
        private const string DeadSightUndeadAllyBuffGuid = "EB7D0EF2165249328836A9BEAB2A4573";
        private const string DeadSightLivingEnemyDebuffGuid = "A369500B100B449B977BF994D7BED363";

        private static readonly string TurnUndeadNecromancyDescription = "TurnUndeadNecromancy.Description";
        private static readonly string UndeadTouchName = "UndeadTouch.Name";
        private static readonly string UndeadTouchDescription = "UndeadTouch.Description";
        private static readonly string DeadSightName = "DeadSight.Name";
        private static readonly string DeadSightDescription = "DeadSight.Description";
        private static readonly string SpecialisationSchoolNecromancyDescription = "SpecialisationSchoolNecromancy.Description";
        public static void Configure()
        {
            BlueprintAbility turnUndeadNecromancy = BlueprintTool.Get<BlueprintAbility>(TurnUndeadNecromancyGuid);
            BlueprintFeature undeadType = BlueprintTool.Get<BlueprintFeature>("734a29b693e9ec346ba2951b27987e33");
            BlueprintFeature necromancySchoolBaseFeature = BlueprintTool.Get<BlueprintFeature>("927707dce06627d4f880c90b5575125f");
            BlueprintProgression specialisationSchoolNecromancyProgression = BlueprintTool.Get<BlueprintProgression>("e9450978cc9feeb468fb8ee3a90607e3");
            BlueprintAbility necromancySchoolBaseAbility = BlueprintTool.Get<BlueprintAbility>("39af648796b7b9b4ab6321898ebb5fff");
            BlueprintFeature necromancySchoolGreaterFeature = BlueprintTool.Get<BlueprintFeature>("82371e899df830e4bb955429d89b755c");
            BlueprintAbilityResource necromancySchoolBaseResource = BlueprintTool.Get<BlueprintAbilityResource>("d3c8231b4ab43d248944b6da83776522");
            BlueprintActivatableAbility necromancySchoolGreaterAbility = BlueprintTool.Get<BlueprintActivatableAbility>("23929ea35519488459ed30eea4425a04");
            BlueprintFeature constructType = BlueprintTool.Get<BlueprintFeature>("fd389783027d63343b4a5634bd81645f");
            
            AbilityConfigurator.For(turnUndeadNecromancy)
            .SetDescription(TurnUndeadNecromancyDescription) 
            .SetDescriptionShort(TurnUndeadNecromancyDescription)
            .AddContextRankConfig(
                ContextRankConfigs
                    .ClassLevel(new[] { "ba34257984f4c41408ce1dc2004e342e" }) // Wizard
                    .WithDiv2Progression()
            )
            .EditComponent<AbilityEffectRunAction>(c =>
            {
                var list = c.Actions.Actions.ToList();

                list.Add(
                    new Conditional
                    {
                        ConditionsChecker = new ConditionsChecker
                        {
                            Conditions = new Condition[]
                            {
                                new ContextConditionIsEnemy(),
                                new ContextConditionHasFact
                                {
                                     m_Fact = undeadType.ToReference<BlueprintUnitFactReference>()
                                }
                            }
                        },
                        IfTrue = new ActionList
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
                                            ValueType = ContextValueType.CasterProperty,
                                            Property = UnitProperty.StatBonusIntelligence
                                        }
                                    },
                                    DamageType = new DamageTypeDescription
                                    {
                                        Type = DamageType.Untyped
                                    }
                                },
                                new ContextActionDealDamage
                                {
                                    Value = new ContextDiceValue
                                    {
                                        DiceType = DiceType.Zero,
                                        DiceCountValue = 0,
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
                );

                c.Actions.Actions = list.ToArray();
            })
            .Configure();

            BlueprintAbilityResource undeadTouchResource =
                AbilityResourceConfigurator.New("UndeadTouchResource", UndeadTouchResourceGuid)
                .CopyFrom(necromancySchoolBaseResource)
                .SetIsDomainAbility()
                .Configure();

            var rankConfig1 = ContextRankConfigs.ClassLevel(new[] { WizardClassGuid })
                .WithDivStepProgression(5);
            rankConfig1.m_Type = AbilityRankType.StatBonus;

            var rankConfig2 = ContextRankConfigs.ClassLevel(new[] { WizardClassGuid });
            rankConfig2.m_Type = AbilityRankType.DamageBonus;

            BlueprintBuff undeadTouchBuff = 
                 BuffConfigurator.New("UndeadTouchBuff", UndeadTouchBuffGuid)
                .SetDisplayName(UndeadTouchName)
                .SetDescription(UndeadTouchDescription)
                .AddContextRankConfig(
                    rankConfig1
                )
                .AddContextRankConfig(
                    rankConfig2
                )
                .AddContextStatBonus(
                    stat: Kingmaker.EntitySystem.Stats.StatType.AdditionalAttackBonus,
                    descriptor: ModifierDescriptor.Profane,
                    value: new ContextValue
                    {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    }
                )
                .AddContextStatBonus(
                    stat: Kingmaker.EntitySystem.Stats.StatType.SaveFortitude,
                    descriptor: ModifierDescriptor.Profane,
                    value: new ContextValue
                    {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    }
                )
                .AddContextStatBonus(
                    stat: Kingmaker.EntitySystem.Stats.StatType.SaveReflex,
                    descriptor: ModifierDescriptor.Profane,
                    value: new ContextValue
                    {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    }
                )
                .AddContextStatBonus(
                    stat: Kingmaker.EntitySystem.Stats.StatType.SaveWill,
                    descriptor: ModifierDescriptor.Profane,
                    value: new ContextValue
                    {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    }
                )
                .AddTemporaryHitPointsFromAbilityValue(
                    value: new ContextValue
                    {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.DamageBonus
                    }
                )
                .SetIcon(necromancySchoolBaseAbility.Icon)
                .Configure();

            BlueprintAbility undeadTouch =
                AbilityConfigurator.New("UndeadTouch", UndeadTouchGuid)
                .SetDisplayName(UndeadTouchName)
                .SetDescription(UndeadTouchDescription)
                .SetIcon(necromancySchoolBaseAbility.Icon)
                .SetCanTargetEnemies(false)
                .SetCanTargetFriends(true)
                .SetCanTargetSelf(false)
                .AddContextRankConfig(
                    ContextRankConfigs
                        .ClassLevel(new[] { WizardClassGuid })
                        .WithDiv2Progression()
                )
                .AddAbilityResourceLogic(
                    requiredResource: undeadTouchResource,
                    amount: 1,
                    isSpendResource: true
                )
                .AddComponent<AbilityEffectRunAction>(c =>
                {
                    c.Actions = new ActionList
                    {
                        Actions = new GameAction[]
                        {
                            new Conditional
                            {
                                ConditionsChecker = new ConditionsChecker
                                {
                                    Conditions = new Condition[]
                                    {
                                        new ContextConditionIsAlly(),
                                        new ContextConditionHasFact
                                        {
                                            m_Fact = undeadType.ToReference<BlueprintUnitFactReference>()
                                        }
                                    }
                                },
                                IfTrue = new ActionList
                                {
                                    Actions = new GameAction[]
                                    {
                                        new ContextActionApplyBuff
                                        {
                                            m_Buff = undeadTouchBuff.ToReference<BlueprintBuffReference>(),
                                            ToCaster = false,
                                            DurationValue = new ContextDurationValue
                                            {
                                                Rate = DurationRate.Rounds,
                                                DiceType = DiceType.One,
                                                DiceCountValue = new ContextValue
                                                {
                                                    ValueType = ContextValueType.Rank
                                                },
                                                BonusValue = 0
                                            },
                                        }
                                    }
                                }
                            }
                        }
                    };
                })
                .Configure();

            FeatureConfigurator.For(necromancySchoolBaseFeature)
                .SetDescription(SpecialisationSchoolNecromancyDescription)
                 .AddAbilityResources(
                    resource: undeadTouchResource,
                    restoreAmount: true
                 )
                 .EditComponent<AddFacts>(c =>
                 {
                     c.m_Facts = new BlueprintUnitFactReference[]
                     {
                         undeadTouch.ToReference<BlueprintUnitFactReference>(),
                         turnUndeadNecromancy.ToReference<BlueprintUnitFactReference>()
                     };
                 })
                .Configure();

            ProgressionConfigurator.For(specialisationSchoolNecromancyProgression)
                .SetDescription(SpecialisationSchoolNecromancyDescription)
                .Configure();

            BlueprintBuff undeadAllyBuff =
                BuffConfigurator.New("DeadSightUndeadAllyBuff", DeadSightUndeadAllyBuffGuid)
                .SetIcon(necromancySchoolGreaterAbility.Icon)
                .AddContextStatBonus(
                    stat:Kingmaker.EntitySystem.Stats.StatType.AdditionalAttackBonus,
                    descriptor: ModifierDescriptor.Profane,
                    value: 1
                )
                .AddBuffAllSavesBonus(ModifierDescriptor.Profane, 1)
                .AddContextStatBonus(
                    stat: Kingmaker.EntitySystem.Stats.StatType.AdditionalDamage,
                    descriptor: ModifierDescriptor.Profane,
                    value: 1
                )
                .AddBuffAbilityRollsBonus(
                    affectAllStats: true,
                    descriptor: ModifierDescriptor.Profane,
                    value: -1
                )
                .Configure();

            BlueprintBuff livingEnemyDebuff =
                BuffConfigurator.New("DeadSightLivingEnemyDebuff", DeadSightLivingEnemyDebuffGuid)
                .SetIcon(necromancySchoolGreaterAbility.Icon)
                .AddStatBonus(
                    stat: Kingmaker.EntitySystem.Stats.StatType.AdditionalAttackBonus,
                    descriptor: ModifierDescriptor.None,
                    value: -1
                )
                .AddBuffAllSavesBonus(ModifierDescriptor.None, -1)
                .AddStatBonus(
                    stat: Kingmaker.EntitySystem.Stats.StatType.AdditionalDamage,
                    descriptor: ModifierDescriptor.None,
                    value: -1
                )
                .AddBuffAbilityRollsBonus(
                    affectAllStats: true,
                    descriptor: ModifierDescriptor.None,
                    value: -1
                )
                .Configure();

            BlueprintAbilityAreaEffect deadSightArea =
                AbilityAreaEffectConfigurator.New("DeadSightArea", DeadSightAreaGuid)
                    .SetSize(30.Feet())
                    .SetShape(AreaEffectShape.Cylinder)                    
                    .SetTargetType(BlueprintAbilityAreaEffect.TargetType.Any)
                    .SetAffectEnemies(true)
                    .SetAggroEnemies(true)
                    .AddComponent<AbilityAreaEffectRunAction>(c =>
                    {
                        c.UnitEnter = new ActionList
                        {
                            Actions = new GameAction[]
                            {
                                new Conditional
                                {
                                    ConditionsChecker = new ConditionsChecker
                                    {
                                        Conditions = new Condition[]
                                        {
                                            new ContextConditionIsAlly(),
                                            new ContextConditionHasFact
                                            {
                                                m_Fact = undeadType.ToReference<BlueprintUnitFactReference>()
                                            }
                                        }
                                    },
                                    IfTrue = new ActionList
                                    {
                                        Actions = new GameAction[]
                                        {
                                            new ContextActionApplyBuff
                                            {
                                                m_Buff = undeadAllyBuff.ToReference<BlueprintBuffReference>(),
                                                Permanent = true
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
                                new ContextActionRemoveBuff
                                {
                                    m_Buff = undeadAllyBuff.ToReference<BlueprintBuffReference>()
                                }
                            }
                        };
                    })                    
                    .Configure();

            BlueprintAbilityAreaEffect deadSightArea2 =
                AbilityAreaEffectConfigurator.New("DeadSightArea2", DeadSightArea2Guid)
                    .SetSize(30.Feet())
                    .SetShape(AreaEffectShape.Cylinder)                    
                    .SetTargetType(BlueprintAbilityAreaEffect.TargetType.Any)
                    .SetAffectEnemies(true)
                    .SetAggroEnemies(true)                    
                    .AddComponent<AbilityAreaEffectRunAction>(c =>
                    {
                        c.UnitEnter = new ActionList
                        {
                            Actions = new GameAction[]
                            {
                                new Conditional
                                {
                                    ConditionsChecker = new ConditionsChecker
                                    {
                                        Conditions = new Condition[]
                                        {
                                            new ContextConditionIsEnemy(),
                                            new ContextConditionHasFact
                                            {
                                                m_Fact = undeadType.ToReference<BlueprintUnitFactReference>(),
                                                Not = true
                                            },
                                            new ContextConditionHasFact
                                            {
                                                m_Fact = constructType.ToReference<BlueprintUnitFactReference>(),
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
                                                m_Buff = livingEnemyDebuff.ToReference<BlueprintBuffReference>(),
                                                Permanent = true
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
                                new ContextActionRemoveBuff
                                {
                                    m_Buff = livingEnemyDebuff.ToReference<BlueprintBuffReference>()
                                }
                            }
                        };
                    })
                    .Configure();

            BlueprintBuff deadSightSelfBuff =
                BuffConfigurator.New("DeadSightSelfBuff", DeadSightSelfBuffGuid)
                    .AddBlindsense(30.Feet()) 
                    .AddAreaEffect(deadSightArea.ToReference<BlueprintAbilityAreaEffectReference>())
                    .AddAreaEffect(deadSightArea2.ToReference<BlueprintAbilityAreaEffectReference>())
                    .Configure();

            ActivatableAbilityConfigurator.For(necromancySchoolGreaterAbility)
                .SetBuff(deadSightSelfBuff)
                .SetDisplayName(DeadSightName)
                .SetDescription(DeadSightDescription)
                .Configure();

            FeatureConfigurator.For(necromancySchoolGreaterFeature)
                .SetDisplayName(DeadSightName)
                .SetDescription(DeadSightDescription)
                .Configure();
        }
    }
}
