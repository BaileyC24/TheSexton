using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{

    public void resume()
    {
        gameManager.instance.stateUnpaused();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpaused();
    }
    
    public void play()
    {
        SceneManager.LoadScene(1);
    }

    public void quit()
    {
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
            gameManager.instance.playerScript.updatePlayerUI();
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
}
