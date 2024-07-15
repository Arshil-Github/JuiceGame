using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    public Transform[] spawners; //Fruits will be spawner at random selection of these spawners                      
    public Sprite[] fruitsSprites;//All the spawnable Fruits
    public GameObject pf_reference;

    private List<GameObject> fruits;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            Spawn(5);
    }
    public void Spawn(int number)
    {

        for (int i = 0; i < number; i++)
        {
            Vector3 spawnVec = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0) + spawners[Random.Range(0, spawners.Length)].position;
            GameObject spawnedFruit = generateFruit(spawnVec, fruitsSprites[Random.Range(0, fruitsSprites.Length)]);
        }
    }

    private GameObject generateFruit(Vector3 pos, Sprite thisSprite)
    {
        GameObject _thisFruit = Instantiate(pf_reference);
        _thisFruit.name = thisSprite.name;
        _thisFruit.AddComponent<SpriteRenderer>().sprite = thisSprite;
        _thisFruit.transform.position = pos;

        StartCoroutine(colliderAddDelay(_thisFruit));

        return _thisFruit;
    }
    private IEnumerator colliderAddDelay(GameObject fruitObj)
    {
        yield return new WaitForSeconds(0.1f);
        fruitObj.AddComponent<PolygonCollider2D>();
    }

}
