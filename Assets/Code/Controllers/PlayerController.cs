using System.Linq;
using UnityEngine;

[AddComponentMenu("Ludum Dare 37/Controllers/Player Controller")]
public class PlayerController : MonoBehaviour
{
    private const float MOVE_RATE = 3f;

    public Sprite FloorTileSprite;

    private bool _ignoreNextTriggerEnter;

    #region MonoBehaviour

    public void Update()
    {
        if (Input.GetAxis("Horizontal") < 0f)
        {
            float x = transform.localPosition.x - MOVE_RATE * Time.deltaTime;
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
        }
        else if (Input.GetAxis("Horizontal") > 0f)
        {
            float x = transform.localPosition.x + MOVE_RATE * Time.deltaTime;
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
        }

        if (Input.GetAxis("Vertical") < 0f)
        {
            float y = transform.localPosition.y - MOVE_RATE * Time.deltaTime;
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
        }
        else if (Input.GetAxis("Vertical") > 0f)
        {
            float y = transform.localPosition.y + MOVE_RATE * Time.deltaTime;
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
        }

        // stub out what it is like to drop an item, we will use this later
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject key = gameObject.SearchHierarchy(HierarchySearchType.Children, true, "Key").FirstOrDefault();

            if (key != null)
            {
                GameObject room1 = gameObject.SearchHierarchy(HierarchySearchType.Siblings, true, "Room1").First();
                GameObject room1DoorTile = room1.SearchHierarchy(HierarchySearchType.Descendants, true, "DoorTile").First();
                room1DoorTile.GetComponent<Collider2D>().isTrigger = false;

                key.transform.parent = room1.transform;
                key.transform.localScale = Vector3.one;

                _ignoreNextTriggerEnter = true;
                key.GetComponent<Collider2D>().enabled = true;
            }
        }
    }

    #endregion

    #region Collisions

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (_ignoreNextTriggerEnter)
        {
            _ignoreNextTriggerEnter = false;
        }
        else
        {
            if (collision.gameObject.name == "Key")
            {
                Debug.Log("Picked up key");

                GameObject key = collision.gameObject;

                key.transform.parent = gameObject.transform;
                key.transform.localPosition = Vector3.zero;
                key.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                key.GetComponent<Collider2D>().enabled = false;

                GameObject room1 = gameObject.SearchHierarchy(HierarchySearchType.Siblings, true, "Room1").First();
                GameObject room1DoorTile = room1.SearchHierarchy(HierarchySearchType.Descendants, true, "DoorTile").First();
                room1DoorTile.GetComponent<Collider2D>().isTrigger = true;
            }
            else if (collision.gameObject.name == "DoorTile")
            {
                Debug.Log("Unlocked door");

                GameObject doorTile = collision.gameObject;

                doorTile.GetComponent<SpriteRenderer>().sprite = FloorTileSprite;
                Destroy(doorTile.GetComponent<Collider2D>());

                GameObject key = gameObject.SearchHierarchy(HierarchySearchType.Children, true, "Key").First();
                Destroy(key);

                Camera.main.GetComponent<CameraController>().ZoomLevelTarget = 2;
            }
        }
    }

    #endregion
}
