using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

namespace CommonRPG
{
    
    [CreateAssetMenu(fileName = "CraftingRecipeData", menuName = "ScriptableObjects/CraftingRecipeDataScriptableObject", order = 5)]
    public class CraftingRecipeDataScriptableObject : ScriptableObject
    {
        public const int ITEM_COUNT_MULTIPLIER = 10000;

        [SerializeField]
        private List<CraftingRecipe> craftingRecipes;
        /// <summary>
        /// int : recipeKey == (int)EItemName + NeededItemCount * ITEM_COUNT_MULTIPLIER, CraftingRecipeNode : TotalRecipeInfo..
        /// </summary>
        private Dictionary<int, CraftingRecipeNode> recipeTable = new Dictionary<int, CraftingRecipeNode>();
        public Dictionary<int, CraftingRecipeNode> RecipeTable 
        { 
            get 
            {
                //ArrangeRecipes();
                return recipeTable; 
            } 
        }

        private Stack<CraftingRecipeNode> dFSStack = new Stack<CraftingRecipeNode>();

        public void PrintCurrentTable()
        {
            Debug.Log("Print Start====================");
            foreach (CraftingRecipeNode node in recipeTable.Values) 
            {
                dFSStack.Clear();
                dFSStack.Push(node);

                while (dFSStack.Count > 0)
                {
                    CraftingRecipeNode current = dFSStack.Pop();

                    if (current.CraftingMaterialInfo == null) 
                    {
                        Debug.Log($"Result : {current.ResultItemName}");
                    }
                    else
                    {
                        Debug.Log($"item : {current.CraftingMaterialInfo.RecipeItemName}, count : {current.CraftingMaterialInfo.NeededItemCount}");
                    }
                    
                    for (int i = 0; i < current.Children.Count; ++i)
                    {
                        dFSStack.Push(current.Children[i]);
                    }
                }
            }
            Debug.Log("Print End====================");
        }

        private void SortRecipes()
        {
            foreach (CraftingRecipe craftingRecipe in craftingRecipes)
            {
                craftingRecipe.Materials.Sort();
            }
        }

