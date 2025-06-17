using UnityEngine;
using System.Reflection;

[RequireComponent(typeof(RNBOgenHelper))]
public class RNBOParameterBinder : MonoBehaviour
{
    public RNBOParameterList parameterList;
    public int parameterIndex = 0;

    public MonoBehaviour targetScript;
    public string targetFieldOrProperty;

    public string inputMinField = "";
    public string inputMaxField = "";

    private RNBOgenHandle plugin;
    private FieldInfo field;
    private PropertyInfo property;
    private RNBOParameterList.Parameter param;

    private float inputMin = 0f;
    private float inputMax = 1f;

    void Start()
    {
        var helper = RNBOgenHelper.FindById(0);
        plugin = helper.Plugin;

        if (parameterList == null || parameterIndex < 0 || parameterIndex >= parameterList.parameters.Length)
        {
            Debug.LogError("Invalid parameter selection.");
            return;
        }

        param = parameterList.parameters[parameterIndex];

        if (targetScript != null && !string.IsNullOrEmpty(targetFieldOrProperty))
        {
            field = targetScript.GetType().GetField(targetFieldOrProperty);
            property = targetScript.GetType().GetProperty(targetFieldOrProperty);

            if (field == null && property == null)
            {
                Debug.LogError("Could not find the field or property on target script.");
            }
        }

        AutoDetectInputRange();
    }

    void Update()
    {
        ApplyValue();
    }

    public void ApplyValue()
    {
        Debug.Log(plugin);
        Debug.Log(param);
        if (plugin == null || param == null) return;

        float value = 0f;

        if (field != null)
        {
            value = (float)field.GetValue(targetScript);
        }
        else if (property != null)
        {
            value = (float)property.GetValue(targetScript);
        }
        else
        {
            Debug.LogWarning("No field or property reference available to fetch value.");
            return;
        }

        int? index = RNBOgenHandle.GetParamIndexById(param.paramId);
        if (!index.HasValue)
        {
            Debug.LogWarning($"Could not find index for parameter ID {param.paramId}");
            return;
        }

        // Clamp input value first for safety
        value = Mathf.Clamp(value, inputMin, inputMax);

        // Normalize based on detected or manually set range
        float t = Mathf.InverseLerp(inputMin, inputMax, value);
        // Map to parameter's defined min/max
        float mapped = Mathf.Lerp(param.min, param.max, t);
        Debug.Log(mapped);
        plugin.SetParamValue(index.Value, mapped);
    }

    private void AutoDetectInputRange()
    {
        if (targetScript == null) return;

        FieldInfo minField = !string.IsNullOrEmpty(inputMinField) ? targetScript.GetType().GetField(inputMinField) : null;
        FieldInfo maxField = !string.IsNullOrEmpty(inputMaxField) ? targetScript.GetType().GetField(inputMaxField) : null;
    
    
        if (minField != null && maxField != null)
        {
            inputMin = (float)minField.GetValue(targetScript);
            inputMax = (float)maxField.GetValue(targetScript);
            Debug.Log($"Auto-detected input range: {inputMin} to {inputMax}");
        }
        else
        {
            Debug.Log("Could not auto-detect input range. Defaulting to 0–1. You can manually override in inspector.");
        }
    }
}
