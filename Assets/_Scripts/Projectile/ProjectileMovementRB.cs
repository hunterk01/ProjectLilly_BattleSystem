using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovementRB : MonoBehaviour
{
    BattleController battleControl;
    Transform target;
    Vector3 targetHeightOffset = new Vector3(0, 1.25f, 0);

    public float speed;
    public float launchDelay = 0;
    private float launchDelayTimer;

    public bool targetAiming = true;

    void Start()
    {
        battleControl = GameObject.Find("BattleManager").GetComponent<BattleController>();
        target = battleControl.activeAgentList[0].targetGO.transform;
        launchDelayTimer = launchDelay;
    }


    void FixedUpdate()
    {
        if (targetAiming)
        {
            transform.LookAt(target.position + targetHeightOffset);
        }

        if (launchDelay > 0)
        {
            if (launchDelayTimer > 0)
            {
                launchDelayTimer -= Time.deltaTime;
            }
            else
            {
                transform.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
                launchDelayTimer = launchDelay;
            }
        }
        else
        {
            transform.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        }

    }
}
