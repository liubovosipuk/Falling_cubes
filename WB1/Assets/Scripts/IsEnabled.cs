using UnityEngine;

public class IsEnabled : MonoBehaviour
{
    public int needToUnlock; 
    public Material greyMaterial; //not available cube

    private void Start()
    {
        if (PlayerPrefs.GetInt("score") < needToUnlock)
            GetComponent<MeshRenderer>().material = greyMaterial;

    }

}
