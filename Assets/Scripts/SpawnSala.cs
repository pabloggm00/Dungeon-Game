using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor.VersionControl;
using UnityEngine;


[Serializable]
public class ObjectsToSpawn
{
    public GameObject asset;
    public int minSpawn;
    public int maxSpawn;
    public bool canRotate;
}

public class SpawnSala : MonoBehaviour
{
    [Header("Base")]
    MeshRenderer meshRenderer;
    public GameObject wallSpawn;
    public GameObject wallSpawnParent;
    public GameObject groundSpawn;
    public GameObject ceilingSpawn;
    public GameObject flatParent;
    public float ceilingOffset;
    public int minX;
    public int maxX;
    public int minZ;
    public int maxZ;
   

    [Header("Objects")]
    public GameObject cellsParent;
    public GameObject cell;
    public GameObject killerCells;
    private GameObject killerCellsGenerated;
    List<Transform> cells = new List<Transform>();
    public GameObject objectsSpawnParent;
    public List<ObjectsToSpawn> objectsToSpawn = new List<ObjectsToSpawn>();
    public float cellOffset; // Ajusta esto según la separación deseada entre las celdas
    public float cellSize = 1;   // Ajusta esto según el tamaño de tus celdas

    [Header("Player")]
    public GameObject player;


    Vector3 giroMuroIzqZ;
    Vector3 giroMuroDerZ;
    Vector3 giroMuroSupX;
    Vector3 playerPosition;

    float rndMuroX;
    float rndMuroZ;
    float widthObject;
    float roomX;
    float roomZ;
    private SpawnEnemies spawnEnemies;
    public int numMaxObjectInRoom;

    public NavMeshSurface surface;

