using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.ElementsSystem;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.CasterCheckers;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Mechanics.Properties;
using System;
using System.Linq;

namespace CruoromancerTweaks.ModifiedContent.Classes
{
    public class FocusedInfusion
    {
        private static readonly string Description = "FocusedInfusion.Description";
        private static readonly string Name = "FocusedInfusion.Name";
        private static readonly string CooldownName = "FocusedInfusionCooldown.Name";

        private const string FocusedInfusionAbilityGuid = "5a5d3a4bcbc54ea1b8581923373fd992";

        private const string FocusedInfusionCooldownBuffGuid = "1D4995C544F2453AA41F8B14236260D0";

        private const string FocusedInfusionFreeAbilityGuid = "aeba73f7263c42f0ae6f1e31578d6f5d";

        private const string FocusedInfusionDCBonusBuffGuid = "45696A72151E42F58C3CC1BAC2D15FCF";

        private const string FocusedInfusionDCLv20BonusBuffGuid = "BC89B5F5EF6C49E7B1692C8016612099";

        public static void Configure()
        {
            BlueprintAbility ability = BlueprintTool.Get<BlueprintAbility>(FocusedInfusionAbilityGuid);

            AbilityConfigurator.For(ability)
                .SetDescription(Description)
                .SetDescriptionShort(Description)
                .Configure();

            //AbilityConfigurator.For(ability)
            //    .AddContextRankConfig(
            //        ContextRankConfigs.ClassLevel(
            //            new string[]
            //            {
            //                "8789dcfc6f8e4fe49d80b90472ea6993"
            //            }
            //        )
            //        .WithCustomProgression(
            //            (4, 1),
            //            (20, 2)
            //        )
            //    )
            //    .Configure();

            BlueprintBuff cooldownBuff =
                BuffConfigurator.New(
                        "FocusedInfusionCooldown",
                        FocusedInfusionCooldownBuffGuid
                    )
                    .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                    .SetStacking(StackingType.Replace)
                    .SetDisplayName(CooldownName)
                    .Configure();

            BlueprintBuff focusedInfusionBuff =
                BuffConfigurator.New(
                "FocusedInfusionDCBonusBuff",
                FocusedInfusionDCBonusBuffGuid)
                .SetDisplayName(null)
                .SetDescription(null)
                .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                .AddIncreaseSpellSchoolDC(
                    bonusDC: 1,
                    descriptor: null,
                    school: SpellSchool.Necromancy
                )
                .Configure();

            BlueprintBuff focusedInfusionlv20Buff =
                BuffConfigurator.New(
                "FocusedInfusionDCLv20BonusBuff",
                FocusedInfusionDCLv20BonusBuffGuid)
                .SetDisplayName(null)
                .SetDescription(null)
                .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                .AddIncreaseSpellSchoolDC(
                    bonusDC: 2,
                    descriptor: null,
                    school: SpellSchool.Necromancy
                )
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

                  list.Add(new Conditional
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
                                      Value = 5
                                  }
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
                                              m_Buff = focusedInfusionBuff.ToReference<BlueprintBuffReference>(),
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
                      }
                  });

                  c.Actions.Actions = list.ToArray();
              })
              .EditComponent<AbilityExecuteActionOnCast>(c =>
              {
                  foreach (var action in c.Actions.Actions)
                  {
                      if (action is Conditional cond)
                      {
                          cond.IfTrue.Actions = cond.IfTrue.Actions
                              .Append<GameAction>(
                                  new ContextActionApplyBuff
                                  {
                                      m_Buff = focusedInfusionlv20Buff
                                          .ToReference<BlueprintBuffReference>(),
                                      ToCaster = true,
                                      DurationValue = new ContextDurationValue
                                      {
                                          DiceType = DiceType.Zero,
                                          DiceCountValue = 0,
                                          BonusValue = 2,
                                          Rate = DurationRate.Rounds
                                      }
                                  }
                              ).ToArray();
                      }
                  }
              })
              .Configure();

            Type[] componentTypes = ability.ComponentsArray
            .Select(c => c.GetType())
            .Distinct()
            .ToArray();

            BlueprintAbility focusedInfusionFree =
                AbilityConfigurator.New(
                    "FocusedInfusionFree",
                    FocusedInfusionFreeAbilityGuid)
                .CopyFrom(ability, componentTypes)
                .SetDisplayName(Name)
                .SetActionType(UnitCommand.CommandType.Free)
                .Configure();
            focusedInfusionFree.m_AllElements = ability.ElementsArray;
        }
    }
}
