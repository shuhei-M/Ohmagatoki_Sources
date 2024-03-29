using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyDamageObj : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageableComponent damageableComponent = other.gameObject.GetComponent<IDamageableComponent>();
        if (damageableComponent == null) return;

        damageableComponent.Damage(20);
    }
}
