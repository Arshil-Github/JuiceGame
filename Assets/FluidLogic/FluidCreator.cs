using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidCreator : MonoBehaviour
{
    public float quadZPos;
    public float paritcleZPos = 0;

    public GameObject pf_liquidParticle;
    public Transform liquidParentObject;

    public Camera fluidCam;
    public Transform quadPlane;
    // Start is called before the first frame update
    void Awake()
    {
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetMouseButtonDown(0))
        {
            Vector3 spawnLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            spawnLoc.z = paritcleZPos;
            CreateWater(spawnLoc, 1);
        }*/
    }
    public void CreateWater(Vector3 location, float completionTime, int particleNumber = 100, float lifeSpan = 0f)
    {
        StartCoroutine(delayWaterCreation(location, completionTime, particleNumber, lifeSpan));
    }
    public IEnumerator delayWaterCreation(Vector3 location, float completionTime, int particleNumber = 100, float lifeSpan = 0f)
    {
        //Spawn Particles at specific locations
        //Spawn Particles at specific locations

        Vector3 spawnOffset = new Vector3();
        int spawnRadius = 2;

        float time = completionTime / particleNumber;

        for (int i = 0; i < particleNumber; i++)
        {
            GameObject _Lparticle = Instantiate(pf_liquidParticle, location + spawnOffset, Quaternion.identity);
            _Lparticle.transform.SetParent(liquidParentObject, true);

            if (lifeSpan != 0f)
                Destroy(_Lparticle, lifeSpan);

            spawnOffset = new Vector3(Random.Range(-0.1f, 0.1f) * spawnRadius, Random.Range(-0.3f, 0.3f) * spawnRadius, 0);


            yield return new WaitForSeconds(time);
        }
    }
    private void SetUp()
    {
        /*float orthographicSize = Camera.main.orthographicSize;
        if (fluidCam != null)
        {
            fluidCam.orthographicSize = orthographicSize;
            quadPlane.localScale = new Vector3(orthographicSize * 4, orthographicSize * 2, 0);
            quadPlane.localPosition = new Vector3(0, 0, quadZPos);
        }*/
    }
}
