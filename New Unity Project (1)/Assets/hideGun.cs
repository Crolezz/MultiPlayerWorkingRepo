using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hideGun : MonoBehaviour
{
    public PhotonView PV;
    Renderer R;

    void Start()
    {
        R = GetComponent<Renderer>();
        PV = GetComponent<PhotonView>();
    }
    // Update is called once per frame
    void Update()
    {
        if (PV.isMine)
        {
            R.enabled = false;
        }
    }
}
