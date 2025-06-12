using UnityEngine;

[CreateAssetMenu(menuName = "RNBO/Parameter List")]
public class RNBOParameterList : ScriptableObject
{
    public Parameter[] parameters;

    [System.Serializable]
    public class Parameter
    {
        public string paramId;
        public float min;
        public float max;
        public float defaultValue;
    }
}
