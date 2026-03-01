using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;
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
    [SerializeField] private GameObject menuStageComplete;
    [SerializeField] private Image currentWeaponIcon;
    [SerializeField] private Image previousWeaponIcon;
    
    [Header("Upgrade")]
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI weaponEffect;
    [SerializeField] private TextMeshProUGUI weaponChance;
    
    [SerializeField] public PlayerActiveData currentPlayerData;

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

    public bool isTransitioning;
    public bool isDead;
    public bool freezeGameplay;

    public enum MenuType
    {
        Lose,
        Win,
        Store,
        Inventory,
        StageComplete
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

    // Update is called once per frame
    void Update()
    {
        UpdateText();

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
            int coinsRewarded = 30 + (level * 5);
            InventoryManager.instance.coinsOnHand += coinsRewarded;
            SoundManager.PlaySound(SoundType.Buy);
            SendAlert("Level Up! You are now level " + level.ToString("F0") + "!", 1.5f);
        }
    }

    private void UpdateText()
    {
        levelText.text = level.ToString("F0") + "/" + maxLevel.ToString("F0");
        pointsText.text = points.ToString("F0");
        strText.text = (playerStats.currentWeapon.damage + currentPlayerData.damageUpgrade).ToString("F0");
        attackSpdText.text = (playerStats.currentWeapon.totalTime - currentPlayerData.atkSpeedUpgrade).ToString("F2");
        HealthText.text = (playerScript.health) + "/" + (playerScript.HPOrig + currentPlayerData.healthUpgrade);
        weaponName.text = playerStats.currentWeapon.weaponName;
        weaponChance.text = (playerStats.currentWeapon.specialChance + playerStats.currentWeapon.effectChanceUpgrade).ToString("F2");
        weaponEffect.color = WeaponData.SpecialEffectColor[playerStats.currentWeapon.specialEffect];
        weaponEffect.text = playerStats.currentWeapon.specialEffect.ToString();
    }

    public void statePaused()
    {

        if (gameManager.instance.isTransitioning)
            return; // block pausing during transitions

        if(gameManager.instance.isDead)
            return; // block pausing when dead


        SoundManager.PlaySound(SoundType.Pause);
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerScript.GetInput().Disable();
    }


    public void stateUnpaused()
    {
        SoundManager.PlaySound(SoundType.Unpause);
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive?.SetActive(false);
        menuActive = null;
        playerScript.GetInput().Enable();
    }

    public void OpenMenu(MenuType type)
    {
        statePaused();
        if (menuActive)
            menuActive.SetActive(false);

        switch (type)
        {
            case MenuType.Lose:
                gameManager.instance.freezeGameplay = true;
                gameManager.instance.isDead = true;
                menuActive = menuLose;
                break;

            case MenuType.StageComplete:
                gameManager.instance.freezeGameplay = true;
                menuActive = menuStageComplete;
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

    private void SwitchLevels(int levelIndex)
    {
        currentPlayerData.SaveData(InventoryManager.instance);
        SceneManager.LoadScene(levelIndex);
    }

    public void UpdateWeaponIcons(WeaponData current, WeaponData previous)
    {
        if (currentWeaponIcon != null)
        {
            currentWeaponIcon.sprite = current != null ? current.weaponIcon : null;
            currentWeaponIcon.enabled = currentWeaponIcon.sprite != null;
        }

        if (previousWeaponIcon != null)
        {
            previousWeaponIcon.sprite = previous != null ? previous.weaponIcon : null;
            previousWeaponIcon.enabled = previousWeaponIcon.sprite != null;
        }
    }
}
