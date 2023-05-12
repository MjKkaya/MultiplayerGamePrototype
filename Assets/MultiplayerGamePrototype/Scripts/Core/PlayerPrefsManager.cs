using MultiplayerGamePrototype.Utilities;
using UnityEngine;


namespace MultiplayerGamePrototype.Core
{
    public class PlayerPrefsManager : SingletonMonoPersistent<PlayerPrefsManager>
    {
        public static readonly string EDITOR_LAST_OPENED_SCENE = "EK1";

        public static readonly string PLAYER_USERNAME_KEY = "PK1";


        public void SetString(string key, string stringValue)
        {
            PlayerPrefs.SetString(key, stringValue);
            PlayerPrefs.Save();
        }

        public string GetString(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public void SetInt(string key, int intValue)
        {
            PlayerPrefs.SetInt(key, intValue);
            PlayerPrefs.Save();
        }
    }
}
