using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySpriteCutter;
using UnityGameFeel;


public class SwipeHandler : MonoBehaviour
{

    public LayerMask layerMask; //Layermask of the swiper
    public float minSwipeDistance; //Minimum Swipe Distance
    public float maxSwipeDistance;
    public Vector2 shakeMagNDura;
    public GameObject trailObject;


    [HideInInspector] public bool isRestricted = false; //For controlling the action of this script
    private Vector2 mouseStart;
    private Vector2 mouseEnd;
    private SwipeData thisSwipe = new SwipeData();
    private bool isSwiping = false;
    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            SwipeStart();
        }

        if(Input.GetMouseButton(0))
        {
            Swiping();
        }

        //If mouse Button is up and maximum swipe distance is reached
        if ((Input.GetMouseButtonUp(0) || (Mathf.Abs((mouseStart - mouseEnd).magnitude) >= maxSwipeDistance)) && isSwiping)
        {
            SwipeEnd();
        }

    }

    //Called once at start of swipe
    private void SwipeStart()
    {
        isSwiping = true;

        //Swipe Starting
        mouseStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        TrailHandlar.trailStart(trailObject, mouseStart);

        //Spawn the trail Game Object
    }
    //Continuosly called when Swiping
    private void Swiping()
    {
        if (!isSwiping) return;

        mouseEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        TrailHandlar.trailUpdate(mouseEnd);
        //trailgameObject position of mouse

    }
    //Called at ending of Swipe
    private void SwipeEnd()
    {
        TrailHandlar.trailEnd();

        isSwiping = false;

        if (Mathf.Abs((mouseStart - mouseEnd).magnitude) <= minSwipeDistance || isSwiping) return; //Checking for minimum SwipeDistance

        //Swipe Ending
        //Debug.Log("Now Cut " + (Mathf.Abs((mouseStart - mouseEnd).magnitude) >= minSwipeDistance).ToString());

        thisSwipe.start = mouseStart;
        thisSwipe.end = mouseEnd;

        LinecastCut(mouseStart, mouseEnd, layerMask.value);
        //Destroy trailGameObject
    }

    //For cutting the fruits
    private void LinecastCut(Vector2 lineStart, Vector2 lineEnd, int layerMask = Physics2D.AllLayers)
    {
        if (isRestricted) return; //If restricted do Nothing

        bool applyShake = false;

        List<GameObject> gameObjectsToCut = new List<GameObject>();//The GameObjects that will be cut

        RaycastHit2D[] hits = Physics2D.LinecastAll(lineStart, lineEnd, layerMask);//Casting a ray
        foreach (RaycastHit2D hit in hits)
        {
            if (HitCounts(hit))
            {
                gameObjectsToCut.Add(hit.transform.gameObject);
            }
        }

        foreach (GameObject go in gameObjectsToCut)
        {

            if (go.TryGetComponent(out Fruit fruit))//If it has a fruit element
            {

                /*if (fruit.isDragged || fruit.hasReachedMaxSlashed)
                    continue;*/
                if (fruit.isDragged)
                    continue;

                if (!fruit.inContactWithJar) continue;

                SpriteCutterOutput output = SpriteCutter.Cut(new SpriteCutterInput()
                {
                    lineStart = lineStart,
                    lineEnd = lineEnd,
                    gameObject = go,
                    gameObjectCreationMode = SpriteCutterInput.GameObjectCreationMode.CUT_OFF_COPY,
                });

                applyShake = true;
                //Has a fruit component
                fruit.AfterCut(output, thisSwipe);//Everything after the cut

            }
            else
            {
                //Doesnt have a fruit Component
                Debug.LogError("No Fruit Component detected on hit object");//Throw error
            }
        }

        GameFeelManager.Instance.shouldShake = applyShake;
    }
    bool HitCounts(RaycastHit2D hit)
    {
        return (hit.transform.GetComponent<SpriteRenderer>() != null ||
                 hit.transform.GetComponent<MeshRenderer>() != null);
    }
}
