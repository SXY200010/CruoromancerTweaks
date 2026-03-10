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
using Kingmaker.UnitLogic.Mechanics.Components;
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
    internal class NecromancyLevel6
    {
        public static void Configure()
        {
            BlueprintAbility SiphonLife = BlueprintTool.Get<BlueprintAbility>("7bd52a86498c7854ebe99bc3cfb85bfe");

            BlueprintBuff Fatigued = BlueprintTool.Get<BlueprintBuff>("e6f2fc5d73d88064583cb828801212f4");

            AbilityConfigurator.For(SiphonLife)
                .EditComponent<AbilityEffectRunAction>(c =>
                {
                    foreach (var rootAction in c.Actions.Actions)
                    {
                        if (rootAction is ContextActionDealDamage dealDamage)
                        {
                            dealDamage.Value = new ContextDiceValue()
                            {
                                DiceType = DiceType.D8,
                                DiceCountValue = new ContextValue
                                {
                                    ValueType = ContextValueType.Rank
                                },
                                BonusValue = 0
                            };
                        }
                    }
                })
                .EditComponent<AbilityEffectRunAction>(c =>
                {
                    var list = c.Actions.Actions.ToList();

                    list.Add(new ContextActionApplyBuff
                    {
                        m_Buff = Fatigued.ToReference<BlueprintBuffReference>(),
                        DurationValue = new ContextDurationValue
                        {
                            Rate = DurationRate.Rounds,
                            DiceType = DiceType.Zero,
                            DiceCountValue = 0,
                            BonusValue = 0
                        },
                        Permanent = true
                    });

                    c.Actions.Actions = list.ToArray();
                })
                .Configure();
        }
    }
}
