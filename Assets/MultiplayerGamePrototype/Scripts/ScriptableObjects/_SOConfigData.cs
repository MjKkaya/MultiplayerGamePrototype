using MultiplayerGamePrototype.Core;
using UnityEngine;


namespace MultiplayerGamePrototype.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ConfigData", menuName = "MultiplayerGamePrototype/Data/ConfigData")]
    public class SOConfigData : ScriptableObjectSingleton<SOConfigData>
    {
        [Header("Editor Config")]
        [Header("Tags")]
        public string Tag_Targets = "Targets";
    }
}
