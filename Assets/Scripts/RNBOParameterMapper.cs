using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(RNBOgenHelper))]
public class RNBOParameterMapper : MonoBehaviour
{
    [Tooltip("The parameter list asset generated from RNBO description.json")]
    public RNBOParameterList parameterList;

    [Tooltip("Index of the parameter in the list to bind")]
    public int parameterIndex;

    [Tooltip("Value to be mapped to the parameter (e.g., speed, health)")]
    public float gameplayValue;

    [Tooltip("Scale gameplay value into parameter range")]
    public bool normalizeValue = true;

    private RNBOgenHandle plugin;
    private int? paramIndex;

    void Start()
    {
        var helper = GetComponent<RNBOgenHelper>();
        plugin = helper.Plugin;

        if (parameterList == null || parameterList.parameters.Length == 0)
        {
            Debug.LogError("Parameter list not assigned or empty.");
            return;
        }

        if (parameterIndex < 0 || parameterIndex >= parameterList.parameters.Length)
        {
            Debug.LogError("Invalid parameter index.");
            return;
        }

        string paramId = parameterList.parameters[parameterIndex].paramId;
        paramIndex = RNBOgenHandle.GetParamIndexById(paramId);

        if (paramIndex == null)
        {
            Debug.LogError($"Parameter ID '{paramId}' not found in RNBO plugin.");
        }
    }

    void Update()
    {
        if (plugin == null || paramIndex == null) return;

        var paramData = parameterList.parameters[parameterIndex];
        float valueToSend = gameplayValue;

        if (normalizeValue)
        {
            valueToSend = Mathf.Clamp(valueToSend, paramData.min, paramData.max);
        }

        plugin.SetParamValue((int)paramIndex, valueToSend);
    }
}
