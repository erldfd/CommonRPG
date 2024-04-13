using UnityEngine;
using UnityEditor;
using TMPro;

namespace CommonRPG
{
    public class FontChanger : Editor
    {
        [MenuItem("Tools/Change TMP Font")]
        public static void ChangeTMPFont()
        {
            string folderPath = "Assets/YourFolder"; // 원하는 폴더 경로로 변경하세요.
            string[] guids = AssetDatabase.FindAssets("t:prefab", new string[] { folderPath });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (prefab != null)
                {
                    TextMeshProUGUI[] texts = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);
                    foreach (TextMeshProUGUI text in texts)
                    {
                        text.autoSizeTextContainer = true;
                    }

                    EditorUtility.SetDirty(prefab);
                }
            }

            AssetDatabase.SaveAssets();
        }
    }
}