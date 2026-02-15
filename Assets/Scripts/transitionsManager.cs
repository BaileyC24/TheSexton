using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
public class transitionsManager : MonoBehaviour
{

    // Singleton pattern to ensure only one instance of transitionsManager exists
    public static transitionsManager instance;

    public Slider progressBar;
    public GameObject transitionsContainer;

    private sceneTransitions[] transitions;
    private void Awake()
    {
        // If an instance of transitionsManager already exists, destroy this new one
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // If an instance already exists, destroy this new one to maintain the singleton pattern
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Get all sceneTransitions components from the transitionsContainer
        transitions = transitionsContainer.GetComponentsInChildren<sceneTransitions>();
    }

    public void LoadScene(string sceneName, string transitionName)
    {
        // Start the scene transition process for the specified scene
        StartCoroutine(LoadSceneAsync(sceneName, transitionName));
    }

    private IEnumerator LoadSceneAsync(string sceneName, string transitionName)
    {
        sceneTransitions transition = transitions.First(t => t.name == transitionName);

        // Start loading the scene asynchronously but do not allow it to activate until the transition is complete
        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        // Animate the transition in
        yield return transition.AnimateTransitionIn();
        // Show the progress bar while the scene is loading
        progressBar.gameObject.SetActive(true);

        // Update the progress bar until the scene is fully loaded (progress reaches 0.9)
        do
        {
            progressBar.value = scene.progress;
            yield return null;
        } while (scene.progress < 0.9f);

        // Once the scene is fully loaded, allow it to activate and hide the progress bar
        scene.allowSceneActivation = true;
        progressBar.gameObject.SetActive(false);

        // Animate the transition out after the scene has loaded
        yield return transition.AnimateTransitionOut();
    }

}
