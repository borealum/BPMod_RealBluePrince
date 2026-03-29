using HarmonyLib;
using Il2CppSystem.Text.RegularExpressions;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using static MelonLoader.MelonLogger;

[assembly: MelonInfo(typeof(BPMod_RealBluePrince.Mod_RealBluePrince), "BPMod_RealBluePrince", "1.0.0", "Borealum", null)]
[assembly: MelonGame("Dogubomb", "BLUE PRINCE")]

namespace BPMod_RealBluePrince
{
    public class Mod_RealBluePrince : MelonMod
    {
        public static Material sourceMaterial;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName != null && sceneName.Equals("Mount Holly Estate"))
            {
                //Get just a whatever atelier wall
                GameObject gg = FindDeep(GameObject.Find("Standalone Rooms").transform, "Standard Room Spwn/ATELIER/Junction279/B-house/45 - Aquarium - B/_CULLABLE/_Static/Baked/Layer 1/_Combined Mesh [Layer 1 - Cast Recieve]")?.gameObject;
                sourceMaterial = gg.GetComponent<MeshRenderer>().materials[0];

                SetMaterialsBlue(GameObject.FindObjectsOfType<Renderer>());
                SetTextsBlue(GameObject.FindObjectsOfType<TextMeshPro>());

                RenderSettings.skybox = sourceMaterial;
            }
        }

        private static void SetMaterialsBlue(Renderer[] renderers)
        {
            foreach (Renderer rend in renderers)
            {
                Material[] mats = rend.materials;
                //if (rend.gameObject.layer == 28 || rend.gameObject.layer == 29)
                //{
                //    continue;
                //}
                for (int i = 0; i < mats.Length; i++)
                {
                    if (mats[i] != null && sourceMaterial != null)
                    {
                        mats[i].CopyPropertiesFromMaterial(sourceMaterial);
                    }
                }
                // Reassign (not always necessary, but safe)
                rend.materials = mats;
            }
        }

        private static void SetTextsBlue(TextMeshPro[] texts)
        {
            foreach (TextMeshPro text in texts)
            {
                if (text == null) continue;
                string textText = text.text ?? "";
                textText = Regex.Replace(textText, "<.*?>", "");
                
                MatchCollection matches = Regex.Matches(textText, @"\b\w+\b");
                for (int i = 0; i < matches.Count; i++)
                {
                    Match match = matches[i];
                    string word = match.Value;
                    if (word.Length > 2)
                    {
                        string replacement = char.IsUpper(word[0]) ? "Blue" : "blue";
                        textText = textText.Replace(word, replacement);
                    }
                }
                text.text = textText;
                text.ForceMeshUpdate();
            }
        }

        [HarmonyPatch(typeof(GameObject), "SetActive")]
        class Patch_SetActive
        {
            static void Prefix(GameObject __instance, bool value)
            {
                if (sourceMaterial != null)
                {
                    SetMaterialsBlue(__instance.GetComponentsInChildren<Renderer>(true));
                }
                SetTextsBlue(__instance.GetComponentsInChildren<TextMeshPro>(true));
            }
        }

        // Finds child by path, including inactive objects
        private Transform FindDeep(Transform parent, string path)
        {
            string[] parts = path.Split('/');
            Transform current = parent;
            bool logOutput = false;
            foreach (string partTxt in parts)
            {
                if (logOutput) LoggerInstance.Msg($"findDeep - current: {current.name}");
                if (logOutput) LoggerInstance.Msg($"findDeep - childCount: {current.childCount}");
                if (logOutput) LoggerInstance.Msg($"findDeep - part: {partTxt}");

                for (int i = 0; i < current.childCount; i++)
                {
                    Transform child = current.GetChild(i);
                    if (logOutput) LoggerInstance.Msg($"findDeep - current inner: {current.name}");
                    if (logOutput) LoggerInstance.Msg($"findDeep - child: {child.name}");
                    if (child.name.Equals(partTxt))
                    {
                        current = child;
                        break;
                    }
                }
            }
            return current;
        }
    }
}
    