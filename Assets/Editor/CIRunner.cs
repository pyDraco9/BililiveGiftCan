using System.IO;
using System.Linq;
using UnityEditor;

class CIRunner
{
    [MenuItem("Build/Windows Build")]
    static void Build()
    {
        string path = "build";
        string[] scenes = new string[] { "Assets/bilibili-live.unity" };

        FileUtil.DeleteFileOrDirectory(path);

        BuildPipeline.BuildPlayer(scenes, path + "/BililiveGiftCan.exe", BuildTarget.StandaloneWindows64, BuildOptions.CompressWithLz4HC);

        // FileUtil.DeleteFileOrDirectory(path + "/UnityCrashHandler64.exe");
        // new DirectoryInfo(path + "/Data/Managed").EnumerateFiles("*.xml").ToList().ForEach(x => x.Delete());
    }
}