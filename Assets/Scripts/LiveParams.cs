namespace UnityEngine
{
    class LiveParams
    {
        public static bool TeleportEnabled = false;
        public static bool NavigationEnabled = false;
        public static bool ComingToRealLocation = false;

        public static void InitFields()
        {
            TeleportEnabled = false;
            NavigationEnabled = false;
            ComingToRealLocation = false;
        }
    }
}