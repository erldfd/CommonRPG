using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace CommonRPG
{

    [CreateAssetMenu(fileName = "CraftingRecipeData", menuName = "ScriptableObjects/CraftingRecipeDataScriptableObject", order = 5)]
    public class CraftingRecipeDataScriptableObject : ScriptableObject
    {
        [SerializeField]
        private List<CraftingRecipe> craftingRecipes;
        /// <summary>
        /// string : recipeKey, it can generate by using GenerateCraftingHashCode method
        /// </summary>
        private Dictionary<string, SItemRecipeResultInfo> recipeTable = new Dictionary<string, SItemRecipeResultInfo>();
        /// <summary>
        /// string : recipeKey, it can generate by using GenerateCraftingHashCode method
        /// </summary>
        public Dictionary<string, SItemRecipeResultInfo> RecipeTable
        {
            get
            {
                return recipeTable;
            }
        }

        private StringBuilder hashCodeStringBuilder = new StringBuilder();

        private void SortRecipes()
        {
            foreach (CraftingRecipe craftingRecipe in craftingRecipes)
            {
                craftingRecipe.Materials.Sort();
            }
        }

        public void CreateRecipeTable()
        {
            SortRecipes();
            recipeTable.Clear();

            foreach (CraftingRecipe craftingRecipe in craftingRecipes) 
            {
                recipeTable.TryAdd(GenerateCraftingHashCode(craftingRecipe.Materials), craftingRecipe.Result);
            }
        }

        public string GenerateCraftingHashCode(in List<CraftingMaterialInfo> craftingMaterials)
        {
            craftingMaterials.Sort();
            hashCodeStringBuilder.Clear();

            foreach (CraftingMaterialInfo craftingMaterial in craftingMaterials) 
            {
                if (craftingMaterial.RecipeItemName == EItemName.None) 
                {
                    continue;
                }

                hashCodeStringBuilder.Append(craftingMaterial.RecipeItemName.ToString());
                hashCodeStringBuilder.Append(craftingMaterial.NeededItemCount);
            }

            // 문자열을 바이트 배열로 변환 (UTF-8 인코딩 사용)
            byte[] data = Encoding.UTF8.GetBytes(hashCodeStringBuilder.ToString());

            // SHA-512 해시코드 생성
            using (SHA512 sha = SHA512.Create())
            {
                byte[] hash = sha.ComputeHash(data);

                // 해시코드를 16진수 문자열로 변환
                hashCodeStringBuilder.Clear();
                foreach (byte b in hash)
                {
                    hashCodeStringBuilder.Append(b.ToString());
                }

                // 해시코드 문자열 반환
                return hashCodeStringBuilder.ToString();
            }
        }
    }

    [Serializable]
    public class CraftingRecipe
    {
        [SerializeField]
        private string recipeName;

        [SerializeField]
        private List<CraftingMaterialInfo> materials;
        public List<CraftingMaterialInfo> Materials { get { return materials; } }

        public SItemRecipeResultInfo Result;
    }

    [Serializable]
    public class CraftingMaterialInfo : IComparable<CraftingMaterialInfo>
    {
        [SerializeField]
        private EItemName recipeItemName;
        public EItemName RecipeItemName { get { return recipeItemName; } }

        [SerializeField]
        private int neededItemCount;
        public int NeededItemCount { get { return neededItemCount; } }

        public CraftingMaterialInfo(EItemName recipeItemName, int neededItemCount)
        {
            this.recipeItemName = recipeItemName;
            this.neededItemCount = neededItemCount;
        }

        public void SetInfos(EItemName recipeItemName, int neededItemCount)
        {
            this.recipeItemName = recipeItemName;
            this.neededItemCount = neededItemCount;
        }

        public int CompareTo(CraftingMaterialInfo other)
        {
            if (other == null || (int)RecipeItemName < (int)other.RecipeItemName)
            {
                return 1;
            }
            else if ((int)RecipeItemName == (int)other.RecipeItemName)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }

    [Serializable]
    public struct SItemRecipeResultInfo
    {
        public EItemName ResultItem;
        public int ItemCount;
    }
}
