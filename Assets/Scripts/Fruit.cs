using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySpriteCutter;
using UnityEngine.EventSystems;
using UnityGameFeel;

public class Fruit : MonoBehaviour
{
    public float juiceAmount;//Juice to be released
    public int maxSlashes; //,Max amount of slashes allowed
    public float deleteAfter;


    private Jar jar;
    public int numberOfSlashes = 0;
    [HideInInspector] public bool isDragged = false;
    [HideInInspector] public bool hasReachedMaxSlashed;

    public bool inContactWithJar = false;
    private Vector3 offset;
    private void Start()
    {
        jar = FindFirstObjectByType<Jar>();

        Physics2D.IgnoreLayerCollision(gameObject.layer, 6);//Ignore Liquids
        Physics2D.IgnoreLayerCollision(gameObject.layer, 7);//Ignore Juice Layer

    }
    //To be called after cutting
    public void AfterCut(SpriteCutterOutput _cutterOutput, SwipeData _swipeData)
    {
        numberOfSlashes++;//Add a slash
        hasReachedMaxSlashed = (maxSlashes <= numberOfSlashes);

        Fruit firstFruit = _cutterOutput.firstSideGameObject.GetComponent<Fruit>();//This one
        Fruit secondFruit = _cutterOutput.secondSideGameObject.GetComponent<Fruit>();//the otherone

        //Apply ForcewithDelay
        StartCoroutine(addForceWithDelay(firstFruit, secondFruit, _swipeData));

        //For controlling numberofSlashes
        firstFruit.numberOfSlashes = numberOfSlashes;
        secondFruit.numberOfSlashes = numberOfSlashes;

        //Smaller piece will release less Juice
        firstFruit.juiceAmount = juiceAmount / 2;
        secondFruit.juiceAmount = juiceAmount / 2;


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "jarEffector")
        {
            //add this fruit to jar's List on contact
            inContactWithJar = true;
            jar.fruitsInJar.Add(this);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "jarEffector")
        {
            //add this fruit to jar's List on contact
            inContactWithJar = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "jarEffector")
        {
            jar.fruitsInJar.Remove(this);
            inContactWithJar = false;
        }
    }

    //Mouse Functions
    private void OnMouseDown()
    {
        DragStart();
    }
    private void OnMouseDrag()
    {
        Dragging();
    }
    private void OnMouseUp()
    {
        StartCoroutine(DragEnd());
    }

    //Drag Functions
    private void DragStart()
    {
        //Start of the drag
        GetComponent<Collider2D>().enabled = false; //Disable the collider

        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);//Offset

        //Stop SwipeHandler
    }
    private void Dragging()
    {
        isDragged = true;

        //Change Position
        float originalZ = transform.position.z;
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        transform.position -= new Vector3(0, 0, transform.position.z - originalZ);
    }
    private IEnumerator DragEnd()
    {
        GetComponent<Collider2D>().enabled = true; //enable the collider

        yield return new WaitForSeconds(0.5f);

        isDragged = false;
    }

    private IEnumerator addForceWithDelay(Fruit first, Fruit second, SwipeData _swipeData)
    {
        Rigidbody2D rbOne = first.GetComponent<Rigidbody2D>();
        Rigidbody2D rbTwo = second.GetComponent<Rigidbody2D>();

        rbOne.velocity = Vector3.zero;
        rbTwo.velocity = Vector3.zero;

        rbOne.isKinematic = true;
        rbTwo.isKinematic = true;

        GameFeelManager.Instance.swipeParticle.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(GameFeelManager.Instance.SliceStayTime);

        rbOne.isKinematic = false;
        rbTwo.isKinematic = false;

        //These variables control the force added on the particle after cutting
        Vector2 forceDir = Vector2.Perpendicular(_swipeData.end - _swipeData.start);
        float FORCEONPIECES = 5;
        //Adding Force on each particle in perpendicular dirn
        first.GetComponent<Rigidbody2D>().AddForce(forceDir * FORCEONPIECES, ForceMode2D.Impulse);
        second.GetComponent<Rigidbody2D>().AddForce(-forceDir * FORCEONPIECES, ForceMode2D.Impulse);
        //Play a blast animation
        GameFeelManager.Instance.PlayExplosionEffect(transform.position);

        GameFeelManager.Instance.playJuiceParticle(transform.position, _swipeData);

        GameFeelManager.Instance.ApplyShake();

        AndroidManager.HapticFeedback();

        CheckForSize();
        second.CheckForSize();

        if (inContactWithJar) //If not in contact do all the things below
            jar.AddJuice(juiceAmount);//Release Juice


    }

    private MeshRenderer myMesh;

    public float meshSize;
    //Check for mesh size and if it is below a certain value blow it
    private void CheckForSize()
    {
        bool hasMesh = TryGetComponent(out myMesh);
        if (!hasMesh) return;//Doesnt have a Mesh Renderer Right now

        meshSize = myMesh.bounds.size.magnitude;
        if(meshSize <= GameFeelManager.Instance.fruitBlastSize)//If size below a certain threshold blast it
        {

            if(inContactWithJar)
                jar.fruitsInJar.Remove(this);

            //Destroy this
            Destroy(gameObject);
        }
    }

    //This Section is to be deleted in the final build. use this to make your life easier
    /*Rect rect = new Rect(0, 0, 600, 200);
    Vector3 offsetText = new Vector3(0f, 0f, 0f); // height above the target position

    void OnGUI()
    {
        if (!showText) return;

        Vector2 point = Camera.main.WorldToScreenPoint(transform.position + offsetText);
        rect.x = point.x;
        rect.y = Screen.height - point.y - rect.height; // bottom left corner set to the 3D point

        GUI.Label(rect, myMesh.bounds.size.magnitude.ToString()); // display its name, or other string

    }*/
}
