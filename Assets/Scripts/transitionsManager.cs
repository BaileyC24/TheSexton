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
    public GameObject iconImage;

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

        // Ensure the progress bar is hidden at the start
        progressBar.gameObject.SetActive(false);

        // Ensure the icon image is hidden at the start
        iconImage.SetActive(false);

    }

    private void Update()
    {
        progressBar.transform.Rotate(0f, 0f, -100f * Time.deltaTime); // Rotate the progress bar for a spinning effect

        //Zoom the icon image in and out while the scene is loading
        if (iconImage.activeSelf)
        {
            float scale = 8f + Mathf.PingPong(Time.time, 2f); // scale oscillates between 8 and 10
            iconImage.transform.localScale = new Vector3(scale, scale, scale);
        }



    }

    public void LoadScene(int sceneIndex, string transitionName)
    {
        // Start the scene transition process for the specified scene
        StartCoroutine(LoadSceneAsync(sceneIndex, transitionName));
    }

    private IEnumerator LoadSceneAsync(int sceneIndex, string transitionName)
    { 
        sceneTransitions transition = transitions.First(t => t.name == transitionName);
        
        
         // Animate the transition in
        yield return transition.AnimateTransitionIn();

        // Show the progress bar while the scene is loading
        progressBar.gameObject.SetActive(true);
        

        // Show the icon image while the scene is loading
        iconImage.SetActive(true);
        yield return null;

        // Start loading the scene asynchronously but do not allow it to activate until the transition is complete
        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneIndex);
        scene.allowSceneActivation = false;


        // Update the progress bar until the scene is fully loaded (progress reaches 0.9)

        while (scene.progress < 0.9f)
        {
            progressBar.value = scene.progress / 0.9f; // Normalize progress to 0-1 range
            yield return null; // Wait for the next frame before updating again
        }

        yield return new WaitForSeconds(5f); // Optional: Add a small delay to ensure the progress bar is visible at 100%

        // Once the scene is fully loaded, allow it to activate and hide the progress bar
        scene.allowSceneActivation = true;
        progressBar.gameObject.SetActive(false);
        iconImage.SetActive(false);

        // Animate the transition out after the scene has loaded
        yield return transition.AnimateTransitionOut();
    }

}
