using GearSpawner;
using ModComponent;
using System;

namespace RandomizeInterloperRareSpawns
{
    internal static class ProbabilityFunctions
    {
        internal static void AddToModComponent()
        {
            Implementation.Log("Registering spawn tag functions with SpawnTagManager...");
            try
            {
                SpawnTagManager.AddFunction("RandomizeInterloperRareSpawns_Guaranteed",
                    new Func<DifficultyLevel, FirearmAvailability, GearSpawnInfo, float>(GuaranteedSpawns));
                Implementation.Log("  Registered: RandomizeInterloperRareSpawns_Guaranteed");

                SpawnTagManager.AddFunction("RandomizeInterloperRareSpawns_Random",
                    new Func<DifficultyLevel, FirearmAvailability, GearSpawnInfo, float>(RandomSpawns));
                Implementation.Log("  Registered: RandomizeInterloperRareSpawns_Random");
            }
            catch (Exception ex)
            {
                Implementation.Log("ERROR registering spawn tag functions: {0}", ex.Message);
                Implementation.Log("  Stack trace: {0}", ex.StackTrace);
            }
        }

        public static float GuaranteedSpawns(DifficultyLevel difficultyLevel, FirearmAvailability firearmAvailability, GearSpawnInfo gearSpawnInfo)
        {
            Implementation.Log("[Guaranteed] FUNCTION ENTERED for '{0}'", gearSpawnInfo.PrefabName);
            Implementation.Log("[Guaranteed] Called for prefab='{0}' difficulty={1} firearms={2}",
                gearSpawnInfo.PrefabName, difficultyLevel.ToString(), firearmAvailability.ToString());

            if (difficultyLevel != DifficultyLevel.Interloper)
            {
                Implementation.Log("[Guaranteed] Skipping: not Interloper (got {0}).", difficultyLevel.ToString());
                return 0f;
            }

            float result;
            switch (gearSpawnInfo.PrefabName.ToLower())
            {
                case "gear_bedroll":
                    result = Settings.options.guaranteedBedroll ? 100f : 0f;
                    Implementation.Log("[Guaranteed] gear_bedroll: guaranteedBedroll={0} -> {1}%", Settings.options.guaranteedBedroll, result);
                    break;
                case "gear_firestriker":
                    result = Settings.options.guaranteedFirestriker ? 100f : 0f;
                    Implementation.Log("[Guaranteed] gear_firestriker: guaranteedFirestriker={0} -> {1}%", Settings.options.guaranteedFirestriker, result);
                    break;
                case "gear_hacksaw":
                    result = Settings.options.guaranteedHacksaw ? 100f : 0f;
                    Implementation.Log("[Guaranteed] gear_hacksaw: guaranteedHacksaw={0} -> {1}%", Settings.options.guaranteedHacksaw, result);
                    break;
                case "gear_hammer":
                    result = Settings.options.guaranteedHammer ? 100f : 0f;
                    Implementation.Log("[Guaranteed] gear_hammer: guaranteedHammer={0} -> {1}%", Settings.options.guaranteedHammer, result);
                    break;
                case "gear_kerosenelampb":
                    result = Settings.options.guaranteedLantern ? 100f : 0f;
                    Implementation.Log("[Guaranteed] gear_kerosenelampb: guaranteedLantern={0} -> {1}%", Settings.options.guaranteedLantern, result);
                    break;
                case "gear_magnifyinglens":
                    result = Settings.options.guaranteedMagLens ? 100f : 0f;
                    Implementation.Log("[Guaranteed] gear_magnifyinglens: guaranteedMagLens={0} -> {1}%", Settings.options.guaranteedMagLens, result);
                    break;
                default:
                    Implementation.Log("[Guaranteed] '{0}' is not a tracked rare item (no switch case match). Returning 0.", gearSpawnInfo.PrefabName);
                    return 0f;
            }

            return result;
        }

        public static float RandomSpawns(DifficultyLevel difficultyLevel, FirearmAvailability firearmAvailability, GearSpawnInfo gearSpawnInfo)
        {
            Implementation.Log("[Random] FUNCTION ENTERED for '{0}'", gearSpawnInfo.PrefabName);
            Implementation.Log("[Random] Called for prefab='{0}' difficulty={1} firearms={2}",
                gearSpawnInfo.PrefabName, difficultyLevel.ToString(), firearmAvailability.ToString());

            if (difficultyLevel != DifficultyLevel.Interloper)
            {
                Implementation.Log("[Random] Skipping: not Interloper (got {0}).", difficultyLevel.ToString());
                return 0f;
            }

            float result = GetRareItemSpawnProbability(gearSpawnInfo.PrefabName);
            Implementation.Log("[Random] '{0}': returning {1}%.", gearSpawnInfo.PrefabName, result);
            return result;
        }

        private static float GetRareItemSpawnProbability(string itemName)
        {
            float result;
            switch (itemName.ToLower())
            {
                case "gear_bedroll":
                    result = 100f * Settings.options.bedrollSpawnExpectation / 50f;
                    Implementation.Log("[Random][Probability] gear_bedroll: expectation={0} -> {1}%", Settings.options.bedrollSpawnExpectation, result);
                    break;
                case "gear_firestriker":
                    result = 100f * Settings.options.firestrikerSpawnExpectation / 20f;
                    Implementation.Log("[Random][Probability] gear_firestriker: expectation={0} -> {1}%", Settings.options.firestrikerSpawnExpectation, result);
                    break;
                case "gear_hacksaw":
                    result = 100f * Settings.options.hacksawSpawnExpectation / 50f;
                    Implementation.Log("[Random][Probability] gear_hacksaw: expectation={0} -> {1}%", Settings.options.hacksawSpawnExpectation, result);
                    break;
                case "gear_hammer":
                    result = 100f * Settings.options.hammerSpawnExpectation / 40f;
                    Implementation.Log("[Random][Probability] gear_hammer: expectation={0} -> {1}%", Settings.options.hammerSpawnExpectation, result);
                    break;
                case "gear_kerosenelampb":
                    result = 100f * Settings.options.lanternSpawnExpectation / 30f;
                    Implementation.Log("[Random][Probability] gear_kerosenelampb: expectation={0} -> {1}%", Settings.options.lanternSpawnExpectation, result);
                    break;
                case "gear_magnifyinglens":
                    result = 100f * Settings.options.maglensSpawnExpectation / 20f;
                    Implementation.Log("[Random][Probability] gear_magnifyinglens: expectation={0} -> {1}%", Settings.options.maglensSpawnExpectation, result);
                    break;
                default:
                    Implementation.Log("[Random][Probability] '{0}' not tracked. Returning 0.", itemName);
                    return 0f;
            }
            return result;
        }
    }
}