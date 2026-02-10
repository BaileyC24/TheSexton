using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] private GameObject menuActive;
    [SerializeField] private GameObject menuPause;
    [SerializeField] private GameObject menuWin;
    [SerializeField] private GameObject menuLose;
    [SerializeField] private GameObject menuStore;
    [SerializeField] private GameObject menuAlert;
    [SerializeField] private GameObject menuInventory;
    
    [SerializeField] private CharacterData characterData;

    public GameObject playerSpawnPos;
    public Image playerHPBar;
    public bool isPaused;
    public GameObject player;
    public GameObject damageFlash;
    public PlayerAttack playerStats;
    public int maxLevel;
    [SerializeField] double nextLevel;
    public int exp;
    public int points;
    public int level;
    
    public enum MenuType
    {
        Lose,
        Win,
        Store,
        Inventory
    }
    
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

    private void Start()
    {
        InventoryManager.instance.AddStartingItems(characterData.startingItems, characterData.startingWeapons);
    }

    // Update is called once per frame
    void Update()
    {
        levelText.text = level.ToString("F0") + "/" + maxLevel.ToString("F0");
        pointsText.text = points.ToString("F0");
        strText.text = playerStats.currentWeapon.damage.ToString("F0");
        attackSpdText.text = "N/A TODO";
        HealthText.text = playerScript.health.ToString("F0") + "/" + playerScript.HPOrig.ToString("F0");
        
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive != null)
            {
                stateUnpaused();
            }
            else
            {
                statePaused();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
        }

        if (exp >= nextLevel && level < maxLevel)
        {
            level++;
            points += 3;
            nextLevel = (nextLevel * 1.3) + 2;
            exp = 0;
            SendAlert("Level Up! You are now level " + level.ToString("F0") + "!", 1.5f);
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

    public void OpenMenu(MenuType type)
    {
        statePaused();
        if (menuActive)
            menuActive.SetActive(false);

        switch (type)
        {
            case MenuType.Lose:
                menuActive = menuLose;
                break;

            case MenuType.Win:
                menuActive = menuWin;
                break;

            case MenuType.Store:
                menuActive = menuStore;
                break;
            case MenuType.Inventory:
                menuActive = menuInventory;
                break;
        }

        menuActive.SetActive(true);
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalText.text = gameGoalCount.ToString("F0");
    }


    public void SendAlert(string message, float delay = 0.5f)
    {
        StartCoroutine(Alert(message, delay));
    }
    
    IEnumerator Alert(string message, float delay)
    {
        menuAlert.SetActive(true);
        menuAlert.GetComponentInChildren<TMP_Text>().text = message;
        yield return new WaitForSeconds(delay);
        menuAlert.SetActive(false);
    }
}
