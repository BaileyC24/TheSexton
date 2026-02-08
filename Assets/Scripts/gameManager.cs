using UnityEngine;
using TMPro;
using UnityEngine.UI;

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



    public GameObject playerSpawnPos;
    public Image playerHPBar;
    public bool isPaused;
    public GameObject player;
    public GameObject damageFlash;
    public PlayerAttack playerStats;
    [SerializeField] int maxLevel;
    [SerializeField] double nextLevel;
    public int exp;
    public int points;
    int level;



    public PlayerStateMachine playerScript;



    int gameGoalCount;
    float timeScaleOrig;

    public TMP_Text gameGoalText;

    public TMP_Text pointsText;
    public TMP_Text levelText;
    public TMP_Text strText;
    public TMP_Text attackSpdText;
    public TMP_Text HealthText;


    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");

        level = 1;
        playerScript = player.GetComponent<PlayerStateMachine>();
        playerStats = player.GetComponent<PlayerAttack>();
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
    }

    // Update is called once per frame
    void Update()
    {
        levelText.text = "Level: " + level.ToString("F0") + "/" + maxLevel.ToString("F0");
        pointsText.text = points.ToString("F0");
        strText.text = "STR: " + playerStats.currentWeapon.damage.ToString("F0");
        attackSpdText.text = "A.SPD:" + playerStats.currentWeapon.fireRate;
        HealthText.text = "HP: " + playerScript.health.ToString("F0") + "/" + playerScript.HPOrig.ToString("F0");
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

        if(exp >= nextLevel && level < maxLevel)
        {
            level++;
            points += 3;
            menuUpgrade();
            nextLevel = (nextLevel * 1.3) + 2;
            exp = 0;
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
        menuActive?.SetActive(false);
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
        gameGoalCount += amount;
        gameGoalText.text = gameGoalCount.ToString("F0");
    }



 }
