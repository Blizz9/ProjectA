using UnityEngine;

[AddComponentMenu("ProjectA/Controllers/Player Controller")]
public class PlayerController : MonoBehaviour
{
    #region MonoBehaviour

    // commonly used code-by-convention MonoBehaviour method stub https://docs.unity3d.com/Manual/ExecutionOrder.html
    public void Awake()
    {
    }

    // commonly used code-by-convention MonoBehaviour method stub https://docs.unity3d.com/Manual/ExecutionOrder.html
    public void Start()
    {
    }

    public void Update()
    {
        if (Input.GetButtonDown("Jump")) // button name comes from Input settings (Edit -> Project Settings -> Input)
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 6f), ForceMode2D.Impulse); // force is just a magic number that felt ok

        if (Input.GetAxis("Horizontal") < 0f) // axis name comes from Input settings (Edit -> Project Settings -> Input)
            GetComponent<Rigidbody2D>().AddForce(new Vector2(-5f, 0f), ForceMode2D.Force); // force is just a magic number that felt ok
        else if (Input.GetAxis("Horizontal") > 0f)
            GetComponent<Rigidbody2D>().AddForce(new Vector2(5f, 0f), ForceMode2D.Force);
    }

    // commonly used code-by-convention MonoBehaviour method stub https://docs.unity3d.com/Manual/ExecutionOrder.html
    public void LateUpdate()
    {
    }

    #endregion
}
