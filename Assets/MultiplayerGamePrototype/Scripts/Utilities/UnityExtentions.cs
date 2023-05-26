using System.Collections.Generic;
using System.Text;
using Unity.Services.Lobbies.Models;
using UnityEngine;


namespace MultiplayerGamePrototype.Utilities
{
    public static class UnityExtentions
    {
        #region Canvas Group

        public static void SetActive(this CanvasGroup canvasGroup, bool isActive)
        {
            canvasGroup.alpha = isActive ? 1 : 0;
            canvasGroup.blocksRaycasts = isActive;
            canvasGroup.interactable = isActive;
        }

        #endregion


        #region List

        public static string ToStringFull<T>(this List<T> list)
        {
            if (list == null)
                return "-Null-";
            else if (list.Count == 0)
                return "-Empty-";

            StringBuilder fullLog = new("\n");
            int len = list.Count;
            for (int i = 0; i < len; i++)
            {
                fullLog.AppendFormat("{0} : {1} \n", i, list[i].ToString());
            }
            return fullLog.ToString();
        }

        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            return list == null || list.Count < 1;
        }

        #endregion


        #region UGS

        #region Player

        public static string ToStringFull(this Player player)
        {
            StringBuilder fullLog = new();
            fullLog.Append($"Id:{player.Id}\n");
            fullLog.Append($"AllocationId:{player.AllocationId}\n");
            fullLog.Append($"ConnectionInfo:{player.ConnectionInfo}\n");

            if (player.Data == null)
                fullLog.Append($"Data:-Null-\n");
            else
            {
                fullLog.Append($"Data.Count:{player.Data.Count}\n");
                fullLog.Append($"Data=>\n");
                foreach (var item in player.Data)
                {
                    fullLog.Append($"{item.Key}=>{item.Value.ToStringFull()}\n");
                }
            }

            fullLog.Append($"Profile:{player.Profile}\n");
            return fullLog.ToString();
        }

        public static string ToStringFull(this PlayerDataObject playerDataObject)
        {
            return $"Value:{playerDataObject.Value}, Vis: {playerDataObject.Visibility}";
        }

        #endregion

        #endregion
    }
}