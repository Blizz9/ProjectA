using UnityEngine;

[AddComponentMenu("ProjectA/Controllers/Player Controller")]
public class PlayerController : MonoBehaviour
{
    #region MonoBehaviour

    public void Awake()
    {
    }

    public void Start()
    {
    }

    public void Update()
    {
        if (Input.GetButtonDown("Jump"))
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 6f), ForceMode2D.Impulse);

        if (Input.GetAxis("Horizontal") < 0f)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(-5f, 0f), ForceMode2D.Force);
        }
        else if (Input.GetAxis("Horizontal") > 0f)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(5f, 0f), ForceMode2D.Force);
        }
    }

    public void LateUpdate()
    {
    }

    #endregion
}
