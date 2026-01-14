using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using TMPro;
using UnityEditor.Build.Reporting;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuHome;
    [SerializeField] GameObject menuUpgrades;
    [SerializeField] GameObject menuGuns;
    [SerializeField] GameObject menuObjectives;




    
    public bool isPaused;
    public GameObject player;

    
    public PlayerStateMachine playerScript;



    int gameGoalCount;
    float timeScaleOrig;

    public TMP_Text gameGoalText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");

        
        playerScript = player.GetComponent<PlayerStateMachine>();

        
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
            
        if (Input.GetButtonDown("U") && menuActive == false)
        {
                  
                  menuUpgrade();
        }
        else if(menuActive == menuUpgrades && Input.GetButtonDown("Cancel") || Input.GetButtonDown("U"))
        {
            stateUnpaused();
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

    public void youLose()
    {
        statePaused();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void youWin()
    {
        statePaused();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void menuUpgrade()
        {
        statePaused();
        menuActive = menuUpgrades;
        menuActive.SetActive(true);
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount; ;
        gameGoalText.text = "Enemies Remaining: " + gameGoalCount;

        if (gameGoalCount <= 0)
        {
            statePaused();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }







 }
