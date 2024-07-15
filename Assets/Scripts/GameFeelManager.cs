using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGameFeel;

public class GameFeelManager : MonoBehaviour
{
    public static GameFeelManager Instance;
    void Awake()
    {
        /*if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }*/

        Instance = this;

    }

    public float SliceStayTime;
    public Vector2 shakeMagNDura;
    public new ParticleSystem swipeParticle;
    public SwipeHandler swipeHandler;
    public float fruitBlastSize;
    public GameObject pf_BlastEffect;

    [HideInInspector] public bool shouldShake;

    private void Start()
    {
        SpawnFruits((int)Random.Range(2, 5));
    }
    public void ApplyShake()//Called to apply CamShake
    {
        if (!shouldShake) return;

        StartCoroutine(CameraShake.Shake(shakeMagNDura.x, shakeMagNDura.y));

        shouldShake = false;
    }

    public void playJuiceParticle(Vector2 pos, SwipeData swipe)//Called for Particle Effect
    {
        swipeParticle.transform.position = pos;

        float angle = AngleBetweenVector2(swipe.start, swipe.end);

        swipeParticle.transform.eulerAngles = new Vector3(0, 0, angle);

        swipeParticle.Play();
    }

    public void restrictInput(bool shouldRestrict)//Called to resetrict Input
    {
        if (shouldRestrict == swipeHandler.isRestricted) return;

        swipeHandler.isRestricted = shouldRestrict;
    }

    public void PlayExplosionEffect(Vector2 pos)
    {
        //Debug.Log("play explosion animation at " + pos);
        ParticleSystem blast =  Instantiate(pf_BlastEffect, pos, Quaternion.identity).GetComponent<ParticleSystem>();

        blast.Play();

        Destroy(blast.gameObject, 2);
    }

    public void RestartLevel()//Tobe called from the button
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SpawnFruits(int number)
    {
        GetComponent<FruitSpawner>().Spawn(number);
    }

    //Tools
    private float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
    {
        //Take Difference(Line) between two vectors
        Vector2 diference = vec2 - vec1;
        float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;

        //Add 180 and calc from left if vec2 < vec1 : 3rd and 4th Quadrant
        if (sign == -1f)
        {
            return Vector2.Angle(Vector2.left, diference) + 180;
        }
        else
        {
            return Vector2.Angle(Vector2.right, diference);
        }

    }
}
