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
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.ResourceLinks;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.Abilities.Components.Base;
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
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kingmaker.EntitySystem.Properties.BaseGetter.PropertyContextAccessor;

namespace CruoromancerTweaks.ModifiedContent.Spells.Necromancy
{
    internal class NecromancyLevel4
    {
        private const string HorrifiedThisRoundBuff = "CD36AAA4CDAA4E62BECCAD90B94E3D3A";
        private const string HorrifyingCurseBuff = "0E09954923CE478F8EA0AD4AA4665866";
        private const string HorrifyingCurse = "BCE2C17F062C409881FEB00C6D80C7E2";

        private static readonly string BestowCurse = "BestowCurse.Description"; 
        private static readonly string HorrifyingCurseName = "HorrifyingCurse.Name";
        private static readonly string HorrifyingCurseDescription = "HorrifyingCurse.Description";
        private static readonly string ContagionDescription = "Contagion.Description";
        private static readonly string BoneshatterDescription = "Boneshatter.Description";
        private static readonly string PoisonCastDescription = "PoisonCast.Description";
        private static readonly string PoisonDescription = "Poison.Description";

        public static void Configure()
        {
            BlueprintAbility bestowCurse = BlueprintTool.Get<BlueprintAbility>("989ab5c44240907489aba0a8568d0603");
            BlueprintAbility BestowCurseFeebleBody = BlueprintTool.Get<BlueprintAbility>("0c853a9f35a7bf749821ebe5d06fade7");
            BlueprintAbility BestowCurseIdiocy = BlueprintTool.Get<BlueprintAbility>("a9c4c2552b9a9564e8ee2eb3b20cc66d");
            BlueprintAbility BestowCurseWeakness = BlueprintTool.Get<BlueprintAbility>("f428939d629fa9942a55e1a144edd294");
            BlueprintAbility BestowCurseDeterioration = BlueprintTool.Get<BlueprintAbility>("69851cc3b821c2d479ac1f2d86e8ffa5");

            BlueprintAbility[] bestowCurses = new BlueprintAbility[] { BestowCurseFeebleBody, BestowCurseIdiocy, BestowCurseWeakness, BestowCurseDeterioration };

            BlueprintBuff horrifiedThisRoundBuff = BuffConfigurator.New("HorrifiedThisRoundBuff", HorrifiedThisRoundBuff)
                .AddCondition(Kingmaker.UnitLogic.UnitCondition.Frightened)
                .AddCondition(Kingmaker.UnitLogic.UnitCondition.MovementBan)
                .Configure();

            BlueprintBuff horrifyingCurseBuff = BuffConfigurator.New("HorrifyingCurseBuff", HorrifyingCurseBuff)
                .SetIcon(bestowCurse.Icon)
                .SetDisplayName(HorrifyingCurseName)
                .SetDescription(HorrifyingCurseDescription)
                .AddFactContextActions(
                    newRound: new ActionList
                    {
                        Actions = new GameAction[]
                        {
                            new ContextActionRandomize
                            {
                                m_Actions = new ContextActionRandomize.ActionWrapper[]
                                {
                                    new ContextActionRandomize.ActionWrapper
                                    {
                                        Weight = 1,
                                        Action = new ActionList()
                                    },
                                    new ContextActionRandomize.ActionWrapper
                                    {
                                        Weight = 1,
                                        Action = new ActionList
                                        {
                                            Actions = new GameAction[]
                                            {
                                                new ContextActionApplyBuff
                                                {
                                                    m_Buff = horrifiedThisRoundBuff
                                                        .ToReference<BlueprintBuffReference>(),
                                                    DurationValue = new ContextDurationValue
                                                    {
                                                        Rate = DurationRate.Rounds,
                                                        DiceType = DiceType.Zero,
                                                        DiceCountValue = 0,
                                                        BonusValue = 1
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                )
                .Configure();

            BlueprintAbility BestowCurseHorrifying = AbilityConfigurator.New("HorrifyingCurse", HorrifyingCurse)
                .CopyFrom(BestowCurseFeebleBody, typeof(SpellComponent), typeof(SpellDescriptorComponent), typeof(AbilitySpawnFx))
                .SetDisplayName(HorrifyingCurseName)
                .SetDescription(HorrifyingCurseDescription)
                .SetRange(AbilityRange.Close)
                .AddComponent<AbilityEffectRunAction>(c =>
                {
                    c.Actions = new ActionList
                    {
                        Actions = new GameAction[]
                        {

                            new ContextActionConditionalSaved
                            {
                                Succeed = new ActionList
                                {
                                    Actions = Array.Empty<GameAction>()
                                },

                                Failed = new ActionList
                                {
                                    Actions = new GameAction[]
                                    {
                                        new ContextActionApplyBuff
                                        {
                                            m_Buff = horrifyingCurseBuff.ToReference<BlueprintBuffReference>(),
                                             DurationValue = new ContextDurationValue
                                             {
                                                DiceType = DiceType.Zero,
                                                DiceCountValue = 0,
                                                BonusValue = 0
                                             },
                                            Permanent = true
                                        }
                                    }
                                }
                            }
                        }
                    };
                    c.SavingThrowType=SavingThrowType.Will;
                })
                .Configure();

            var variants = new List<Blueprint<BlueprintAbilityReference>>
                {
                    BestowCurseFeebleBody,
                    BestowCurseIdiocy,
                    BestowCurseWeakness,
                    BestowCurseDeterioration,
                    BestowCurseHorrifying
                };

            AbilityConfigurator.For(bestowCurse)
                .SetRange(AbilityRange.Close)
                .SetDescription(BestowCurse)
                .AddAbilityVariants(variants, mergeBehavior: ComponentMerge.Replace)
                .Configure();

            foreach (var curse in bestowCurses)
            {
                AbilityConfigurator.For(curse)
                    .SetRange(AbilityRange.Close)
                    .RemoveComponents(c => c is AbilityDeliverTouch)
                    .Configure();
            }

            BlueprintAbility Contagion = BlueprintTool.Get<BlueprintAbility>("48e2744846ed04b4580be1a3343a5d3d");
            BlueprintAbility ContagionCackleFeverResolve = BlueprintTool.Get<BlueprintAbility>("78f6126335c7f48448b88762935b232e");
            BlueprintAbility ContagionShakesResolve = BlueprintTool.Get<BlueprintAbility>("662d281795158394289b0c3efdd8d17b");
            BlueprintAbility ContagionBlindingSicknessResolve = BlueprintTool.Get<BlueprintAbility>("f9e7a49da9eff3d4a98b646bf6c16cc4");
            BlueprintAbility ContagionMindFireResolve = BlueprintTool.Get<BlueprintAbility>("8cdc071f953733049bab19ac2d15dda6");
            BlueprintAbility ContagionBubonicPlagueResolve = BlueprintTool.Get<BlueprintAbility>("8adcc5db933f4464cbd45817ec81c3aa");

            BlueprintAbility[] contagionResolves = new BlueprintAbility[] { ContagionCackleFeverResolve, ContagionShakesResolve, ContagionBlindingSicknessResolve, ContagionMindFireResolve, ContagionBubonicPlagueResolve };
            BlueprintBuff[] aggravations = new BlueprintBuff[] { ContagionAggravationBuffs.MindFireAggravation, ContagionAggravationBuffs.ShakesAggravation, ContagionAggravationBuffs.BlindingSicknessAggravation, ContagionAggravationBuffs.BrainFeverAggravation, ContagionAggravationBuffs.BubonicPlagueAggravation};

            for (int i = 0; i < contagionResolves.Length; i++)
            {
                var ability = contagionResolves[i];
                var aggravationBuff = aggravations[i];

                AbilityConfigurator.For(ability)
                    .SetRange(AbilityRange.Close)
                    .RemoveComponents(c => c is AbilityDeliverTouch)
                    .EditComponent<AbilityEffectRunAction>(c =>
                    {
                        foreach (var rootAction in c.Actions.Actions)
                        {
                            if (rootAction is ContextActionConditionalSaved saved)
                            {
                                var failedList = saved.Failed.Actions.ToList();

                                failedList.Add(
                                    new ContextActionApplyBuff
                                    {
                                        m_Buff = aggravationBuff
                                            .ToReference<BlueprintBuffReference>(),

                                        DurationValue = new ContextDurationValue
                                        {
                                            Rate = DurationRate.Rounds,
                                            DiceType = DiceType.Zero,
                                            DiceCountValue = 0,
                                            BonusValue = new ContextValue
                                            {
                                                ValueType = ContextValueType.CasterProperty,
                                                Property = UnitProperty.Level
                                            }
                                        }
                                    });

                                saved.Failed.Actions = failedList.ToArray();
                                //LogWrapper.Get("NecromancyLevel4").Info($"Added aggravation buff to {ability.name}");
                            }
                        }
                    })
                    .Configure();
            }
            var contagionResolvesRef = new List<Blueprint<BlueprintAbilityReference>>
                {
                    ContagionCackleFeverResolve, ContagionShakesResolve, ContagionBlindingSicknessResolve, ContagionMindFireResolve, ContagionBubonicPlagueResolve
                };

            AbilityConfigurator.For(Contagion)
                .SetRange(AbilityRange.Close)
                .SetDescription(ContagionDescription)
                .AddAbilityVariants(contagionResolvesRef, mergeBehavior: ComponentMerge.Replace)
                .Configure();

            BlueprintAbility Boneshatter = BlueprintTool.Get<BlueprintAbility>("f2f1efac32ea2884e84ecaf14657298b");
            BlueprintFeature incorporeal = BlueprintTool.Get<BlueprintFeature>("c4a7f98d743bc784c9d4cf2105852c39");
            BlueprintFeature constructType = BlueprintTool.Get<BlueprintFeature>("fd389783027d63343b4a5634bd81645f");
            BlueprintFeature undeadType = BlueprintTool.Get<BlueprintFeature>("734a29b693e9ec346ba2951b27987e33");

            AbilityConfigurator.For(Boneshatter)
                .SetDescription(BoneshatterDescription)
                .EditComponent<AbilityEffectRunAction>(c =>
                {
                    var list = c.Actions.Actions.ToList();
                    var dealDamage = list.OfType<ContextActionDealDamage>().FirstOrDefault();

                    if (dealDamage != null)
                    {
                        list.Remove(dealDamage);

                        var conditional = new Conditional
                        {
                            ConditionsChecker = new ConditionsChecker
                            {
                                Operation = Operation.And,
                                Conditions = new Condition[]
                                {
                                        new ContextConditionHasFact
                                        {
                                            m_Fact = undeadType.ToReference<BlueprintUnitFactReference>(),
                                        }
                                }
                            },

                            IfTrue = new ActionList
                            {
                                Actions = new GameAction[]
                                {
                                        new ContextActionDealDamage
                                        {
                                            AbilityType = dealDamage.AbilityType,
                                            DamageType = dealDamage.DamageType,
                                            Duration = dealDamage.Duration,
                                            Value = new ContextDiceValue
                                            {
                                                DiceType = DiceType.D8,
                                                DiceCountValue = dealDamage.Value.DiceCountValue,
                                                BonusValue = dealDamage.Value.BonusValue
                                            },
                                            HalfIfSaved = true
                                        }
                                }

                            },

                            IfFalse = new ActionList
                            {
                                Actions = new GameAction[]
                                {
                                        dealDamage
                                }
                            }
                        };

                        list.Add(conditional);
                    }

                    c.Actions.Actions = list.ToArray();
                })
                .Configure();


            BlueprintAbility PoisonCast = BlueprintTool.Get<BlueprintAbility>("2a6eda8ef30379142a4b75448fb214a3");
            BlueprintBuff PoisonBuff = BlueprintTool.Get<BlueprintBuff>("ba1ae42c58e228c4da28328ea6b4ae34");

            BuffConfigurator.For(PoisonBuff)
                .SetDescription(PoisonDescription)
                .EditComponent<BuffPoisonStatDamage>(c =>
                {
                    c.Value = new DiceFormula
                    {
                        m_Dice = DiceType.D4,
                        m_Rolls = 1
                    };
                })
                .Configure();

            AbilityConfigurator.For(PoisonCast)
                .SetRange(AbilityRange.Close)
                .SetDescription(PoisonCastDescription)
                .RemoveComponents(c => c is AbilityEffectStickyTouch)
                .AddComponent<AbilityEffectRunAction>(c =>
                {
                    c.Actions = new ActionList
                    {
                        Actions = new GameAction[]
                        {
                            new ContextActionConditionalSaved
                            {
                                Succeed = new ActionList
                                {
                                    Actions = Array.Empty<GameAction>()
                                },

                                Failed = new ActionList
                                {
                                    Actions = new GameAction[]
                                    {
                                        new ContextActionApplyBuff
                                        {
                                            m_Buff = PoisonBuff
                                                .ToReference<BlueprintBuffReference>(),

                                            DurationValue = new ContextDurationValue
                                            {
                                                DiceType = DiceType.Zero,
                                                DiceCountValue = 0,
                                                BonusValue = 6
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    };
                    c.SavingThrowType = SavingThrowType.Fortitude;
                })
                .Configure();

            BlueprintAbility ConsumeUndead = BlueprintTool.Get<BlueprintAbility>("0e633cc133207a849915e4d15dec8410");
            BlueprintAbility SiphonLife = BlueprintTool.Get<BlueprintAbility>("7bd52a86498c7854ebe99bc3cfb85bfe");

            AbilityConfigurator.For(ConsumeUndead)
                .AddComponent<AbilitySpawnFx>(c =>
                {
                    c.PrefabLink = new PrefabLink
                    {
                        AssetId = "afc207ce60f237746a561a2877ec29e3"
                    };
                    c.Anchor = AbilitySpawnFxAnchor.Caster;
                    c.PositionAnchor = AbilitySpawnFxAnchor.Caster;
                    c.OrientationMode = AbilitySpawnFxOrientation.TurnTo;
                    c.OrientationAnchor = AbilitySpawnFxAnchor.SelectedTarget;
                    c.Time = AbilitySpawnFxTime.OnApplyEffect;
                })
                .AddComponent<AbilityDeliverProjectile>(c =>
                {
                    c.m_Projectiles = new BlueprintProjectileReference[]
                    {
                        BlueprintTool
                            .Get<BlueprintProjectile>("3b12e2485e793c9489536b71797cf99d")
                            .ToReference<BlueprintProjectileReference>()
                    };

                    c.Type = AbilityProjectileType.Simple;
                    c.NeedAttackRoll = false;

                    c.m_LineWidth = 5.Feet();
                    c.m_Length = 0.Feet();
                    c.DelayBetweenProjectiles = 0f;
                })
                .SetAnimation(SiphonLife.Animation)
                .SetHasFastAnimation(true)
                .Configure();
            ConsumeUndead.AnimationStyle = SiphonLife.AnimationStyle;
        }
    }
}
