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
using Kingmaker.Designers.EventConditionActionSystem.Conditions;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
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
using Kingmaker.UnitLogic.Buffs.Actions;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
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
    internal class NecromancyLevel2
    {
        private static readonly string BoneFistsUndeadBuffGuid = "6A5C534AA2ED4D849672472B193A60F6";

        private static readonly string ScareDescription = "Scare.Description";
        private static readonly string BoneshakerDescription = "Boneshaker.Description";
        public static void Configure()
        {
            BlueprintAbility scare = BlueprintTool.Get<BlueprintAbility>("08cb5f4c3b2695e44971bf5c45205df0");
            BlueprintAbility commandUndead = BlueprintTool.Get<BlueprintAbility>("0b101dd5618591e478f825f0eef155b4");
            BlueprintAbility boneshaker = BlueprintTool.Get<BlueprintAbility>("b7731c2b4fa1c9844a092329177be4c3");
            BlueprintAbility boneFists = BlueprintTool.Get<BlueprintAbility>("0da2046b4517427bb9b2e304ea6342bf");
            BlueprintAbility perniciousPoison = BlueprintTool.Get<BlueprintAbility>("dee3074b2fbfb064b80b973f9b56319e");

            BlueprintBuff shaken = BlueprintTool.Get<BlueprintBuff>("25ec6cb6ab1845c48a95f9c20b034220");
            BlueprintBuff commandUndeadBuff = BlueprintTool.Get<BlueprintBuff>("7cd727ddd4cc4be498720e45f0c1f6f4");
            BlueprintBuff commandUndeadIntelligentBuff = BlueprintTool.Get<BlueprintBuff>("07f4f8d2000a91c459c23c7fff8c74fb");
            BlueprintBuff perniciousPoisonbuff = BlueprintTool.Get<BlueprintBuff>("2d10623d79e11014c9f70db93a28554c");
            BlueprintBuff poxPustulesBuff = BlueprintTool.Get<BlueprintBuff>("a8a28fd665c3d49428463cfee78b79d1");
            BlueprintBuff ghoulTouchBuff = BlueprintTool.Get<BlueprintBuff>("bdc12558a89d47a1b4f859b2664bcef3");
            BlueprintBuff summonedCreatureSpawnMonsterIVVI = BlueprintTool.Get<BlueprintBuff>("50d51854cf6a3434d96a87d050e1d09a");

            BlueprintFeature incorporeal = BlueprintTool.Get<BlueprintFeature>("c4a7f98d743bc784c9d4cf2105852c39");
            BlueprintFeature constructType = BlueprintTool.Get<BlueprintFeature>("fd389783027d63343b4a5634bd81645f");
            BlueprintFeature undeadType = BlueprintTool.Get<BlueprintFeature>("734a29b693e9ec346ba2951b27987e33");

            BlueprintUnit ghoulCreeper = BlueprintTool.Get<BlueprintUnit>("492e9c74335b7b140bb5cab8413f7a42");

            BlueprintSummonPool summonMonsterPool = BlueprintTool.Get<BlueprintSummonPool>("d94c93e7240f10e41ae41db4c83d1cbe");

            AbilityConfigurator.For(scare)
                .SetDescription(ScareDescription)
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

            AbilityConfigurator.For(commandUndead)
                .EditComponent<AbilityEffectRunAction>(c =>
                {
                    foreach (var rootAction in c.Actions.Actions)
                    {
                        ActionTreeUtils.Walk(rootAction, a =>
                        {
                            if (a is ContextActionApplyBuff apply)
                            {
                                if (apply.m_Buff?.Guid == commandUndeadBuff.AssetGuid ||
                                    apply.m_Buff?.Guid == commandUndeadIntelligentBuff.AssetGuid)
                                {
                                    apply.DurationValue = new ContextDurationValue
                                    {
                                        Rate = DurationRate.Minutes,
                                        DiceType = DiceType.Zero,
                                        DiceCountValue = 0,
                                        BonusValue = new ContextValue
                                        {
                                            ValueType = ContextValueType.Rank,
                                            ValueRank = AbilityRankType.Default
                                        }
                                    };
                                }
                            }
                        });
                    }
                })
                .Configure();

            AbilityConfigurator.For(boneshaker)
                .SetDescription(BoneshakerDescription)
                .EditComponent<AbilityEffectRunAction>(c =>
                {
                    var cond = c.Actions.Actions
                        .OfType<Conditional>()
                        .FirstOrDefault();

                    if (cond == null) return;

                    cond.ConditionsChecker = new ConditionsChecker
                    {
                        Operation = Operation.Or,
                        Conditions = new Condition[]
                        {
                            new ContextConditionHasFact
                            {
                                m_Fact = incorporeal.ToReference<BlueprintUnitFactReference>(),
                                Not = true
                            },
                            new ContextConditionHasFact
                            {
                                m_Fact = constructType.ToReference<BlueprintUnitFactReference>(),
                                Not = true
                            }
                        }
                    };
                })
                .Configure();

            var rankConfig1 = ContextRankConfigs.CasterLevel().WithDiv2Progression();
            rankConfig1.m_Max = 10;

            BlueprintBuff boneFistsUndeadBuff = BuffConfigurator.New("BoneFistsUndeadBuff", BoneFistsUndeadBuffGuid)
                .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                .AddContextStatBonus(
                    stat: Kingmaker.EntitySystem.Stats.StatType.AC,
                    descriptor: ModifierDescriptor.NaturalArmor,
                    value: 2
                )
                .AddContextRankConfig(
                    rankConfig1
                )
                .AddContextCalculateSharedValue(
                    valueType: AbilitySharedValue.Damage,
                    value: new ContextDiceValue
                    {
                        DiceType = DiceType.D6,
                        DiceCountValue = 1,
                        BonusValue = new ContextValue
                        {
                            ValueType = ContextValueType.Rank
                        }
                    }
                )
                .AddTemporaryHitPointsFromAbilityValue(
                    value: new ContextValue
                    {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.Damage
                    }
                )
                .Configure();

            AbilityConfigurator.For(boneFists)
                .EditComponent<AbilityEffectRunAction>(c =>
                {
                    var list = c.Actions.Actions.ToList();

                    list.Add(new Conditional
                    {
                        ConditionsChecker = new ConditionsChecker
                        {
                            Operation = Operation.And,
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
                                    m_Buff = boneFistsUndeadBuff.ToReference<BlueprintBuffReference>(),
                                    DurationValue = new ContextDurationValue
                                    {
                                        Rate = DurationRate.Minutes,
                                        DiceType = DiceType.Zero,
                                        DiceCountValue = 0,
                                        BonusValue = new ContextValue
                                        {
                                            ValueType = ContextValueType.Rank,
                                            ValueRank = AbilityRankType.Default
                                        }
                                    }
                                }
                            }
                        },
                        IfFalse = new ActionList
                        {
                            Actions = Array.Empty<GameAction>()
                        }
                    });


                    c.Actions.Actions = list.ToArray();
                })
                .Configure();

            AbilityConfigurator.For(perniciousPoison)
                .SetRange(AbilityRange.Medium)
                .Configure();

            BuffConfigurator.For(perniciousPoisonbuff)
                .AddContextStatBonus(
                    stat: StatType.SaveFortitude,
                    descriptor: ModifierDescriptor.Penalty,
                    value: -2
                )
                .AddComponent<InitiatorSavingThrowTrigger>(c =>
                {
                    c.OnFailedSave = new ActionList
                    {
                        Actions = new GameAction[]
                        {
                        new DealStatDamage
                        {
                            Stat = StatType.Constitution,
                            DamageDice = new DiceFormula
                            {
                                m_Dice=DiceType.D3,
                                m_Rolls=1
                            },
                        }
                        }
                    };

                    c.OnSuccessfulSave = new ActionList(); 
                })
                .Configure();

            BuffConfigurator.For(poxPustulesBuff)
                .RemoveComponents(c => c is AddStatBonus)
                .AddContextRankConfig(
                    ContextRankConfigs
                        .CasterLevel()
                        .WithDiv2Progression()
                )
                .AddContextCalculateSharedValue(
                    valueType: AbilitySharedValue.Damage,
                    value: new ContextDiceValue
                    {
                        DiceType = DiceType.One,
                        DiceCountValue = 4,
                        BonusValue = new ContextValue
                        {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.Default
                        }
                    },
                    modifier: -1
                )
                .AddFactContextActions(
                    activated: new ActionList
                    {
                        Actions = new GameAction[]
                        {
                            new ContextActionSavingThrow
                            {
                                Type = SavingThrowType.Fortitude,
                                Actions = new ActionList
                                {
                                    Actions = new GameAction[]
                                    {
                                        new ContextActionConditionalSaved
                                        {
                                            Failed = new ActionList
                                            {
                                                Actions = new GameAction[]
                                                {
                                                    new BuffActionAddStatBonus
                                                    {
                                                        Stat = StatType.Dexterity,
                                                        Descriptor = ModifierDescriptor.Penalty,
                                                        Value = new ContextValue
                                                        {
                                                            ValueType = ContextValueType.Shared,
                                                            ValueRank = AbilityRankType.Default,
                                                        }
                                                    }
                                                }
                                            },

                                            Succeed = new ActionList
                                            {
                                                Actions = new GameAction[]
                                                {
                                                    new ContextActionChangeSharedValue
                                                    { 
                                                        SharedValue = AbilitySharedValue.Damage,
                                                        Type = SharedValueChangeType.Div2
                                                    },

                                                    new BuffActionAddStatBonus
                                                    {
                                                        Stat = StatType.Dexterity,
                                                        Descriptor = ModifierDescriptor.Penalty,
                                                        Value = new ContextValue
                                                        {
                                                            ValueType = ContextValueType.Shared,
                                                            ValueRank = AbilityRankType.Default,
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }                            
                        }
                    },
                    deactivated: new ActionList()
                )
                .Configure();

            BuffConfigurator.For(ghoulTouchBuff)
              .AddContextRankConfig(
                  ContextRankConfigs.CasterLevel()
              )
              .AddComponent<AddIncomingDamageTrigger>(c =>
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
                                        new ContextConditionHasFact
                                        {
                                            m_Fact = undeadType.ToReference<BlueprintUnitFactReference>(),
                                            Not = true
                                        },
                                        new ContextConditionHasFact
                                        {
                                            m_Fact = constructType.ToReference<BlueprintUnitFactReference>(),
                                            Not = true
                                        },
                                        new ContextConditionHasFact
                                        {
                                            m_Fact = ghoulTouchBuff.ToReference<BlueprintUnitFactReference>(),
                                            Not = false
                                        }
                                  }
                              },
                              IfTrue = new ActionList
                              {
                                  Actions = new GameAction[]
                                  {
                                      new ContextActionSpawnMonster
                                      {
                                          m_Blueprint = ghoulCreeper.ToReference<BlueprintUnitReference>(),
                                          m_SummonPool = summonMonsterPool.ToReference<BlueprintSummonPoolReference>(),
                                          CountValue = new ContextDiceValue
                                          {
                                              DiceType = DiceType.Zero,
                                              DiceCountValue = 0,
                                              BonusValue = new ContextValue
                                              {
                                                ValueType = ContextValueType.Simple,
                                                Value = 1
                                              }
                                          },
                                          DurationValue = new ContextDurationValue
                                          {
                                              Rate = DurationRate.Rounds,
                                              DiceType = DiceType.Zero,
                                              DiceCountValue = 0,
                                              BonusValue = new ContextValue
                                              {
                                                  ValueType = ContextValueType.Rank,
                                                  ValueRank = AbilityRankType.Default
                                              }
                                          },
                                          AfterSpawn = new ActionList
                                          {
                                              Actions = new GameAction[]
                                              {
                                                  new ContextActionApplyBuff
                                                  {
                                                      m_Buff = summonedCreatureSpawnMonsterIVVI.ToReference<BlueprintBuffReference>()
                                                  }
                                              } 
                                          } 
                                      }
                                  }
                              }
                          }
                      }
                  };
                  c.CompareType = 0;
                  c.TargetValue = 0;
              })
              .Configure();

        }
    }
}
