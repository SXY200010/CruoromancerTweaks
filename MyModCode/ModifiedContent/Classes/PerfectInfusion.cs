using BlueprintCore.Blueprints.CustomConfigurators.Classes;

namespace CruoromancerTweaks.ModifiedContent.Classes
{
    internal class PerfectInfusion
    {
        private static readonly string Description = "PerfectInfusion.Description";

        private const string PerfectInfusionFeatureGuid = "74686a88b35946219aa72455c50a15b0";
        public static void Configure()
        {
            FeatureConfigurator.For(PerfectInfusionFeatureGuid)
                .SetDescription(Description)
                .Configure();
        }
    }
}
