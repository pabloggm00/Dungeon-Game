using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShoot : MonoBehaviour
{

    public Image indicatorShoot;
    
    public Transform cam;
    public float rango;
    public LayerMask ignoreMask;

    public Vector3 nuevaDireccion;
    public Ray raycast;

    private PlayerMove playerMove;

    // Start is called before the first frame update
    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        Detector();
    }

    private void Detector()
    {

        RaycastHit hit;
        raycast = Camera.main.ScreenPointToRay(playerMove._look);


        if (Physics.Raycast(cam.position, cam.forward, out hit, rango, ~ignoreMask))
        {
   
            nuevaDireccion = hit.point;
            indicatorShoot.color = Color.white;

            if (hit.collider.CompareTag("Objetos"))
            {
        
                indicatorShoot.color = Color.red;
            }


        }
        else
        {
            nuevaDireccion = cam.position + cam.forward * 10f;
            indicatorShoot.color = Color.white;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(cam.position, cam.forward * rango);
    }
}
