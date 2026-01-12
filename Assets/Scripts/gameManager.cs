using UnityEditor.Build;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuHome;
    
    public bool isPaused;
    public GameObject player;

    //ToDo: link player controller script - Lorenzo
    //public playerController playerScript; 




    float timeScaleOrig;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");

        /* ToDo: link player controller script - Lorenzo
        playerScript = player.GetComponent<playerController>();*/

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        { 
            if(menuActive == null)
            {
                statePaused();
                menuActive = menuPause;
                menuActive.SetActive(true);

            }
            else if(menuActive == menuPause)
            {
                stateUnpaused();
            }

        }




    }


    public void statePaused()
    {      
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    public void stateUnpaused()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }



}
