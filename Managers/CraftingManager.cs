using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CraftingManager : Singleton<CraftingManager>
{
    private List<RecipeData> recipes;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (instance == this)
                Destroy(Instance);
        }

        recipes = new List<RecipeData>();
        LoadRecipes();
    }

    private void LoadRecipes()
    {
        int index = 0;
        string fileName = "Recipe";
        while (true)
        {
            index++;
            string path = "Assets/Scripts/ScriptableObject/Data/Recipe/" + fileName + index.ToString("D3") +".asset";
            RecipeData data = AssetDatabase.LoadAssetAtPath<RecipeData>(path);
            if (data == null) break;
            recipes.Add(data);
        }        
    }

    public RecipeData[] GetRecipes()
    {
        return recipes.ToArray();
    }
}
