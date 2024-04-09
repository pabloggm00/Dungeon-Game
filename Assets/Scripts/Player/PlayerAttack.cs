using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class PlayerAttack : MonoBehaviour
{

    Animator anim;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float speedBullet;
    [SerializeField] private Transform spawnShoot;
    [SerializeField] private ParticleSystem particleShot;

    [SerializeField] private float shotRate;
    private bool startShotRate;
    private bool canShoot = true;

    private GameObject poolParent;

    private List<GameObject> bulletActive = new();
    private List<GameObject> bulletPool = new();

    private float counter;

    private PlayerShoot playerShoot;
    private PlayerMove playerMove;

    private void OnEnable()
    {
        InputManager.playerControls.Player.Attack.performed += GetAttackInput;
        InputManager.playerControls.Player.Attack.canceled += GetAttackInput;
    }

    private void OnDisable()
    {
        InputManager.playerControls.Player.Attack.performed -= GetAttackInput;
        InputManager.playerControls.Player.Attack.canceled -= GetAttackInput;
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        playerShoot = GetComponent<PlayerShoot>();
        playerMove = GetComponent<PlayerMove>();
        // Creamos un objeto vacío para meter dentro las balas de la pool.
        poolParent = new GameObject("Pool Parent");
    }

    // Update is called once per frame
    void Update()
    {
        if (startShotRate)
        {
            counter += Time.deltaTime;

            if (counter > shotRate)
            {
                canShoot = true;
                startShotRate = false;
                counter = 0;
            }
        }
    }

    void GetAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed && canShoot)
        {
            anim.SetLayerWeight(0,0.5f);
            anim.SetLayerWeight(1, 0.5f);

            canShoot = false;

            anim.SetTrigger("Attack");
        }
    }

    public void BackToLayerBase()
    {
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 0);
    }

    public void Disparar()
    {

        //particleShot.gameObject.SetActive(true);

        // Comprobamos qué balas de la lista de balas en uso
        // se han descativado al colisionar y las devolvemos a la pool.
        foreach (GameObject bullet in bulletActive)
        {
            if (bullet.activeInHierarchy) continue;

            bulletPool.Add(bullet);
            bulletActive.Remove(bullet);
            break;
        }

        // Creamos una variable para almacenar la bala elegida.
        GameObject chosenBullet;

      
        // Si hay balas en la pool...
        if (bulletPool.Count > 0)
        {
            // Sacamos la primera y la movemos de la pool a la lista de balas en uso.
            chosenBullet = bulletPool[0];
            bulletPool.Remove(chosenBullet);
            bulletActive.Add(chosenBullet);
        }
        // Si no hay ninguna disponible...
        else
        {
            // La instanciamos para nunca quedarnos sin balas.
            chosenBullet = Instantiate(bulletPrefab, spawnShoot.position, Quaternion.identity, poolParent.transform);
            bulletActive.Add(chosenBullet);
        }

        chosenBullet.GetComponentInChildren<Bullet>().isPlayer = true;
        // Movemos la bala elegida a la posición de disparo y la activamos.
        chosenBullet.transform.position = spawnShoot.position;
        chosenBullet.SetActive(true);


        //Aplicamos la fuerza necesaria para que la bala se mueva
     
        //chosenBullet.GetComponentInChildren<Rigidbody>().AddForce(transform.forward * speedBullet, ForceMode.Impulse);
        chosenBullet.transform.LookAt(playerShoot.nuevaDireccion);


        startShotRate = true;

    }

}
