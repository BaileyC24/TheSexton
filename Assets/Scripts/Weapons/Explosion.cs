using UnityEngine;

public class Explosion : MonoBehaviour
{

    [SerializeField] float destroyTime; //in how long should it destroy itself, making it settable without hard coding

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(this.gameObject, destroyTime); //the object will be destroyed after X amount of time

    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
