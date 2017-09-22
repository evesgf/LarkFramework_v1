using LarkFramework.Config;
using LarkFramework.Module;
using LarkFramework.Scenes;
using LarkFramework.UI;
using Project;
using LarkFramework.Console;
using LarkFramework.Tick;
using LarkFramework.Audio;

namespace LarkFramework.GameEntry
{
    public partial class GameEntry
    {
        public void InitBuiltinComponents()
        {
            //InitTick
            TickManager.Instance.Init();

            Console.Console.Create();

            //InitConfig
            AppConfig.Init();

            ModuleManager.Instance.Init("Project");

            //InitModule
            //Init Resources
            SingletonMono<ResourcesMgr.ResourcesMgr>.Create();

            //Init Scene
            ScenesManager.Instance.Init("Scene/");
            ScenesManager.MainScene = SceneDef.MenuScene;
            if (lanuchType == LaunchType.Debug && !string.IsNullOrEmpty(startScene))
            {
                ScenesManager.Instance.LoadScene(startScene);
            }

            //Init UI
            UIManager.Instance.Init("UI/");
            UIManager.MainPage = UIDef.MenuPage;
            if (lanuchType == LaunchType.Debug && !string.IsNullOrEmpty(startUI))
            {
                UIManager.Instance.OpenPage(startUI, null);
            }

            //Init Audio
            AudioManager.Instance.Init("Audio/");
            if (lanuchType == LaunchType.Debug && !string.IsNullOrEmpty(startAudio))
            {
                AudioManager.Instance.PlayBGM(AudioDef.BGM_MainBGM,0.2f);
            }
        }
    }
}
