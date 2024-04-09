using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AssetsRandom
{
    public GameObject asset;
    public int prob;
    public bool canRotate;
}


public class RandomTypeObject : MonoBehaviour
{

    [SerializeField]
    private List<AssetsRandom> assets;

    // Start is called before the first frame update
    void Start()
    {
        ActivateRandomObject();

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            // Obtiene el hijo actual
            Transform child = transform.GetChild(i);

            // Verifica si el hijo está desactivado
            if (!child.gameObject.activeSelf)
            {
                // Elimina el hijo desactivado
                Destroy(child.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    // Método para activar un objeto aleatorio basado en probabilidades
    void ActivateRandomObject()
    {
        // Calcular la suma total de probabilidades
        int totalProbabilities = 0;


        foreach (AssetsRandom asset in assets)
        {
            totalProbabilities += asset.prob;
        }

        // Generar un número aleatorio dentro del rango total de probabilidades
        int randomValue = UnityEngine.Random.Range(1, totalProbabilities + 1);

        // Recorrer la lista de activos y seleccionar el objeto basado en probabilidades
        foreach (AssetsRandom asset in assets)
        {
            if (randomValue <= asset.prob)
            {
                // Activar el objeto seleccionado
                asset.asset.SetActive(true);
                if (asset.canRotate)
                {
                    float rndRotation = UnityEngine.Random.Range(0, 360f);
                    asset.asset.transform.Rotate(new Vector3(asset.asset.transform.rotation.x, rndRotation, asset.asset.transform.rotation.z));
                }
                break;
            }
            else
            {
                // Continuar con el siguiente objeto
                randomValue -= asset.prob;
            }
        }
    }
}

