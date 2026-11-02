using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFlow
{
    private MonoBehaviour bootstrap;
    private ScreenFader fader;
    private GameServices services;

    public SceneFlow(MonoBehaviour bootstrap, ScreenFader fader, GameServices services)
    {
        this.bootstrap = bootstrap;
        this.fader = fader;
        this.services = services;
    }

    public void GoTo(string sceneName, System.Action onComplete = null)
    {
        bootstrap.StartCoroutine(LoadSceneAsync(sceneName, onComplete));
    }

    private IEnumerator LoadSceneAsync(string sceneName, System.Action onComplete)
    {
        if (fader != null) yield return bootstrap.StartCoroutine(fader.FadeOut());

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!op.isDone) yield return null;

        SceneInstaller installer = Object.FindObjectOfType<SceneInstaller>();
        if (installer != null)
        {
            installer.Install(services);
        }

        if (fader != null) yield return bootstrap.StartCoroutine(fader.FadeIn());
        onComplete?.Invoke();
    }

    public void GoToBattle(System.Action onBattleEnded)
    {
        services.Battle.SetFinishCallback(result =>
        {
            GoTo("Scn_World", onBattleEnded);
        });
        GoTo("Scn_Battle");
    }
}