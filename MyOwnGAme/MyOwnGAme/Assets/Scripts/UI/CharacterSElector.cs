using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterSElector : MonoBehaviour
{
	public static CharacterSElector instance;
	public CharacterData characterData;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Debug.LogWarning("EXTRA" + this + "DELETED");
			Destroy(gameObject);
		}
	}


	public static CharacterData GetData()
	{
		if (instance && instance.characterData)
		{
			return instance.characterData;

		}
		else
		{

			string[] allAssetsPaths = AssetDatabase.GetAllAssetPaths();
			List<CharacterData> characters = new List<CharacterData>();
			foreach (string  assetPath  in allAssetsPaths)
			{
				if (assetPath.EndsWith(".asset"))
				{
					CharacterData characterData = AssetDatabase.LoadAssetAtPath<CharacterData>(assetPath);
					if (characterData!=null)
					{
						characters.Add(characterData);
						
					}
				}
			}

			if (characters.Count> 0)
			{
				return characters[Random.Range(0, characters.Count)];
			}
		}
		return null;
	}

	public void SelectCharacter(CharacterData character)
	{
		characterData = character;
	}

	public static void DestroySingelton()
	{
		if (instance)
		{
			Destroy(instance.gameObject);
		}

	}
}
