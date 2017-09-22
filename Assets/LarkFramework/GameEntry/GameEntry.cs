using UnityEngine;

namespace LarkFramework.GameEntry
{
    public partial class GameEntry : MonoBehaviour
    {
        public LaunchType lanuchType = LaunchType.Debug;
        public string startScene;
        public string startUI;
        public string startAudio;

        public enum LaunchType
        {
            Debug = 1,
            Release = 2,
        }

        // Use this for initialization
	    void Awake()
        {
            Init();
        }

        public void Init()
        {
            switch (lanuchType)
            {
                case LaunchType.Debug:
                    DebugLaunch();
                    break;

                case LaunchType.Release:
                    ReleaseLaunch();
                    break;
            }

            InitBuiltinComponents();
            InitCustomComponents();

            DontDestroyOnLoad(gameObject);
        }

        private void DebugLaunch()
        {
            Debuger.EnableLog = true;
        }

        private void ReleaseLaunch()
        {
            Debuger.EnableLog = false;
        }
    }

}
