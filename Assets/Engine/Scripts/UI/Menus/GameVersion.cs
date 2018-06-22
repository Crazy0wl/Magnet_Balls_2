using UnityEngine;

namespace MB_Engine
{
    public class GameVersion : MonoBehaviour
    {
        public UILabel VersionText;

        private void Start()
        {
            VersionText.text = string.Format("v {0}", Application.version);
        }
    }
}
