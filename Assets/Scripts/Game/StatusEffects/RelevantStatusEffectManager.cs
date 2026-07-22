using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class NamedStatusEffectPrefab
{
    public string effectName;
    public GameObject effectPrefab;
}

[System.Serializable]
public class StatusEffectBuildRecipe
{
    public string effectType;
    public List<NamedStatusEffectPrefab> namedPrefabs = new List<NamedStatusEffectPrefab>();
}

public class RelevantStatusEffectManager : MonoBehaviour
{

    public StatusEffect BuildEffectChainFromRecipe(StatusEffectBuildRecipe recipe)
    {
        if (recipe == null || recipe.namedPrefabs.Count == 0)
        {
            Debug.LogWarning("Invalid or empty recipe.");
            return null;
        }

        StatusEffect baseEffect = null;
        StatusEffect topEffect = null;

        foreach (var namedPrefab in recipe.namedPrefabs)
        {
            
            GameObject prefab = namedPrefab.effectPrefab;

            if (prefab == null)
            {
                Debug.LogWarning("Null prefab in recipe.");
                continue;
            }

            GameObject go = Instantiate(prefab);
            StatusEffect effect = go.GetComponent<StatusEffect>();

            if (effect == null)
            {
                Debug.LogWarning($"Prefab '{prefab.name}' does not contain a StatusEffect component.");
                Destroy(go);
                continue;
            }

            if (baseEffect == null)
            {
                baseEffect = effect;
                topEffect = effect;
                continue;
            }

            if (effect is StatusEffectDecorator decorator)
            {
                decorator.SetBaseEffect(topEffect);
                topEffect.transform.SetParent(decorator.transform);
                topEffect = decorator;
            }
            else
            {
                Destroy(go);
            }
        }

        if (topEffect.GetBaseEffectType() != recipe.effectType)
        {
            Debug.LogWarning($"Base effect ({topEffect.GetBaseEffectType()}) does not match recipe ({recipe.effectType})!");
        }

        return topEffect;
    }

    private void UnwrapNamedPrefab(NamedStatusEffectPrefab namedPrefab)
    {
        GameObject prefab = namedPrefab.effectPrefab;
        if (prefab == null)
        {
            // Debug.Log("Null prefab...");
            return;
        }
        GameObject go = Instantiate(prefab);
        StatusEffect effect = go.GetComponent<StatusEffect>();
        if (effect == null)
        {
            // Debug.Log("Null effect...");
            Destroy(go);
            return;
        }
        namedPrefab.effectName = effect.GetEffectType();
        nameToPrefab[namedPrefab.effectName] = namedPrefab.effectPrefab;
        // Debug.Log(namedPrefab.effectName);
        Destroy(go);
    }

    private void UnwrapRecipe(StatusEffectBuildRecipe recipe)
    {
        // Debug.Log("Building " + recipe);
        foreach (var namedPrefab in recipe.namedPrefabs)
        {   
            UnwrapNamedPrefab(namedPrefab);
        }
    }

    [SerializeField] private List <StatusEffectBuildRecipe> recipes = new List<StatusEffectBuildRecipe>();
    [SerializeField] private List <NamedStatusEffectPrefab> extras = new List<NamedStatusEffectPrefab>();
    private Dictionary <string, StatusEffectBuildRecipe> recipeDict = new Dictionary<string, StatusEffectBuildRecipe>();
    private Dictionary <string, GameObject> nameToPrefab = new Dictionary<string, GameObject>();

	public static RelevantStatusEffectManager Instance { get; private set; }

    private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

        foreach (var recipe in recipes)
        {
            UnwrapRecipe(recipe);
            StatusEffect temp = BuildEffectChainFromRecipe(recipe);
            recipeDict[temp.GetBaseEffectType()] = recipe;
            Destroy(temp.gameObject);
        }

        foreach (NamedStatusEffectPrefab namedPrefab in extras)
        {
            UnwrapNamedPrefab(namedPrefab);
        }

	}

    public GameObject FindRelevantStatusEffectPrefab(string effectName)
    {

        GameObject test = BuildEffectChainFromRecipe(recipeDict[effectName]).gameObject;

        return test;
    }

    public void RemoveDecorator(string effectName, GameObject decoratorPrefab)
    {
        if (decoratorPrefab == null || string.IsNullOrEmpty(effectName) || !recipeDict.ContainsKey(effectName))
            return;

        StatusEffectBuildRecipe recipe = recipeDict[effectName];

        int removed = recipe.namedPrefabs.RemoveAll(
            entry => entry.effectPrefab == decoratorPrefab
        );

        if (removed == 0)
            Debug.LogWarning($"Decorator '{decoratorPrefab.name}' not found in recipe '{effectName}'.");
    }

    private void AddDecoratorOffset(string effectName, GameObject decoratorPrefab, GameObject relativeDecoratorPrefab, int offset)
    {
        if (decoratorPrefab == null || string.IsNullOrEmpty(effectName) || relativeDecoratorPrefab == null || !recipeDict.ContainsKey(effectName))
        {
            return;
        }

        StatusEffectBuildRecipe recipe = recipeDict[effectName];

        int relativeEffectIndex = -1;
        for (int i = 0; i < recipe.namedPrefabs.Count; i++)
        {
            if (recipe.namedPrefabs[i].effectPrefab == relativeDecoratorPrefab)
            {
                relativeEffectIndex = i;
                break;
            }
        }

        if (relativeEffectIndex == -1)
        {
            Debug.LogWarning($"Effect '{relativeDecoratorPrefab}' not found in recipe '{effectName}'.");
            return;
        }

        GameObject go = Instantiate(decoratorPrefab);
        StatusEffect effect = go.GetComponent<StatusEffect>();

        if (effect == null)
        {
            Debug.LogWarning("No effect found!");
            return;
        }

        recipe.namedPrefabs.Insert(relativeEffectIndex + offset, new NamedStatusEffectPrefab
        {
            effectName = effect.GetEffectType(),
            effectPrefab = decoratorPrefab
        });

        Destroy(go);
    }

    public void AddDecoratorAfter(string effectName, GameObject decoratorPrefab, GameObject relativeDecoratorPrefab)
    {
        AddDecoratorOffset(effectName, decoratorPrefab, relativeDecoratorPrefab, 1);
    }

    public void AddDecoratorBefore(string effectName, GameObject decoratorPrefab, GameObject relativeDecoratorPrefab)
    {
        AddDecoratorOffset(effectName, decoratorPrefab, relativeDecoratorPrefab, 0);
    }

    public GameObject GetPrefab(string name)
    {
        return nameToPrefab[name];
    }

}