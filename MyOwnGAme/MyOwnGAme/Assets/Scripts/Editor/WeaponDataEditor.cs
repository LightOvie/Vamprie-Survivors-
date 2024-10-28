using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;



[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    WeaponData weaponData;
    string[] weaponSubtypes;
    int selectedWeaponSubtype;

    public virtual void RecompileWeaponSubTypes()
    {
        //Cache the weapon data value 
        weaponData = (WeaponData)target;
        //Retrieve all the weapon subtypes and cache it.
        System.Type baseType = typeof(Weapon);
        List<System.Type> subTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => baseType.IsAssignableFrom(p) && p != baseType)
            .ToList();
        //Add a None option in front.
        List<string> subTypesString = subTypes.Select(t => t.Name).ToList();
        subTypesString.Insert(0, "None");
        weaponSubtypes = subTypesString.ToArray();

        //Ensure that we are using the correct weapon subtype.
        selectedWeaponSubtype = Math.Max(0, Array.IndexOf(weaponSubtypes, weaponData.behaviour));

    }


    public override void OnInspectorGUI()
    {

		if (weaponSubtypes == null || weaponSubtypes.Length == 0)
		{
			RecompileWeaponSubTypes();
		}
		//Adds a lis tof all weapon subclasses asa a dropdown

		selectedWeaponSubtype = EditorGUILayout.Popup("Behaviour", Math.Max(0, selectedWeaponSubtype), weaponSubtypes);

            if (selectedWeaponSubtype > 0)
            {
                //Updates the behaviour field 
                weaponData.behaviour = weaponSubtypes[selectedWeaponSubtype].ToString();
                EditorUtility.SetDirty(weaponData); // Marks the object to save.
                DrawDefaultInspector();//Draw the default inspector elements


            }


    }
}