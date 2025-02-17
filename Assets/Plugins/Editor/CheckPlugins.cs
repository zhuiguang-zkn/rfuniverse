﻿# if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using System.Threading.Tasks;

public class CheckPlugins
{
    [MenuItem("RFUniverse/Check Plugins (Fix Error)")]
    private static async void FixError()
    {
        Debug.Log("Begin Check Plugins (Fix Error), Please wait a moment ");
        string[] packageNames =
        {
            "com.unity.editorcoroutines",
            "com.unity.textmeshpro",
            "com.unity.addressables",
            "com.unity.nuget.newtonsoft-json",
            "com.unity.barracuda",
            };
        string[] packageAddNames =
        {
            "com.unity.editorcoroutines",
            "com.unity.textmeshpro",
            "com.unity.addressables",
            "com.unity.nuget.newtonsoft-json",
            "com.unity.barracuda",
            };

        ListRequest listRequest = Client.List();
        while (!listRequest.IsCompleted)
        {
            await Task.Delay(100);
        }
        string[] packageList = listRequest.Result.Select((s) => s.name).ToArray();
        for (int i = 0; i < packageNames.Length; i++)
        {
            if (!packageList.Contains(packageNames[i]))
            {
                Debug.Log("Geting " + packageNames[i]);
                AddRequest addRequest = Client.Add(packageAddNames[i]);
                while (!addRequest.IsCompleted)
                {
                    await Task.Delay(100);
                }
            }
        }

        List<string> defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Split(';').ToList();

        bool exist = Directory.Exists($"{Application.dataPath}/Plugins/BioIK/Setup");
        if (exist && !defines.Contains("BIOIK"))
        {
            Debug.Log("BIOIK plugin detected,Add BIOIK DefineSymbols");
            defines.Add("BIOIK");
        }
        else if (!exist && defines.Contains("BIOIK"))
        {
            defines.Remove("BIOIK");
            Debug.Log("BIOIK plugin undetected,Remove BIOIK DefineSymbols");
        }

        if (Directory.Exists($"{Application.dataPath}/Plugins/BioIK/Setup/BioIK.cs"))
        {
            string tmpPath = $"{Application.dataPath}/Plugins/Editor/BioIK.cs.backup";
            string bioikPath = $"{Application.dataPath}/Plugins/BioIK/BioIK.cs";
            if (File.Exists(tmpPath))
            {
                File.Copy(tmpPath, bioikPath, true);
                Debug.Log("BioIK.cs modified");
            }
        }

        exist = Directory.Exists($"{Application.dataPath}/Plugins/Obi");
        if (exist && !defines.Contains("OBI"))
        {
            defines.Add("OBI");
            Debug.Log("OBI plugin detected,Add OBI DefineSymbols");
        }
        else if (!exist && defines.Contains("OBI"))
        {
            defines.Remove("OBI");
            Debug.Log("OBI plugin undetected,Remove OBI DefineSymbols");
        }

        if (defines.Contains("HYBRID_CLR"))
        {
            defines.Remove("HYBRID_CLR");
            Debug.Log("Remove HYBRID_CLR DefineSymbols");
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defines.ToArray());

        PlayerSettings.allowUnsafeCode = true;
        Debug.Log("PlayerSettings.allowUnsafeCode has been set to true");

        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_Unity_4_8);
        Debug.Log("PlayerSettings.ApiCompatibilityLevel has been set to NET_Unity_4_8");

        var folder = AssetDatabase.LoadAssetAtPath("Assets/RFUniverse", typeof(DefaultAsset));
        Selection.activeObject = folder;
        EditorApplication.ExecuteMenuItem("Assets/Reimport");
        Debug.Log("Check Plugins (Fix Error) Done");
    }
}
#endif