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
    public Sprite WallTileKeyHoleSprite;

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
        GameObject magnetizedItem = null;

        if (Input.GetButton("Jump"))
        {
            if (transform.childCount > 0)
            {
                GameObject heldItem = transform.GetChild(0).gameObject;

                if (Input.GetButtonDown("Jump"))
                {
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
                                {
                                    gameObject.SearchHierarchy(HierarchySearchType.All, true, "ItemInstructions").First().SetActive(false);
                                    Destroy(heldItem);
                                }
                                else if (_currentRoom.name == "Room3")
                                {
                                    if (_currentRoom.SearchHierarchy(HierarchySearchType.Descendants, true, "HiddenKeyHole").FirstOrDefault() == null)
                                        Destroy(heldItem);
                                }

                                Debug.Log("Found hidden door");
                            }
                            else if (nearCollider.collider.name == "HiddenKeyHole")
                            {
                                GameObject wallTile = nearCollider.collider.gameObject;

                                wallTile.name = "KeyHole";
                                wallTile.GetComponent<BoxCollider2D>().size = new Vector2(0.2f, 1f);
                                wallTile.GetComponent<BoxCollider2D>().offset = new Vector2(0.1f, 0.5f);
                                BoxCollider2D secondCollider = wallTile.AddComponent<BoxCollider2D>();
                                secondCollider.size = new Vector2(0.2f, 1f);
                                secondCollider.offset = new Vector2(0.9f, 0.5f);
                                wallTile.GetComponent<SpriteRenderer>().sprite = WallTileKeyHoleSprite;

                                if (_currentRoom.SearchHierarchy(HierarchySearchType.Descendants, true, "HiddenDoorTile").FirstOrDefault() == null)
                                    Destroy(heldItem);

                                Debug.Log("Found hidden key hole");
                            }
                        }
                    }
                }

                if (heldItem.name == "Magnet")
                {
                    GameObject roomKey = _currentRoom.SearchHierarchy(HierarchySearchType.Children, true, "Key").First();

                    float distance = Vector2.Distance(new Vector2(roomKey.transform.position.x, roomKey.transform.position.y), new Vector2(heldItem.transform.position.x, heldItem.transform.position.y));

                    if (distance <= 7f)
                        magnetizedItem = roomKey;
                }
            }
        }

        if (Input.GetAxis("Horizontal") < 0f)
        {
            float x = transform.localPosition.x - MOVE_RATE * Time.deltaTime;
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
            GetComponent<SpriteRenderer>().sprite = PlayerLeftSprite;

            if (magnetizedItem != null)
            {
                float mix = magnetizedItem.transform.localPosition.x - MOVE_RATE * Time.deltaTime;
                magnetizedItem.transform.localPosition = new Vector3(mix, magnetizedItem.transform.localPosition.y, magnetizedItem.transform.localPosition.z);
            }
        }
        else if (Input.GetAxis("Horizontal") > 0f)
        {
            float x = transform.localPosition.x + MOVE_RATE * Time.deltaTime;
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
            GetComponent<SpriteRenderer>().sprite = PlayerRightSprite;

            if (magnetizedItem != null)
            {
                float mix = magnetizedItem.transform.localPosition.x + MOVE_RATE * Time.deltaTime;
                magnetizedItem.transform.localPosition = new Vector3(mix, magnetizedItem.transform.localPosition.y, magnetizedItem.transform.localPosition.z);
            }
        }

        if (Input.GetAxis("Vertical") < 0f)
        {
            float y = transform.localPosition.y - MOVE_RATE * Time.deltaTime;
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
            GetComponent<SpriteRenderer>().sprite = PlayerDownSprite;

            if (magnetizedItem != null)
            {
                float miy = magnetizedItem.transform.localPosition.y - MOVE_RATE * Time.deltaTime;
                magnetizedItem.transform.localPosition = new Vector3(magnetizedItem.transform.localPosition.x, miy, magnetizedItem.transform.localPosition.z);
            }
        }
        else if (Input.GetAxis("Vertical") > 0f)
        {
            float y = transform.localPosition.y + MOVE_RATE * Time.deltaTime;
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
            GetComponent<SpriteRenderer>().sprite = PlayerUpSprite;

            if (magnetizedItem != null)
            {
                float miy = magnetizedItem.transform.localPosition.y + MOVE_RATE * Time.deltaTime;
                magnetizedItem.transform.localPosition = new Vector3(magnetizedItem.transform.localPosition.x, miy, magnetizedItem.transform.localPosition.z);

                if (magnetizedItem.name == "Key")
                {
                    if (miy >= 1.5f)
                    {
                        GameObject magnet = transform.GetChild(0).gameObject;
                        Destroy(magnet);

                        Destroy(magnetizedItem.GetComponent<Rigidbody2D>());
                        magnetizedItem.GetComponent<Collider2D>().isTrigger = true;
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
            else if (collision.gameObject.name == "Magnet")
            {
                dropHeldItem();

                GameObject magnet = collision.gameObject;

                magnet.transform.parent = gameObject.transform;
                magnet.transform.localPosition = new Vector3(-0.333f, -0.667f, 0f);
                magnet.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                magnet.GetComponent<Collider2D>().enabled = false;

                Debug.Log("Picked up magnet");
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
