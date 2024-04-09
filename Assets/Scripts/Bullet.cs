using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;  // Velocidad de la bala
    [SerializeField] private GameObject particleCollision;

    public int damageAttack;
    [HideInInspector] public bool isPlayer;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

   

    private void OnTriggerEnter(Collider other)
    {
        //particleCollision.SetActive(true);
        Debug.Log(other.name);
        gameObject.SetActive(false);
        GetComponent<SphereCollider>().enabled = false;

        if (other.TryGetComponent<IDamage>(out IDamage damage))
        {
            damage.DoDamage(damageAttack, isPlayer);
        }

        //StartCoroutine(HideBullet());

    }

    IEnumerator HideBullet()
    {
        yield return new WaitForSeconds(particleCollision.GetComponent<ParticleSystem>().main.duration);

        this.gameObject.SetActive(false);
    }
}
