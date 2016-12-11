using System.Linq;
using UnityEngine;

[AddComponentMenu("Ludum Dare 37/Controllers/Player Controller")]
public class PlayerController : MonoBehaviour
{
    private const float MOVE_RATE = 3f;

    public Sprite PlayerUpSprite;
    public Sprite PlayerDownSprite;
    public Sprite PlayerLeftSprite;
    public Sprite PlayerRightSprite;

    public Sprite FloorTileSprite;
    public Sprite DoorTileSprite;

    private GameObject _currentRoom;

    private bool _ignoreNextTriggerEnter;

    #region MonoBehaviour

    public void Awake()
    {
        _currentRoom = gameObject.SearchHierarchy(HierarchySearchType.Siblings, true, "Room1").First();
        gameObject.SearchHierarchy(HierarchySearchType.All, true, "UI").First().SetActive(true);
        gameObject.SearchHierarchy(HierarchySearchType.All, true, "MoveInstructions").First().SetActive(true);
    }

    public void Update()
    {
        if (Input.GetAxis("Horizontal") < 0f)
        {
            float x = transform.localPosition.x - MOVE_RATE * Time.deltaTime;
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
            GetComponent<SpriteRenderer>().sprite = PlayerLeftSprite;
        }
        else if (Input.GetAxis("Horizontal") > 0f)
        {
            float x = transform.localPosition.x + MOVE_RATE * Time.deltaTime;
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
            GetComponent<SpriteRenderer>().sprite = PlayerRightSprite;
        }

        if (Input.GetAxis("Vertical") < 0f)
        {
            float y = transform.localPosition.y - MOVE_RATE * Time.deltaTime;
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
            GetComponent<SpriteRenderer>().sprite = PlayerDownSprite;
        }
        else if (Input.GetAxis("Vertical") > 0f)
        {
            float y = transform.localPosition.y + MOVE_RATE * Time.deltaTime;
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
            GetComponent<SpriteRenderer>().sprite = PlayerUpSprite;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (transform.childCount > 0)
            {
                GameObject heldItem = transform.GetChild(0).gameObject;

                Debug.Log("Using " + heldItem.name);

                if (heldItem.name == "Sonar")
                {
                    heldItem.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Ping");
                    heldItem.GetComponent<AudioSource>().Play();

                    RaycastHit2D[] nearColliders = Physics2D.CircleCastAll(transform.position, 1.3f, Vector2.zero);
                    foreach (RaycastHit2D nearCollider in nearColliders)
                    {
                        if (nearCollider.collider.name == "WallTile")
                        {
                            nearCollider.collider.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.25f);
                        }
                        else if (nearCollider.collider.name == "HiddenDoorTile")
                        {
                            GameObject doorTile = nearCollider.collider.gameObject;

                            doorTile.name = "DoorTile";
                            doorTile.GetComponent<SpriteRenderer>().sprite = DoorTileSprite;

                            if (_currentRoom.name == "Room2")
                                gameObject.SearchHierarchy(HierarchySearchType.All, true, "ItemInstructions").First().SetActive(false);

                            Destroy(heldItem);

                            Debug.Log("Found hidden door");
                        }
                    }
                }
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
                dropHeldItem();

                GameObject key = collision.gameObject;

                key.transform.parent = gameObject.transform;
                key.transform.localPosition = Vector3.zero;
                key.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                key.GetComponent<Collider2D>().enabled = false;

                GameObject roomDoorTile = _currentRoom.SearchHierarchy(HierarchySearchType.Descendants, true, "DoorTile").FirstOrDefault();
                if (roomDoorTile != null)
                    roomDoorTile.GetComponent<Collider2D>().isTrigger = true;

                key.GetComponent<AudioSource>().Play();

                Debug.Log("Picked up key");
            }
            else if (collision.gameObject.name == "DoorTile")
            {
                GameObject doorTile = collision.gameObject;

                doorTile.GetComponent<SpriteRenderer>().sprite = FloorTileSprite;
                Destroy(doorTile.GetComponent<Collider2D>());

                GameObject key = gameObject.SearchHierarchy(HierarchySearchType.Children, true, "Key").First();
                Destroy(key);

                switch (_currentRoom.name)
                {
                    case "Room1":
                        gameObject.SearchHierarchy(HierarchySearchType.All, true, "MoveInstructions").First().SetActive(false);
                        _currentRoom = gameObject.SearchHierarchy(HierarchySearchType.Siblings, true, "Room2").First();
                        Camera.main.GetComponent<CameraController>().ZoomLevelTarget = 2;
                        break;

                    case "Room2":
                        _currentRoom = gameObject.SearchHierarchy(HierarchySearchType.Siblings, true, "Room3").First();
                        Camera.main.GetComponent<CameraController>().ZoomLevelTarget = 3;
                        break;
                }

                doorTile.GetComponents<AudioSource>().ToList().ForEach(aus => aus.Play());

                Debug.Log("Unlocked door");
            }
            else if (collision.gameObject.name == "Sonar")
            {
                dropHeldItem();

                if (_currentRoom.name == "Room2")
                    gameObject.SearchHierarchy(HierarchySearchType.All, true, "ItemInstructions").First().SetActive(true);

                GameObject sonar = collision.gameObject;

                sonar.transform.parent = gameObject.transform;
                sonar.transform.localPosition = Vector3.zero;
                sonar.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                sonar.GetComponent<Collider2D>().enabled = false;

                Debug.Log("Picked up sonar");
            }
        }
    }

    #endregion

    #region Items

    private void dropHeldItem()
    {
        if (transform.childCount > 0)
        {
            GameObject heldItem = transform.GetChild(0).gameObject;

            if (heldItem.name == "Key")
            {
                GameObject roomDoorTile = _currentRoom.SearchHierarchy(HierarchySearchType.Descendants, true, "DoorTile").FirstOrDefault();
                if (roomDoorTile != null)
                    roomDoorTile.GetComponent<Collider2D>().isTrigger = false;
            }
            else if (heldItem.name == "Sonar")
            {
                if (_currentRoom.name == "Room2")
                    gameObject.SearchHierarchy(HierarchySearchType.All, true, "ItemInstructions").First().SetActive(false);
            }

            heldItem.transform.parent = _currentRoom.transform;
            heldItem.transform.localScale = Vector3.one;

            _ignoreNextTriggerEnter = true;
            heldItem.GetComponent<Collider2D>().enabled = true;

            Debug.Log("Dropped " + heldItem.name);
        }
    }

    #endregion
}
