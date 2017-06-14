using UnityEngine;
using System.Collections;

public class MeleeWeaponTrail : MonoBehaviour
{
    bool trailOn = false;

	void Update ()
    {
	    if (trailOn)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}
