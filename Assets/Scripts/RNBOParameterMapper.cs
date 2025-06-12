using UnityEngine;

public class RNBOParameterMapper : MonoBehaviour
{
    public RNBOgenHelper rnboHelper;
    private RNBOgenHandle rnboPlugin;

    [Tooltip("ID of the parameter to control (e.g. 'freq')")]
    public string rnboParameterID;

    [Tooltip("The game value that will be mapped (e.g. speed, health etc.)")]
    public float sourceValue;

    public float minValue = 0f;
    public float maxValue = 1f;

    private int? parameterIndex = null;

    void Start()
    {
        if (rnboHelper == null)
        {
            rnboHelper = RNBOgenHelper.FindById(1); // or 0 depending on your setup
        }

        if (rnboHelper != null)
        {
            rnboPlugin = rnboHelper.Plugin;
            parameterIndex = RNBOgenHandle.GetParamIndexById(rnboParameterID);
            Debug.Log($"Parameter index for '{rnboParameterID}': {parameterIndex}");
        }
        else
        {
            Debug.LogError("RNBO Helper not found!");
        }
    }

    void Update()
    {
        if (rnboPlugin == null || parameterIndex == null) return;

        float clamped = Mathf.Clamp(sourceValue, minValue, maxValue);
        rnboPlugin.SetParamValue((int)parameterIndex, clamped);
    }
}
