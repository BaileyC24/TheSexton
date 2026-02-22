using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public RectTransform uiElementToManage;
    public GameObject storeContent;
    public GameObject upgradeContent;
    public PlayerActiveData currentPlayerData;
    
    public void resume()
    {
        SoundManager.PlaySound(SoundType.Menu);
        gameManager.instance.stateUnpaused();
    }

    public void restart()
    {
        SoundManager.PlaySound(SoundType.Menu);
        gameManager.instance.currentPlayerData.Clear();
        transitionsManager.instance.LoadScene(1 , "CrossFade");
        MusicManager.instance.PlayMusic("InGame");
        gameManager.instance.stateUnpaused();
    }
    
    public void play(CharacterData characterData)
    {
        SoundManager.PlaySound(SoundType.Menu);
        transitionsManager.instance.LoadScene(1 , "CrossFade");
        MusicManager.instance.PlayMusic("InGame");
        currentPlayerData.currentCharacter = characterData;
    }

    public void quit()
    {
        SoundManager.PlaySound(SoundType.Menu);
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            
            Application.Quit();
            return;
        }
        gameManager.instance.currentPlayerData.Clear();
        
        SceneManager.LoadScene(0);
        MusicManager.instance.PlayMusic("MainMenu");
        gameManager.instance.stateUnpaused();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void hpPlus()
    {
        if (gameManager.instance.points < 2)
        {
            gameManager.instance.SendAlert("Not enough points");
            return;
        }
        
        if (gameManager.instance.currentPlayerData.healthUpgrade >= 60)
        {
            gameManager.instance.SendAlert("Upgrade already at max level");
            return;
        }

        gameManager.instance.playerScript.health += gameManager.instance.currentPlayerData.healthUpgrade;
        gameManager.instance.points -= 2;
    }

    public void strPlus()
    {
        if (gameManager.instance.currentPlayerData.damageUpgrade >= 8)
        {
            gameManager.instance.SendAlert("Upgrade already at max level");
            return;
        }
        
        if (gameManager.instance.points < 3)
        {
            gameManager.instance.SendAlert("Not enough points");
            return;
        }

        gameManager.instance.currentPlayerData.damageUpgrade += 1;
        gameManager.instance.points -= 3;
    }

    public void attSpdPlus()
    {
                
        if (gameManager.instance.currentPlayerData.atkSpeedUpgrade >= 0.4f)
        {
            gameManager.instance.SendAlert("Upgrade already at max level");
            return;
        }
        
        if (gameManager.instance.points < 2)
        {
            gameManager.instance.SendAlert("Not enough points");
            return;
        }

        gameManager.instance.currentPlayerData.atkSpeedUpgrade += 0.1f;
        gameManager.instance.points -= 2;
    }

    public void ChanceUpgrade()
    {
        if (gameManager.instance.playerStats.currentWeapon.effectChanceUpgrade >= 0.5f)
        {
            gameManager.instance.SendAlert("Upgrade already at max level");
            return;
        }
        
        if (gameManager.instance.playerStats.currentWeapon.specialEffect == WeaponData.SpecialEffect.None)
        {
            gameManager.instance.SendAlert("Weapon has no special effect");
            return;
        }
        
        if (gameManager.instance.points < 4)
        {
            gameManager.instance.SendAlert("Not enough points");
            return;
        }

        gameManager.instance.playerStats.currentWeapon.effectChanceUpgrade += 0.15f;
        gameManager.instance.points -= 2;
    }

    public void BringToFront()
    {
        if (uiElementToManage != null)
        {
            uiElementToManage.SetAsLastSibling();
            storeContent.SetActive(true);
            upgradeContent.SetActive(false);
        }
    }
    
    public void SendToBack()
    {
        if (uiElementToManage != null)
        {
            uiElementToManage.SetAsFirstSibling();
            storeContent.SetActive(false);
            upgradeContent.SetActive(true);
        }
    }
}
