using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformIntoScript : Photon.MonoBehaviour
{
    public GameObject[] ChangeInto;
    public GameObject TransformAvailableText;

    private bool nearTransform = false;
    public bool alreadyChanged = false;

    Mesh initialMesh;
    public Mesh swapMesh;

    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        initialMesh = GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    public void Update()
    {
        //Draws the ray from the camera to the crosshair

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.6f, 0f));
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
            //Debug.Log(hitInfo.transform.gameObject.name);
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.green);
        }

        //Gets the distance from the player to all the props that they can change into
        foreach (GameObject Change in ChangeInto)
        {
            if (Vector3.Distance(Change.transform.position, gameObject.transform.position) < 7f &&
                Vector3.Distance(Change.transform.position, gameObject.transform.position) > 1.5f)
            {
                nearTransform = true;
                break;
            }
            else
            {
                nearTransform = false;
            }
        }
        //if pressed, the player will change into the prop
        if (Input.GetMouseButtonDown(0) && hitInfo.transform.gameObject.tag == "Transform" && nearTransform == true)
        {
            //swapMesh = hitInfo.transform.gameObject.GetComponent<MeshFilter>().sharedMesh;
            //gameObject.GetComponent<MeshFilter>().mesh = swapMesh;
            //gameObject.transform.localScale = new Vector3(6.5f, 6.5f, 6.5f);


            PhotonNetwork.Instantiate(ChangeInto[0].name, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), 
                                      Quaternion.identity, 0);
            PhotonNetwork.Destroy(gameObject);
        }


        //enables the text that informs the player if he is able to transform into the object he's looking at
        if (nearTransform == true && hitInfo.transform.gameObject.tag == "Transform")
        {
            TransformAvailableText.SetActive(true);
        }
        else
        {
            TransformAvailableText.SetActive(false);
        }
    }
}
