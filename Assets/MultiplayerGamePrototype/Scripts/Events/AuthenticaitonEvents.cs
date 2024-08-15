using System;


namespace MultiplayerGamePrototype.Events
{
    public static class AuthenticaitonEvents
    {
        public static Action<string> SignInAnonymously;
        public static Action OnCompletedSignedIn;
        public static Action OnFailedSignedIn;
    }
}