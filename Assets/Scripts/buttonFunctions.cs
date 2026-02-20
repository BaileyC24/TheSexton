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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        MusicManager.instance.PlayMusic("InGame");
        gameManager.instance.stateUnpaused();
    }
    
    public void play(CharacterData characterData)
    {
        SoundManager.PlaySound(SoundType.Menu);
        SceneManager.LoadScene(1);
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
        if(gameManager.instance.points > 0 && gameManager.instance.playerScript.health < 200)
        {
            gameManager.instance.playerScript.HPOrig += 5;
            gameManager.instance.playerScript.health = gameManager.instance.playerScript.HPOrig;
            gameManager.instance.points--;
        }
    }

    public void strPlus()
    {
        // TODO: REDO STRENGTH UPGRADE TO WORK PROPERLY WITH THE NEW SYSTEM
        if (gameManager.instance.points > 0 && gameManager.instance.playerStats.currentWeapon.damage < 15)
        {
            gameManager.instance.playerStats.currentWeapon.damage += 1;
            gameManager.instance.points--;
        }
    }

    public void attSpdPlus()
    {
        // TODO: REDO ATTACK SPEED UPGRADE TO WORK PROPERLY WITH THE NEW SYSTEM
        /*if (gameManager.instance.points > 0 && gameManager.instance.playerStats.attackSpeed > 0.1f)
        {
            gameManager.instance.playerStats.attackSpeed -= 0.1f;
            gameManager.instance.points--;
        }*/
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

    public void credits(CharacterData characterData)
    {
        SoundManager.PlaySound(SoundType.Menu); //SoundType.Menu _find where is and set to Credits
        SceneManager.LoadScene(2); //_find where is and set to 2 for credits
        MusicManager.instance.PlayMusic("InGame"); //_find and set to "Credits" make new music
        //It would be fun to list the current character name and then in the credits put YOU
        currentPlayerData.currentCharacter = characterData;
    }

    }
