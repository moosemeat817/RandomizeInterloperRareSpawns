using GearSpawner;
using Il2Cpp;
using Il2CppTLD.Gameplay;

namespace RandomizeInterloperRareSpawns
{
    internal class Patches
    {
        internal static void Register()
        {
            SpawnManager.OnFinishSpawning += OnFinishSpawning;
            Implementation.Log("Subscribed to SpawnManager.OnFinishSpawning.");
        }

        private static void OnFinishSpawning(System.Collections.Generic.IReadOnlyList<GearItem> spawnedItems)
        {
            ExperienceModeType modeType = ExperienceModeManager.GetCurrentExperienceModeType();
            Implementation.Log(">>> OnFinishSpawning fired. ExperienceModeType = {0}", modeType.ToString());

            if (modeType == ExperienceModeType.Interloper)
            {
                Implementation.Log("    Interloper confirmed. Calling PatchSceneObjects.");
                Implementation.PatchSceneObjects();
            }
            else if (modeType == ExperienceModeType.Custom)
            {
                var customMode = GameManager.GetCustomMode();
                if (customMode == null)
                {
                    Implementation.Log("    Custom mode: GetCustomMode() returned null. Skipping.");
                    return;
                }
                bool isInterloperLike = customMode.m_ItemSpawnChance ==
                                        Il2CppTLD.Gameplay.Tunable.CustomTunableLMHV.Low;
                Implementation.Log("    Custom mode: m_ItemSpawnChance={0}, IsInterloperLike={1}",
                    customMode.m_ItemSpawnChance.ToString(), isInterloperLike);
                if (isInterloperLike)
                {
                    Implementation.Log("    Custom mode qualifies. Calling PatchSceneObjects.");
                    Implementation.PatchSceneObjects();
                }
                else
                {
                    Implementation.Log("    Custom mode does not qualify. Skipping.");
                }
            }
            else
            {
                Implementation.Log("    Mode '{0}' does not require patching. Skipping.", modeType.ToString());
            }
        }
    }
}