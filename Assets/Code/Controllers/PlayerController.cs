using UnityEngine;

[AddComponentMenu("Ludum Dare 37/Controllers/Player Controller")]
public class PlayerController : MonoBehaviour
{
    private const float MOVE_RATE = 3f;

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
    }

    #endregion
}
