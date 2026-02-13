using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public RectTransform uiElementToManage;
    public GameObject storeContent;
    public GameObject upgradeContent;
    
    public void resume()
    {
        SoundManager.PlaySound(SoundType.Menu);
        gameManager.instance.stateUnpaused();
    }

    public void restart()
    {
        SoundManager.PlaySound(SoundType.Menu);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpaused();
    }
    
    public void play()
    {
        SoundManager.PlaySound(SoundType.Menu);
        SceneManager.LoadScene(1);
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
        
        SceneManager.LoadScene(0);
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
}
