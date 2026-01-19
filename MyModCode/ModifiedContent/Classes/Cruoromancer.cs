using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using System.Collections.Generic;

namespace CruoromancerTweaks.ModifiedContent.Classes
{
    public class Cruoromancer
    {
        private static readonly string BloodInfusionFreeName = "BloodInfusionFree.Name";
        private static readonly string BloodInfusionFreeDescription = "BloodInfusion.Description";
        //private static readonly string addAugmentSummoningName = "AugmentSummoning.Name";
        //private static readonly string addAugmentSummoningDescription = "AugmentSummoning.Description";


        private const string BloodInfusionFreeFeatureGuid = "B5D88962A13240F18FF14F9C8589867B";
        //private const string addAugmentSummoningFeatureGuid = "AE5AD5A391A245DDBA538503CAC5AAE5";


        public static void Configure()
        {
            BlueprintAbility focusedInfusionFree = BlueprintTool.Get<BlueprintAbility>("aeba73f7263c42f0ae6f1e31578d6f5d");
            BlueprintAbility sickeningInfusionFree = BlueprintTool.Get<BlueprintAbility>("D51362FA8BE24AE8900C6768EE794284");
            BlueprintFeature augmentSummoning = BlueprintTool.Get<BlueprintFeature>("38155ca9e4055bb48a89240a2055dcc3");
            BlueprintFeature bloodInfusionFeature = BlueprintTool.Get<BlueprintFeature>("9677fd6917ce46ae9fbd34a97af0849d");
            BlueprintFeature profaneInfusionFeature = BlueprintTool.Get<BlueprintFeature>("856776AC66A44E6381D303E223E48BC1");
            BlueprintArchetype cruoromancerArchetype = BlueprintTool.Get<BlueprintArchetype>("8789dcfc6f8e4fe49d80b90472ea6993");
            BlueprintFeature wizardFeatSelection = BlueprintTool.Get<BlueprintFeature>("8c3102c2ff3b69444b139a98521a4899");

            FeatureConfigurator.For(bloodInfusionFeature)
                .SetDescription(BloodInfusionFreeDescription)
                .SetDescriptionShort(BloodInfusionFreeDescription)
                .Configure();

            BlueprintFeature bloodInfusionFreeFeature =
                FeatureConfigurator.New("BloodInfusionFreeFeature", BloodInfusionFreeFeatureGuid)
                .CopyFrom(bloodInfusionFeature)
                .SetDisplayName(BloodInfusionFreeName)
                .AddFacts(new List<Blueprint<BlueprintUnitFactReference>>
                    {
                        focusedInfusionFree,
                        sickeningInfusionFree
                    })
                .Configure();

            //BlueprintFeature augmentSummoningFeature =
            //    FeatureConfigurator.New("AugmentSummoningFeature", addAugmentSummoningFeatureGuid)
            //    .CopyFrom(
            //        augmentSummoning,
            //        typeof(ComponentsList))
            //    .SetDisplayName(addAugmentSummoningName)
            //    .SetDescription(addAugmentSummoningDescription)
            //    .Configure();

            ArchetypeConfigurator.For(cruoromancerArchetype)
               .AddToAddFeatures(
                   level: 8,
                   bloodInfusionFreeFeature
               )
               .AddToAddFeatures(
                   level: 5,
                   augmentSummoning
               )
               .AddToAddFeatures(
                   level: 15,
                   profaneInfusionFeature
               )
               .Configure();
        }
    }
}
