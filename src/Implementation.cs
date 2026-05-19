using MelonLoader;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RandomizeInterloperRareSpawns
{
    internal class Implementation : MelonMod
    {
        // NOTE: These names must exactly match the GameObject.name values at runtime.
        // If items are still not being destroyed, enable the "dump all objects" log
        // in PatchSceneObjects to compare against these strings.
        public static string[] rareLootNames = { "GEAR_Hacksaw", "GEAR_Hammer", "GEAR_KeroseneLampB", "GEAR_MagnifyingLens", "GEAR_Firestriker", "GEAR_BedRoll" };

        public override void OnApplicationStart()
        {
            Log("=== RandomizeInterloperRareSpawns v{0} starting up ===", Info.Version);
            Settings.OnLoad();
            Log("Settings loaded.");
            ProbabilityFunctions.AddToModComponent();
            Patches.Register();
            Log("Rare loot names registered: {0}", string.Join(", ", rareLootNames));
            Log("=== Startup complete ===");
        }

        internal static void Log(string message)
        {
            MelonLogger.Msg(message);
        }

        internal static void Log(string message, params object[] parameters)
        {
            string preformattedMessage = string.Format(message, parameters);
            Log(preformattedMessage);
        }

        internal static void PatchSceneObjects()
        {
            List<GameObject> rObjs = SpawnUtils.GetRootObjects();
            Log("PatchSceneObjects: found {0} root objects across all loaded scenes.", rObjs.Count);

            int totalChildren = 0;
            int missionIdCount = 0;
            int destroyedCount = 0;

            // Log all root object names to help diagnose name mismatches
            Log("--- Root object scan begin ---");
            foreach (GameObject rootObj in rObjs)
            {
                bool hasMissionId = rootObj.GetComponent<MissionObjectIdentifier>() != null;
                bool isRare = IsRareLoot(rootObj);

                if (hasMissionId)
                {
                    missionIdCount++;
                    Log("  [ROOT][MissionId] name='{0}' isRareLoot={1} pos={2}",
                        rootObj.name, isRare, rootObj.transform.position.ToString());
                }

                if (hasMissionId && isRare)
                {
                    Log("  --> Destroying ROOT rare loot: '{0}'", rootObj.name);
                    UnityEngine.Object.Destroy(rootObj);
                    destroyedCount++;
                    continue;
                }

                List<GameObject> children = new List<GameObject>();
                SpawnUtils.GetChildren(rootObj, children);
                totalChildren += children.Count;
                destroyedCount += PatchObjects(children);
            }

            Log("--- Root object scan end ---");
            Log("PatchSceneObjects summary: rootObjects={0}, totalChildren={1}, rootsWithMissionId={2}, destroyed={3}",
                rObjs.Count, totalChildren, missionIdCount, destroyedCount);

            if (destroyedCount == 0)
            {
                Log("WARNING: No rare loot was destroyed. Possible causes:");
                Log("  1) Rare loot GameObjects have not spawned yet when this runs (timing issue).");
                Log("  2) GameObject names don't match rareLootNames (case/suffix difference).");
                Log("  3) MissionObjectIdentifier component is not present on the loot objects.");
                Log("  4) SpawnTagManager probability functions returned 0 so items never placed.");
            }
        }

        internal static int PatchObjects(List<GameObject> objs)
        {
            int destroyedCount = 0;
            foreach (GameObject obj in objs)
            {
                MissionObjectIdentifier objectIdentifier = obj.GetComponent<MissionObjectIdentifier>();
                if (objectIdentifier != null)
                {
                    bool isRare = IsRareLoot(obj);
                    Log("  [CHILD][MissionId] name='{0}' isRareLoot={1} pos={2}",
                        obj.name, isRare, obj.transform.position.ToString());

                    if (isRare)
                    {
                        Log("  --> Destroying CHILD rare loot: '{0}'", obj.name);
                        UnityEngine.Object.Destroy(obj);
                        destroyedCount++;
                    }
                }
            }
            return destroyedCount;
        }

        public static bool IsRareLoot(GameObject gameObject)
        {
            bool result = rareLootNames.Contains<string>(gameObject.name);
            // Uncomment this if you want per-object name tracing (very verbose):
            // Log("    IsRareLoot('{0}') = {1}", gameObject.name, result);
            return result;
        }
    }
}