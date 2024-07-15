using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jar : MonoBehaviour
{
    //Drag and drop handlar
    //Water level handlar

    public Transform waterLevelTransform;
    public float maxWaterLevel;
    public float rateOfFill;
    public float minWaterLevelforDrag;
    public float holdTime;
    public List<Fruit> fruitsInJar;
    public AnimationClip drinkAnim;
    public AnimationClip idleAnim;
    public Transform glass;
    public ParticleSystem mixEffect;

    [HideInInspector]public float currentWaterLevel = 0;
    Vector2 newScale;
    bool isFull = false;
    [HideInInspector] public bool isDragged;
    private Vector3 originalJarPos = new Vector3();

    private bool inContactWithPanda = false;
    private Vector3 originalPos;
   // private bool canKill = false;

    private float _tempTime = 0;
    //private bool holdCheck = false;
   // private bool canMove = false;
    private Vector3 offset;//Offset on Hold
    private Vector3 originalMousePos = new Vector3();

    private float targetWaterLevel = 0f;
    private bool isPouring = false;

    private enum HoldStatus
    {
        HoldStarted,
        HoldSuccess,
        HoldCanceled
    }
    private HoldStatus _holdStatus;

    private void Awake()
    {
        fruitsInJar = new List<Fruit>();
    }
    private void Start()
    {
        currentWaterLevel = waterLevelTransform.localScale.y; //setting current waterlevel
        targetWaterLevel = currentWaterLevel;
    }
    public  void AddJuice(float amount) //Adds Juice
    {

        if (isFull && amount > 0) return;//If full do nothing


        if (targetWaterLevel + amount > maxWaterLevel)
        {
            //Check if full this time
            amount = maxWaterLevel - targetWaterLevel;
            isFull = true;
        }

        targetWaterLevel = targetWaterLevel + amount;
        isPouring = true;

        //All the effects at the start of the pour
        waterLevelTransform.GetComponent<AudioSource>().Play();
    }

    public void DrinkAnimation(Vector3 v)//Plays Drinking Anim
    {
        GetComponent<Animator>().applyRootMotion = false;
        GetComponent<Animator>().Play(drinkAnim.name);

        originalJarPos = v;

    }
    public void DecreaseDrinkWater(float timeToDrink)//Called from Drinking Anim
    {
        StartCoroutine(delayDecreaseWater(timeToDrink));
    }
    IEnumerator delayDecreaseWater(float drinkingTime)
    {
        if(maxWaterLevel - 0.2f < waterLevelTransform.localScale.y)
        {
            waterLevelTransform.localScale = new Vector3(waterLevelTransform.localScale.x, maxWaterLevel - 0.5f, waterLevelTransform.localScale.z);

            currentWaterLevel = waterLevelTransform.localScale.y;
        }

        int sips = 10;
        float timeDelay = drinkingTime / sips; 

        for (int i = 0; i <= sips; i++)
        {
            AddJuice(-(currentWaterLevel / sips));

            yield return new WaitForSeconds(timeDelay);
        }

        //After the animation
        GetComponent<Animator>().applyRootMotion = true;


        FindAnyObjectByType<SwipeHandler>().isRestricted = false;
        glass.GetComponent<Collider2D>().enabled = true;
        //reset pos
        GetComponent<Animator>().Play(idleAnim.name);
        transform.position = originalJarPos;

        GameFeelManager.Instance.SpawnFruits((int) Random.Range(2, 5));
    }

    private Vector3 originalGlassScale;

    private void Update()
    {
        if (Mathf.Abs(currentWaterLevel - targetWaterLevel) >= 0.1f && isPouring)
        {
            currentWaterLevel = Mathf.Lerp(currentWaterLevel, targetWaterLevel, Time.deltaTime);
            waterLevelTransform.localScale = new Vector3(waterLevelTransform.localScale.x, currentWaterLevel, waterLevelTransform.localScale.z);

        }
        else if(isPouring)
        {
            //All the after pour effects
            waterLevelTransform.GetComponent<AudioSource>().Stop();

            isPouring = false;
        }
        //Keeping Track Of Hold
        _tempTime += Time.deltaTime;
        if (_holdStatus == HoldStatus.HoldStarted && Mathf.Abs((originalMousePos - Input.mousePosition).magnitude) >= 10f)
        {
            //CancelHold
            //Checks if the mouse is holding in a certain area if no stop hold
            //holdCheck = false;
            _holdStatus = HoldStatus.HoldCanceled;

        }

        if (_tempTime >= holdTime && _holdStatus == HoldStatus.HoldStarted)
        {
            //After Hold Events

            _holdStatus = HoldStatus.HoldSuccess;


            //Destroy all fruits in the jar
            DestoyAllFruits();
        }

    }
    public void HoldStart()
    {

        //Start Checking for hold
        _holdStatus = HoldStatus.HoldStarted;

        //Start Effect
        StartCoroutine(HoldEffect());

        //Start Hold Timer
        _tempTime = 0;

        //Store this pos
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        originalPos = transform.position;
        originalMousePos = Input.mousePosition;
        originalGlassScale = glass.transform.localScale;


    }
    public void Dragging()
    {
        if (currentWaterLevel < minWaterLevelforDrag || !(_holdStatus == HoldStatus.HoldSuccess))//Checking if minWater level 
            return;

        GameFeelManager.Instance.restrictInput(true);//Avoid Slashing

        if (glass.GetComponent<Collider2D>().enabled) glass.GetComponent<Collider2D>().enabled = false;

        float originalZ = transform.position.z;

        //Actual Dragging
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        transform.position -= new Vector3(0, 0, transform.position.z - originalZ);
    }

    public void HoldEnd()
    {
        StopAllCoroutines();

        if (_holdStatus == HoldStatus.HoldSuccess) //canMOve true = hold successful
        {
            //check if in contact with the acceptor
            //if yes play anim in teddy
            //if no snap to original possition
            if (inContactWithPanda)
            {
                DrinkAnimation(originalPos);
            }
            else
            {
                transform.position = originalPos;
                glass.transform.localScale = originalGlassScale;
            }
        }
        inContactWithPanda = false;
    }

    private IEnumerator HoldEffect()
    {
        float _t = 0;
        while(_t < holdTime && _holdStatus == HoldStatus.HoldStarted)
        {
            glass.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f) * Time.deltaTime;
            _t += Time.deltaTime;
            yield return null;
        }

        if(_holdStatus == HoldStatus.HoldCanceled)
        {
            glass.transform.localScale = originalGlassScale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Panda"))
            inContactWithPanda = true;
    }
    private void DestoyAllFruits()
    {
        if (!(_holdStatus == HoldStatus.HoldSuccess)) return;


        if (fruitsInJar.Count != 0)
        {
            mixEffect.Play();
            mixEffect.GetComponent<AudioSource>().Play();

            GameFeelManager.Instance.shouldShake = true;
            GameFeelManager.Instance.ApplyShake();
        }

        foreach (Fruit fruit in fruitsInJar.ToArray())
        {
            Destroy(fruit.gameObject);
        }
    }
}
