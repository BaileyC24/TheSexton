using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSpawnPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.F))
        { 
        LaunchProjectile();
        }
    }

    void LaunchProjectile()
    {
        //need to do positional code still
        //if (myCamera == null) 
       // return; 
       //

       // if(leftClickInput > 0) _this uses new Unity system
       // if(Input.GetMouseButtonDown(0))
      //  {
       Instantiate(projectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation); //our spawn point/player position changes origin point of projectile spawn
       // }

    }
}
