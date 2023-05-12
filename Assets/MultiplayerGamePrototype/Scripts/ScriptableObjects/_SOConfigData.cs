using UnityEngine;


namespace MultiplayerGamePrototype.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ConfigData", menuName = "MultiplayerGamePrototype/Data/ConfigData")]
    public class SOConfigData : ScriptableObject
    {
        [Header("Editor Config")]
        [Header("Tags")]
        public string Tag_Targets = "Targets";
    }
}