        public void ArrangeRecipes()
        {
            SortRecipes();

            recipeTable.Clear();

            foreach (CraftingRecipe craftingRecipe in craftingRecipes)
            {
                List<CraftingMaterialInfo> craftingMaterials = craftingRecipe.Materials;
                CraftingMaterialInfo craftingMaterial = craftingMaterials[0];

                int recipeKey = (int)craftingMaterial.RecipeItemName + craftingMaterial.NeededItemCount * ITEM_COUNT_MULTIPLIER;

                if (recipeTable.ContainsKey(recipeKey)) 
                {
                    CraftingRecipeNode craftingRecipeNode = recipeTable[recipeKey];

                    if (craftingRecipeNode.CraftingMaterialInfo != craftingMaterial) 
                    {
                        Debug.LogAssertion("Weird operation dectected... craftingRecipeNode.CraftingMaterialInfo != craftingMaterial");
                        return;
                    }

                    dFSStack.Clear();
                    dFSStack.Push(craftingRecipeNode);

                    int craftingMaterialIndex = 0;

                    while (dFSStack.Count > 0)
                    {
                        craftingRecipeNode = dFSStack.Pop();

                        if (craftingRecipeNode.CraftingMaterialInfo == craftingMaterial) 
                        {
                            craftingMaterialIndex++;

                            if (craftingMaterial.RecipeItemName != EItemName.None && craftingMaterialIndex == craftingMaterials.Count) 
                            {
                                // totally same recipe
                                break;
                            }

                            for (int i = 0; i < craftingRecipeNode.Children.Count; ++i)
                            {
                                dFSStack.Push(craftingRecipeNode.Children[i]);
                            }

                            if (craftingMaterialIndex < craftingMaterials.Count) 
                            {
                                craftingMaterial = craftingMaterials[craftingMaterialIndex];
                            }
                            else
                            {
                                // add Result Node
                                CraftingRecipeNode resultNode = new CraftingRecipeNode(craftingRecipe.ResultItemName);
                                craftingRecipeNode.Children.Add(resultNode);
                                
                                break;
                            }
                        }
                        else
                        {
                            // add remaining node.
                            craftingRecipeNode = craftingRecipeNode.ParentNode;

                            int craftingMaterialsCount = craftingMaterials.Count;
                            for (int i = craftingMaterialIndex; i < craftingMaterialsCount; ++i)
                            {
                                CraftingRecipeNode nextCraftingRecipeNode = new CraftingRecipeNode(craftingMaterials[i]);
                                craftingRecipeNode.Children.Add(nextCraftingRecipeNode);
                                nextCraftingRecipeNode.ParentNode = craftingRecipeNode;
                                craftingRecipeNode = nextCraftingRecipeNode;
                            }

                            CraftingRecipeNode lastCraftingRecipeNode = new CraftingRecipeNode(craftingRecipe.ResultItemName);
                            craftingRecipeNode.Children.Add(lastCraftingRecipeNode);
                            
                            break;
                        }
                    }
                }
                else
                {
                    CraftingRecipeNode craftingRecipeNode = new CraftingRecipeNode(craftingMaterial);
                    recipeTable.Add(recipeKey, craftingRecipeNode);

                    int craftingMaterialsCount = craftingMaterials.Count;
                    for (int i = 1; i < craftingMaterialsCount; ++i)
                    {
                        CraftingRecipeNode nextCraftingRecipeNode = new CraftingRecipeNode(craftingMaterials[i]);
                        craftingRecipeNode.Children.Add(nextCraftingRecipeNode);
                        nextCraftingRecipeNode.ParentNode = craftingRecipeNode;
                        craftingRecipeNode = nextCraftingRecipeNode;
                    }

                    CraftingRecipeNode lastCraftingRecipeNode = new CraftingRecipeNode(craftingRecipe.ResultItemName);
                    craftingRecipeNode.Children.Add(lastCraftingRecipeNode);
                }
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

        [SerializeField]
        private EItemName resultItemName = EItemName.None;
        public EItemName ResultItemName { get { return resultItemName; } }
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

        public int CompareTo(CraftingMaterialInfo other)
        {
            if (other == null || (int)RecipeItemName < (int)other.RecipeItemName)
            {
                return -1;
            }
            else if ((int)RecipeItemName == (int)other.RecipeItemName) 
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public static bool operator ==(CraftingMaterialInfo a, CraftingMaterialInfo b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            {
                return true;
            }
                
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }
             
            return (a.RecipeItemName == b.RecipeItemName) && (a.neededItemCount == b.neededItemCount);
        }

        public static bool operator !=(CraftingMaterialInfo a, CraftingMaterialInfo b)
        {
            return (a == b) == false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            CraftingMaterialInfo other = (CraftingMaterialInfo)obj;
            return RecipeItemName == other.RecipeItemName;
        }

        public override int GetHashCode()
        {
            return neededItemCount;
        }
    }

    public class CraftingRecipeNode
    {
        private CraftingMaterialInfo craftingMaterialInfo = null;
        public CraftingMaterialInfo CraftingMaterialInfo { get { return craftingMaterialInfo; } }

        private EItemName resultItemName = EItemName.None;
        public EItemName ResultItemName { get { return resultItemName; } }

        private CraftingRecipeNode parentNode = null;
        public CraftingRecipeNode ParentNode { get { return parentNode; } set { parentNode = value; } }

        private List<CraftingRecipeNode> children = new List<CraftingRecipeNode>();
        public List<CraftingRecipeNode> Children { get { return children; } }

        public CraftingRecipeNode(EItemName resultItemName)
        {
            this.resultItemName = resultItemName;
        }

        public CraftingRecipeNode(CraftingMaterialInfo newCraftingMaterialInfo)
        {
            craftingMaterialInfo = newCraftingMaterialInfo;
        }

        public void AddChildren(CraftingRecipeNode newNode)
        {
            children.Add(newNode);
        }

        public CraftingRecipeNode MoveToChildNode(int childIndex)
        {
            return children[childIndex];
        }
    }
}
