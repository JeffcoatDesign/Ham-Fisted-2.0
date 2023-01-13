using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public GameObject[] planeSelection;
    private int selection;

    // Start is called before the first frame update
    void Start()
    {
        planeSelection[0].SetActive(false);
        planeSelection[1].SetActive(false);
        planeSelection[2].SetActive(false);

        SelectPlane();
    }

    void SelectPlane()
    {
        selection = Random.Range(0, 3);
        planeSelection[selection].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
