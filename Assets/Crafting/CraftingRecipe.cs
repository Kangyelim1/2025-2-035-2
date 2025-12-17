using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Recipe", menuName = "조합법 생성")]

public class CraftingRecipe : ScriptableObject
{
    [Serializable]
    public struct Ingredient
    {
        public ItemType type;
        public int count;
    }

    [Serializable]
    public struct product
    {
        public ItemType type;
        public int count;
    }

    [Header("결과물 외형")]
    public GameObject resultPrefab; // 이 레시피로 제작된 무기의 프리팹

    public string displayName;
    public List<Ingredient> inputs = new();
    public List<product> outputs = new();
}

