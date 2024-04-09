using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPosObject : MonoBehaviour
{
    public Transform objectsParent;
    public List<Transform> posObject;
    public List<GameObject> objects;
    public List<GameObject> chairs;
    

    // Start is called before the first frame update
    void Start()
    {
      
        for (int i = 0; i < chairs.Count; i++)
        {
            int rndActive = Random.Range(0,2);
            
            if (rndActive == 0) {

                chairs[i].SetActive(false);
            }
            else
            {
                chairs[i].SetActive(true);
            }
        }

        int rndNumObjects = Random.Range(2,posObject.Count);

        for (int i = 0; i < rndNumObjects; i++)
        {
            int rndPos = Random.Range(0, posObject.Count);
            int rndObject = Random.Range(0, objects.Count);

            Instantiate(objects[rndObject], posObject[rndPos].position, Quaternion.identity, objectsParent);

            posObject.Remove(posObject[rndPos]);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
