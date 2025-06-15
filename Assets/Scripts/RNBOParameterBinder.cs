using UnityEngine;
using System.Reflection;

[RequireComponent(typeof(RNBOgenHelper))]
public class RNBOParameterBinder : MonoBehaviour
{
    public RNBOParameterList parameterList;
    public int parameterIndex = 0;

    public MonoBehaviour targetScript;
    public string targetFieldOrProperty;

    public bool normalizeValue = true;
    public bool updateEveryFrame = true;

    private RNBOgenHandle plugin;
    private FieldInfo field;
    private PropertyInfo property;
    private RNBOParameterList.Parameter param;

    void Start()
    {
        var helper = GetComponent<RNBOgenHelper>();
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
    }

    void Update()
    {
        if (updateEveryFrame)
        {
            ApplyValue();
        }
    }

    public void ApplyValue()
    {
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
            Debug.LogError($"Could not find parameter index for ID '{param.paramId}'");
            return;
        }

        if (normalizeValue)
        {
            value = Mathf.InverseLerp(param.min, param.max, value);
            plugin.SetParamValueNormalized(index.Value, value);
        }
        else
        {
            plugin.SetParamValue(index.Value, value);
        }

    }
}
