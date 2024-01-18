using System.Collections;
using System.Collections.Generic;
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
            CharacterData[] characters = Resources.FindObjectsOfTypeAll<CharacterData>();
            if (characters.Length > 0)
            {
                return characters[Random.Range(0, characters.Length)];
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
