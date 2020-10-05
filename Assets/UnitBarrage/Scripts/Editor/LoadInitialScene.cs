#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace hatuxes.ECSShootingProttype
{
    /// <summary>
    /// 初期ゲーム画面のシーンを一気にロードするエディタ拡張
    /// </summary>
    public class LoadInitialScene
    {
        [MenuItem("Window/LoadInitialScene")]
        private static void Load()
        {
            EditorSceneManager.OpenScene("Assets/UnitBarrage/Scenes/BaseScene.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene("Assets/UnitBarrage/Scenes/TitleScene.unity", OpenSceneMode.Additive);
        }
    }
}