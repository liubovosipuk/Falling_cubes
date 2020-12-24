using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections; 
using System.Collections.Generic; 
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{

    private CubePos nowCube = new CubePos(0, 1, 0);
    public float cubeChangePlaceSpeed = 0.5f; //the rate of change of pos for Cube To Place 
    public Transform cubeToPlace; 
    private float camMoveToYPosition, camMoveSpeed = 2f; 

    public Text scoreTxt;

    //cubes in the shop 
    public GameObject[] cubesToCreate; 

    public GameObject allCubes, vfx; 
    public GameObject[] canvasStartPage;
    private Rigidbody allCubesRb;

    public Color[] bgColors;
    private Color toCameraColor;

    private bool IsLose, firstCube; 

    
    
    //places where the cube can't be placed 
    private List<Vector3> allCubesPositions = new List<Vector3> 
    {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, -1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1),
    };

    private int prevCountMaxHorizontal;
    private Transform mainCam;
    private Coroutine showCubePlace; 

    private List<GameObject> possibleCubesToCreate = new List<GameObject>();
    



    private void Start()
    {

        if (PlayerPrefs.GetInt("score") < 10)
            possibleCubesToCreate.Add(cubesToCreate[0]);
        else if (PlayerPrefs.GetInt("score") < 18)
            AddPossibleCubes(2); 
        else if (PlayerPrefs.GetInt("score") < 25)
            AddPossibleCubes(3);
        else if (PlayerPrefs.GetInt("score") < 31)
            AddPossibleCubes(4);
        else if (PlayerPrefs.GetInt("score") < 37)
            AddPossibleCubes(5);
        else if (PlayerPrefs.GetInt("score") < 48)
            AddPossibleCubes(6);
        else if (PlayerPrefs.GetInt("score") < 60)
            AddPossibleCubes(7);
        else if (PlayerPrefs.GetInt("score") < 75)
            AddPossibleCubes(8);
        else if (PlayerPrefs.GetInt("score") < 90)
            AddPossibleCubes(9);
        else 
            AddPossibleCubes(10);


        scoreTxt.text = "<size=50><color=#C36E6C>score</color>:</size>" + PlayerPrefs.GetInt("score")
                         + "\n<size=40>cube:</size> 0";


        // default color
        toCameraColor = Camera.main.backgroundColor;

        //mooved here to smooth camera moovement
        mainCam = Camera.main.transform;
        camMoveToYPosition = 5.1f + nowCube.y - 1; 


        allCubesRb = allCubes.GetComponent<Rigidbody>(); 
        showCubePlace = StartCoroutine(ShowCubePlace());
    }

    private void Update() 
    {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null         
                                                                  && allCubes != null 
                                                                  && !EventSystem.current.IsPointerOverGameObject()) 
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return; // если нет прикосновения к экрану то выходим из функции
#endif

           

            if (!firstCube)
            {
                firstCube = true;
                foreach (GameObject obj in canvasStartPage)
                    
                    obj.SetActive(false); 
            }




           
            GameObject createCube = null;
            if (possibleCubesToCreate.Count == 1)
                createCube = possibleCubesToCreate[0];
            else
                //from this array every time a random cube will be selected which will open in the store
                createCube = possibleCubesToCreate[UnityEngine.Random.Range(0, possibleCubesToCreate.Count)];


            GameObject newCube = Instantiate // 
                                 (createCube, 
                                 cubeToPlace.position,
                                 Quaternion.identity) as GameObject; 
            newCube.transform.SetParent(allCubes.transform); 
            nowCube.setVector(cubeToPlace.position);//coordinates of the new extreme cube from which all calculations of coordinates will go
            allCubesPositions.Add(nowCube.getVector());//as the occupied position add the one in which the new cube was placed


            if (PlayerPrefs.GetString("music") != "No")
                GetComponent<AudioSource>().Play(); 


            //new obj for effects  when standing a cube
            GameObject newVfx = Instantiate(vfx, cubeToPlace.position, Quaternion.identity) as GameObject;
            Destroy(newVfx, 1.5f); 


            allCubesRb.isKinematic = true;
            allCubesRb.isKinematic = false;
                        
            SpawnPositions();
            MoveCameraChangeBg();
        }

        if (!IsLose && allCubesRb.velocity.magnitude > 0.1f) 
        {
            Destroy(cubeToPlace.gameObject);
            IsLose = true;
            StopCoroutine(showCubePlace);
        }

        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition,
            new Vector3(mainCam.localPosition.x, camMoveToYPosition, mainCam.localPosition.z),
            camMoveSpeed * Time.deltaTime);


        //smoothnes of changing color
        if (Camera.main.backgroundColor != toCameraColor)
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, toCameraColor, Time.deltaTime / 1.5f);

    }

    IEnumerator ShowCubePlace()
    {
        
        while (true)
        {
            SpawnPositions();
            yield return new WaitForSeconds(cubeChangePlaceSpeed);
        }
    }



    private void SpawnPositions()
    {
        //all available positions for placing the CubeToPlace
        List<Vector3> position = new List<Vector3>();
       
        if (IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z ))
             && nowCube.x + 1 != cubeToPlace.position.x)
                position.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z))
            && nowCube.x - 1 != cubeToPlace.position.x)

            position.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z))
            && nowCube.y + 1 != cubeToPlace.position.y)

            position.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z))
            && nowCube.y - 1 != cubeToPlace.position.y)
            position.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));

        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1))
            && nowCube.z + 1 != cubeToPlace.position.z)
            position.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z  + 1));

        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1))
            && nowCube.z - 1 != cubeToPlace.position.z)
            position.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));


        if (position.Count > 1)
            cubeToPlace.position = position[UnityEngine.Random.Range(0, position.Count)]; //where we place the cube
        else if (position.Count == 0)
            IsLose = true;
        else
            cubeToPlace.position = position[0];
    }

    
    private bool IsPositionEmpty(Vector3 targetPos)
    {
        if (targetPos.y == 0)
            return false; 

        
        foreach(Vector3 pos in allCubesPositions)
        {
            if (pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z)
                return false;
        }
        return true;
    }




    //camera mooving, changing BG
    private void MoveCameraChangeBg()
    {
        int maxX = 0, maxY = 0, maxZ = 0, maxHor; 

        foreach (Vector3 pos in allCubesPositions)
        {
            if (Mathf.Abs(Convert.ToInt32(pos.x)) > maxX)
                maxX = Convert.ToInt32(pos.x);
        
            if (Mathf.Abs(Convert.ToInt32(pos.y)) > maxY)
                maxY = Convert.ToInt32(pos.y);

            if (Mathf.Abs(Convert.ToInt32(pos.z)) > maxZ)
                 maxZ = Convert.ToInt32(pos.z);
        }


        // scorer of best result and builded cubes
        maxY--; 

        if (PlayerPrefs.GetInt("score") < maxY) 
            PlayerPrefs.SetInt("score", maxY);

        scoreTxt.text = "<size=50><color=#C36E6C>score</color>:</size>" + PlayerPrefs.GetInt("score") 
                         + "\n<size=40>cube:</size>" + maxY;


        /* moving camera at axix Y
        Transform mainCam = Camera.main.transform;
        camMoveToYPosition = 5.1f + nowCube.y - 1f; 
        mainCam.localPosition = new Vector3(mainCam.localPosition.x, camMoveToYPosition, mainCam.localPosition.z);
        */


        //work everu time not just at the start
        camMoveToYPosition = 5.1f + nowCube.y - 1f;  



        //zooming out the camera for obj visibility
        maxHor = maxX > maxZ ? maxX : maxZ;
        if(maxHor % 3 == 0 && prevCountMaxHorizontal != maxHor)
        {
            mainCam.localPosition -= new Vector3(0, 0, 2.5f);
            prevCountMaxHorizontal = maxHor;
        }

        //changing BG depending of the number of cubes 
        if (maxY >= 7)
            toCameraColor = bgColors[2];
        else if (maxY >= 5)
            toCameraColor = bgColors[1];
        else if (maxY >= 2)
            toCameraColor = bgColors[0];

    }




    // open cube that are available by points
    private void AddPossibleCubes(int till)
    {
        for (int i = 0; i < till; i++)
            possibleCubesToCreate.Add(cubesToCreate[i]);
    }



    // struct responsible for keeping the coordinates of any obj
    struct CubePos  
    {
        public int x, y, z;


        public CubePos(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 getVector()
        {
            return new Vector3(x, y, z);
        }

        public void setVector(Vector3 pos)
        {
            x = Convert.ToInt32(pos.x);
            y = Convert.ToInt32(pos.y);
            z = Convert.ToInt32(pos.z); 
        }
    }
}