    // Start is called before the first frame update
    void Start()
    {

        spawnEnemies = GetComponent<SpawnEnemies>();

        CreateRoom();
   
        GenerateCells();
        PlaceRandomObjectsInCells();

        
        surface.BuildNavMesh();

 
        //player.transform.position = playerPosition;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeleteRoom()
    {
        for (int i = 0; i < cellsParent.transform.childCount; i++)
        {
            Destroy(cellsParent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < objectsSpawnParent.transform.childCount; i++)
        {
            Destroy(objectsSpawnParent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < wallSpawnParent.transform.childCount; i++)
        {
            Destroy(wallSpawnParent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < flatParent.transform.childCount; i++)
        {
            Destroy(flatParent.transform.GetChild(i).gameObject);
        }
    }

    public void SpawnRoom()
    {
        DeleteRoom();

        CreateRoom();
        GenerateCells();
        PlaceRandomObjectsInCells();


        //player.GetComponent<PlayerMove>().MovePosition(playerPosition);
        //player.transform.position = playerPosition;
    }


    void GenerateCells()
    {
        // Bucle para generar celdas en el eje X
        for (float x = this.transform.position.x; x < this.transform.position.x + roomX + widthObject / 2; x += cellSize + cellOffset)
        {
            // Bucle para generar celdas en el eje Z
            for (float z = this.transform.position.z + widthObject / 2; z < this.transform.position.z + roomZ - widthObject / 2; z += cellSize + cellOffset)
            {
                Vector3 cellPosition = new Vector3(x, this.transform.position.y, z);
                GameObject cellCreated = Instantiate(cell, cellPosition, Quaternion.identity, cellsParent.transform);
                //cellCreated.GetComponent<MeshRenderer>().enabled = false;
                if (!cellCreated.GetComponentInChildren<DetectSpawn>().IsInAreaEnemy())
                {
                    cells.Add(cellCreated.transform);
                    Debug.Log("Se ha creado");
                }
                else
                {
                    Destroy(cellCreated.gameObject);
                    Debug.Log("No se ha creado");
                }

                //cells.Add(cellCreated.transform);
            }
        }

        //Para evitar que spawnee un objeto debajo del personaje
       /* cells.RemoveAt(cells.Count / 2);
        cells.RemoveAt(cells.Count / 2 + 1);
        cells.RemoveAt(cells.Count / 2 - 1);*/
    }

    void PlaceRandomObjectsInCells()
    {
     

        //Saber cuantos objetos de cada uno vamos a tener
        foreach (ObjectsToSpawn objectSpawn in objectsToSpawn)
        {
            numMaxObjectInRoom = UnityEngine.Random.Range(objectSpawn.minSpawn, objectSpawn.maxSpawn);

            /*Debug.Log("Estoy metiendo objetos");
            Debug.Log("De " + objectSpawn.asset.name + " hay " + numMaxObjectInRoom);*/

            for (int i = 0; i < numMaxObjectInRoom; i++)
            {

                int rndCell = UnityEngine.Random.Range(0, cells.Count); // conseguimos una celda random
                Transform cellSelected = cells[rndCell]; //recogemos la posicion de esa celda

                // Calcula una posición aleatoria dentro de la celda
                float randomOffsetX = UnityEngine.Random.Range(-0.5f * (cellSelected.localScale.x - cellSize), 0.5f * (cellSelected.localScale.x - cellSize));
                float randomOffsetZ = UnityEngine.Random.Range(-0.5f * (cellSelected.localScale.z - cellSize), 0.5f * (cellSelected.localScale.z - cellSize));

                // Coloca el objeto aleatorio en la celda
                Vector3 objectPosition = cellSelected.position;//+ new Vector3(randomOffsetX, 0.0f, randomOffsetZ);

                if (objectSpawn.canRotate)
                {
                    float rndRotation = UnityEngine.Random.Range(0, 360f);
                    objectSpawn.asset.transform.Rotate(new Vector3(objectSpawn.asset.transform.rotation.x, rndRotation, objectSpawn.asset.transform.rotation.z));
                }


                Instantiate(objectSpawn.asset, objectPosition, Quaternion.identity, objectsSpawnParent.transform);
                // Verifica si hay colisiones con otros objetos
                /* Collider[] colliders = Physics.OverlapSphere(objectPosition, 0.7f); // Ajusta el radio según el tamaño de tus objetos

                 if (colliders.Length == 1)
                 {
                     // No hay colisiones, instancia el objeto
                     Instantiate(objectSpawn.asset, objectPosition, Quaternion.identity, objectsSpawnParent.transform);

                 }*/

                cells.Remove(cellSelected);
            }

            

        }

        for (int i = 0; i < spawnEnemies.numSpawnEnemies; i++)
        {
            int rndCell = UnityEngine.Random.Range(0, cells.Count); // conseguimos una celda random
            Transform cellSelected = cells[rndCell]; //recogemos la posicion de esa celda

            // Calcula una posición aleatoria dentro de la celda
            float randomOffsetX = UnityEngine.Random.Range(-0.5f * (cellSelected.localScale.x - cellSize), 0.5f * (cellSelected.localScale.x - cellSize));
            float randomOffsetZ = UnityEngine.Random.Range(-0.5f * (cellSelected.localScale.z - cellSize), 0.5f * (cellSelected.localScale.z - cellSize));

            // Coloca el objeto aleatorio en la celda
            Vector3 enemyPosition = cellSelected.position;//+ new Vector3(randomOffsetX, 0.0f, randomOffsetZ);

            spawnEnemies.InstantiateEnemies(enemyPosition, player.transform);
        }

        for (int i = 0;i < cellsParent.transform.childCount; i++)
        {
            //Destroy(cellsParent.transform.GetChild(i).gameObject);
        }

        cells.Clear();
       

        // Itera sobre las celdas
        
    }

    void CreateRoom()
    {
        meshRenderer = wallSpawn.GetComponent<MeshRenderer>();

        giroMuroIzqZ = new Vector3(0, 90, 0);
        giroMuroDerZ = new Vector3(0, -90, 0);
        giroMuroSupX = new Vector3(0, 180, 0);

        rndMuroX = UnityEngine.Random.Range(minX, maxX);
        rndMuroZ = UnityEngine.Random.Range(minZ, maxZ);



        widthObject = meshRenderer.bounds.extents.x * 2;
        float heightObject = meshRenderer.bounds.extents.y * 2;

        float centerX = (rndMuroX * widthObject - widthObject) / 2;
        float centerZ = (rndMuroZ * widthObject) / 2;

        roomX = rndMuroX * widthObject - widthObject;
        roomZ = rndMuroZ * widthObject;

        Vector3 groundPosition = new Vector3(this.transform.position.x + centerX, 0, this.transform.position.z + centerZ);

       
        playerPosition = new Vector3(this.transform.position.x + centerX, groundPosition.y + 2.5f, this.transform.position.z + centerZ);

        killerCellsGenerated = Instantiate(killerCells, groundPosition, Quaternion.identity, objectsSpawnParent.transform );

        player.GetComponent<PlayerMove>().MovePosition(playerPosition);

        GameObject ground = Instantiate(groundSpawn, groundPosition, Quaternion.identity, flatParent.transform);
        GameObject ceiling = Instantiate(ceilingSpawn, groundPosition + Vector3.up * (heightObject * 2 - ceilingOffset), Quaternion.Euler(180f, 0, 0), flatParent.transform);
        ground.isStatic = true;

        //para calcular la escala, como sabemos que en el eje Z es exacto, aplicamos la regla de tres para saber cual es la escala en x y le sumaremos uno que es lo que faltan en cada lado (0.5).
        ground.transform.localScale = new Vector3(((centerX * centerZ / 2) / centerZ) + 1, 1, centerZ / 2f);
        ceiling.transform.localScale = new Vector3(((centerX * centerZ / 2) / centerZ) + 1, 1, centerZ / 2f);

        /*Debug.Log(rndMuroX);
        Debug.Log(rndMuroZ);
        Debug.Log("El área de la sala es de: " + rndMuroX * widthObject + "x" + rndMuroZ * widthObject);*/ 

        //Creamos los muros en eje x inferior
        for (int muroInfX = 0; muroInfX < rndMuroX; muroInfX++)
        {
            Instantiate(wallSpawn, new Vector3(this.transform.position.x + muroInfX * widthObject, this.transform.position.y, this.transform.position.z), Quaternion.identity, wallSpawnParent.transform);

            if (muroInfX == rndMuroX - 1)
            {
                //Creamos los muros en eje z izquierda
                for (int muroIzqZ = 0; muroIzqZ < rndMuroZ; muroIzqZ++)
                {

                    Instantiate(wallSpawn, new Vector3(this.transform.position.x - widthObject / 2, this.transform.position.y, this.transform.position.z + (muroIzqZ * widthObject) + (widthObject / 2)), Quaternion.Euler(giroMuroIzqZ), wallSpawnParent.transform);

                    if (muroIzqZ == rndMuroZ - 1)
                    {
                        //Creamos los muros en eje z derecha
                        for (int muroDerZ = 0; muroDerZ < rndMuroZ; muroDerZ++)
                        {
                            Instantiate(wallSpawn, new Vector3(this.transform.position.x + rndMuroX * widthObject - widthObject / 2, this.transform.position.y, this.transform.position.z + muroDerZ * widthObject + (widthObject / 2)), Quaternion.Euler(giroMuroDerZ), wallSpawnParent.transform);

                            if (muroDerZ == rndMuroZ - 1)
                            {
                                //Creamos los muros en eje x superior
                                for (int muroSupX = 0; muroSupX < rndMuroX; muroSupX++)
                                {
                                    Instantiate(wallSpawn, new Vector3(this.transform.position.x + muroSupX * widthObject, this.transform.position.y, this.transform.position.z + rndMuroZ * widthObject), Quaternion.Euler(giroMuroSupX), wallSpawnParent.transform);
                                }
                            }
                        }
                    }
                }
            }
        }
    }





}
