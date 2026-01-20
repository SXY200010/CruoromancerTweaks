using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kingmaker.Kingdom.Settlements.SettlementGridTopology;

namespace CruoromancerTweaks.ModifiedContent.Classes
{
    internal class BloodLine
    {
        private static readonly string[] bloodLineUndeadProgressionGuids = [
            "a1a8bf61cadaa4143b2d4966f2d1142e", 
            "a56252fbbd7b45db97129187ee1fa3ba",
            "618c6b173f6947843a62e0cbbed86d16",
            "5bc63fdb68b539f4fa500cfb2d0fe0f6"
        ];
        private static readonly string bloodLineUndeadFeatureGuid = "8a59e4af9f32950418260034c8b477fa";
        private static readonly string bloodLineUndeadArcanaGuid = "1a5e7191279e7cd479b17a6ca438498c";
        private static readonly string displayNameBloodLine = "BloodLineUndead.Name";
        private static readonly string displayNameBloodLineArcana = "BloodLineUndeadArcana.Name";
        private static readonly string displayDescriptionBloodLine = "BloodLineUndead.Description";

        private static readonly string bloodLineClassSkillGuid = "461a5980bfe5579468ba99d369f75308";
        private static readonly string bloodLineClassSkillDescription = "BloodlineUndeadClassSkill.Description";
        private static readonly string BloodlineUndeadSpellLevel1Guid = "3e4080a48cbd3154aac907befca64801";
        private static readonly string BloodlineUndeadSpellLevel1DisplayName = "BloodlineUndeadSpellLevel1.Name";
        private static readonly string BloodlineUndeadSpellLevel1Description = "BloodlineUndeadSpellLevel1.Description";
        
        private static readonly string immunityPrecisionDamageDisplayName = "ImmunityPrecisionDamage.Name";
        private static readonly string immunityPrecisionDamageDescription = "ImmunityPrecisionDamage.Description";

        public static void Configure()
        {
            BlueprintAbility RayOfenfeeblementAbility = BlueprintTool.Get<BlueprintAbility>("450af0402422b0b4980d9c2175869612");
            BlueprintFeature undeadTypeLevelUp15 = FeatureConfigurator.New("undeadTypeFeature15", "C3C79895-5F86-4942-82BC-D5EFC10A5493")
                .CopyFrom(BlueprintTool.Get<BlueprintFeature>("734a29b693e9ec346ba2951b27987e33"))
                .SetIcon(BlueprintTool.Get<BlueprintAbility>("57fcf8016cf04da4a8b33d2add14de7e").Icon)
                .Configure();
            BlueprintFeature undeadTypeLevelUp20 = FeatureConfigurator.New("undeadTypeFeature20", "FB01543A-40D3-4CF0-BBA3-1AD8222A1B15")
                .CopyFrom(BlueprintTool.Get<BlueprintFeature>("734a29b693e9ec346ba2951b27987e33"))
                .SetHideInCharacterSheetAndLevelUp(true)
                .SetHideInUI(true)
                .Configure();
            BlueprintFeature undeadImmunitiesLevelUp = FeatureConfigurator.New("undeadImmunitiesFeature", "52623305-1245-420E-8D60-D103F9C361E5")
                .CopyFrom(BlueprintTool.Get<BlueprintFeature>("8a75eb16bfff86949a4ddcb3dd2f83ae"))
                .SetHideInCharacterSheetAndLevelUp(true)
                .SetHideInUI(true)
                .Configure();
            BlueprintFeature ImmunityPrecisionDamage = FeatureConfigurator.New("ImmunityPrecisionDamage", "845FD279-D06C-4EEA-B79F-544C67859581")
                .AddComponent<AddImmunityToPrecisionDamage>()
                .SetGroups(FeatureGroup.Feat)
                .SetDisplayName(immunityPrecisionDamageDisplayName)
                .SetDescription(immunityPrecisionDamageDescription)
                .SetIcon(BlueprintTool.Get<BlueprintFeature>("b3e403ebbdad8314386270fefc4b4cc8").Icon)
                .Configure();
            //修改血承名称，添加15/20级被动
            foreach (string guid in bloodLineUndeadProgressionGuids)
            {
                ProgressionConfigurator.For(guid)
                    .SetDisplayName(displayNameBloodLine)
                    .SetDescription(displayDescriptionBloodLine)
                    .AddToLevelEntry(15, [undeadTypeLevelUp15, undeadImmunitiesLevelUp])
                    .AddToLevelEntry(20, [undeadTypeLevelUp20, undeadImmunitiesLevelUp])
                    .Configure(delayed: true);
            }
            //修改血承名称
            FeatureConfigurator.For(bloodLineUndeadFeatureGuid)
                .SetDisplayName(displayNameBloodLine)
                .Configure(delayed: true);
            FeatureConfigurator.For(bloodLineUndeadArcanaGuid)
                .SetDisplayName(displayNameBloodLineArcana)
                .Configure(delayed: true);
            //修改血承本职技能描述
            FeatureConfigurator.For(bloodLineClassSkillGuid)
                .SetDescription(bloodLineClassSkillDescription)
                .Configure(delayed: true);
            //修改血承3级技能
            FeatureConfigurator.For(BloodlineUndeadSpellLevel1Guid)
                .SetDisplayName(BloodlineUndeadSpellLevel1DisplayName)
                .SetDescription(BloodlineUndeadSpellLevel1Description)
                .SetIcon(RayOfenfeeblementAbility.Icon)
                .EditComponents<AddKnownSpell>(
                    edit: c => c.m_Spell = RayOfenfeeblementAbility.ToReference<BlueprintAbilityReference>(),
                    predicate: c => true
                )
                .Configure(delayed: true);
            //修改17级凋死术图标
            FeatureConfigurator.For("ac5bf30c6b7e76248a2507047bd5703d")
                .SetIcon(BlueprintTool.Get<BlueprintAbility>("08323922485f7e246acb3d2276515526").Icon)
                .Configure(delayed: true);
            //修改19级吸能术图标
            FeatureConfigurator.For("273ac94653a5f3f4cafcac11499c2016")
                .SetIcon(BlueprintTool.Get<BlueprintAbility>("37302f72b06ced1408bf5bb965766d46").Icon)
                .Configure(delayed: true);
            //修改20级亡者之列
            FeatureConfigurator.For("b3e403ebbdad8314386270fefc4b4cc8")
                .AddComponent<AddImmunityToPrecisionDamage>()
                .Configure(delayed: true);
        }
    }
}
