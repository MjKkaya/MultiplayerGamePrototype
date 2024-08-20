using System;


namespace MultiplayerGamePrototype.Events
{
    public static class LobbyEvents
    {
        public static Action<string> Create;
        public static Action<int> OnCompletedCreation;
        public static Action OnFailedCreation;

        public static Action<string> Join;
        public static Action OnCompletedJoin;
        public static Action OnFailedJoin;

        public static Action QuickJoin;
        // public static Action OnFailedQuickJoin;

        public static Action<string> Leave;

        //When the player left the lobby, this event handler triggers. 
        public static Action OnLeft;

        /// <summary>
        /// First parameter is playerId second is score.
        /// </summary>
        public static Action<string, string> OnChangedPlayerScore;
    }
}