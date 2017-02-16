using UnityEngine.UI;

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
            SetComing(false);
        }

        public static void SetComing(bool v)
        {
            ComingToRealLocation = v;
            GameObject.Find("World")
                .GetComponent<UIManager>()
                .Joystick
                .GetComponent<Image>()
                .enabled = v;
        }
    }
}