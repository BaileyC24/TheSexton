using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] popUps;
    private int popUpIndex;
    public GameObject spawner;


    void Start()
    {
        gameManager.instance.playerScript.GetInput().Player.Movement.Disable();
        gameManager.instance.playerScript.GetInput().Player.Jump.Disable();
        gameManager.instance.playerScript.GetInput().Player.Sprint.Disable();
        gameManager.instance.playerScript.GetInput().Player.Sprint.Disable();
        gameManager.instance.playerScript.GetInput().Player.Attack.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < popUps.Length; i++)
        {
            if (i == popUpIndex)
            {
                popUps[popUpIndex].SetActive(true);
            }
            else
            {
                popUps[popUpIndex].SetActive(false);
            }
        }
        
        if (popUpIndex == 0)
        {
            //trigger for camera
            
            
        }
        else if (popUpIndex == 1)
        {
            if (gameManager.instance.playerScript.GetInput().Player.Movement.ReadValue<Vector2>().magnitude > 0.001f)
            {
                gameManager.instance.playerScript.GetInput().Player.Movement.Enable();
                popUpIndex++;
            }
        }
        else if (popUpIndex == 2)
        {
            if (gameManager.instance.playerScript.GetInput().Player.Sprint.triggered)
            {
                gameManager.instance.playerScript.GetInput().Player.Sprint.Enable();
                popUpIndex++;
            }
        }
        else if (popUpIndex == 3)
        {
            if (gameManager.instance.playerScript.GetInput().Player.Jump.triggered)
            {
                gameManager.instance.playerScript.GetInput().Player.Jump.Enable();
                popUpIndex++;
            }
        }
        else if (popUpIndex == 4)
        {
            if (gameManager.instance.playerScript.GetInput().Player.Attack.triggered)
            {
                gameManager.instance.playerScript.GetInput().Player.Attack.Enable();
                popUpIndex++;
            }
        }
    }
}
