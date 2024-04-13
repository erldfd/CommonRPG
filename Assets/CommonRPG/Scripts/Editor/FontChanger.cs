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
            string folderPath = "Assets/YourFolder"; // ���ϴ� ���� ��η� �����ϼ���.
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