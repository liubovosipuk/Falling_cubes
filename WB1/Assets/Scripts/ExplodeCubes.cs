using UnityEngine;

public class ExplodeCubes : MonoBehaviour
{

    public GameObject restartButton, explosion;
    private bool _collisionSet; 


    private void OnCollisionEnter(Collision collision)
    {
        //checking which obj touched
        if (collision.gameObject.tag == "Cube" && !_collisionSet)  
        {
            for (int i = collision.transform.childCount -1; i>=0; i--)
            {
                // new object (variable) with data type Transform that set up  object's index "i" 
                Transform child = collision.transform.GetChild(i); 
               
                child.gameObject.AddComponent<Rigidbody>(); 
                child.gameObject.GetComponent<Rigidbody>().AddExplosionForce(70f, Vector3.up, 5f);
                child.SetParent(null);

            }
            restartButton.SetActive(true); // after falling cubes
            
            
            // after appearence of restart bttn 
            Camera.main.transform.position -= new Vector3(0, 2f, 2f);
            Camera.main.gameObject.AddComponent<CameraShake>();


           //particle effect
           //pos where will create an exposion
           GameObject newVfx = Instantiate(explosion, new Vector3(collision.contacts[0].point.x, 
                                               collision.contacts[0].point.y,
                                               collision.contacts[0].point.z), Quaternion.identity) as GameObject;
          
 
            Destroy(newVfx, 2.5f); 


            if (PlayerPrefs.GetString("music") != "No")
                GetComponent<AudioSource>().Play(); 


            Destroy(collision.gameObject);
            _collisionSet = true; 
        }
    }
    
}
