using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class RNBOParamImporter : EditorWindow
{
    TextAsset rnboDescriptionJson;
    RNBOParameterList targetAsset;

    [MenuItem("Tools/RNBO/Import Parameter List")]
    public static void ShowWindow()
    {
        GetWindow<RNBOParamImporter>("RNBO Parameter Importer");
    }

    void OnGUI()
    {
        GUILayout.Label("Import RNBO Parameters", EditorStyles.boldLabel);

        rnboDescriptionJson = (TextAsset)EditorGUILayout.ObjectField("Description JSON", rnboDescriptionJson, typeof(TextAsset), false);
        targetAsset = (RNBOParameterList)EditorGUILayout.ObjectField("Parameter List Asset", targetAsset, typeof(RNBOParameterList), false);

        if (GUILayout.Button("Import Parameters"))
        {
            if (rnboDescriptionJson == null || targetAsset == null)
            {
                Debug.LogError("Please assign both a description.json and a parameter list asset.");
                return;
            }

            ImportParameters();
        }
    }

    void ImportParameters()
{
    try
    {
        JObject root = JObject.Parse(rnboDescriptionJson.text);
        JArray paramArray = (JArray)root["parameters"];

        if (paramArray == null)
        {
            Debug.LogError("❌ Could not find 'parameters' array in JSON.");
            return;
        }

        List<RNBOParameterList.Parameter> paramList = new List<RNBOParameterList.Parameter>();

        foreach (var item in paramArray)
        {
                string type = item["type"]?.ToString();
                if (type != "ParameterTypeNumber")
                    continue; // skip signal-type params

                string paramId = item["paramId"]?.ToString();
                float min = item["minimum"]?.Value<float>() ?? 0f;
                float max = item["maximum"]?.Value<float>() ?? 1f;
                float defaultVal = item["initialValue"]?.Value<float>() ?? 0f;

                if (!string.IsNullOrEmpty(paramId))
                {
                    RNBOParameterList.Parameter param = new RNBOParameterList.Parameter
                    {
                        paramId = paramId,
                        min = min,
                        max = max,
                        defaultValue = defaultVal
                    };

                    paramList.Add(param);
                }
            }


        targetAsset.parameters = paramList.ToArray();
        EditorUtility.SetDirty(targetAsset);
        AssetDatabase.SaveAssets();

        Debug.Log($"✅ Imported {paramList.Count} parameters into RNBOParameterList.");
    }
    catch (System.Exception e)
    {
        Debug.LogError("❌ Exception while importing: " + e.Message);
    }
}

    [System.Serializable]
    private class ParameterRaw
    {
        public string paramId;
        public float minimum;
        public float maximum;
        public float initialValue;
    }

    [System.Serializable]
    private class Wrapper
    {
        public ParameterRaw[] parameters;
    }
}
