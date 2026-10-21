using Foundation;

namespace MauiLearn;

/// <summary>
/// Necessário para suportar múltiplas janelas no Mac Catalyst
/// (Application.Current.OpenWindow). Veja UIApplicationSceneManifest no Info.plist.
/// </summary>
[Register("SceneDelegate")]
public class SceneDelegate : MauiUISceneDelegate
{
}
