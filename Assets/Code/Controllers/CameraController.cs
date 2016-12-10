using UnityEngine;

[AddComponentMenu("Ludum Dare 37/Controllers/Camera Controller")]
public class CameraController : MonoBehaviour
{
    private const float X_ZOOM_RATE = 1.777778f;
    private const float Y_ZOOM_RATE = 1f;
    private const float SIZE_ZOOM_RATE = 1f;

    public Vector3 ZoomLevel1;
    public Vector3 ZoomLevel2;

    private int zoomLevelTarget;

    #region MonoBehaviour

    public void Update()
    {
        if (Input.GetButtonDown("Jump"))
            zoomLevelTarget = 2;

        if (zoomLevelTarget != 0)
        {
            float x = Mathf.Clamp(Camera.main.transform.localPosition.x + X_ZOOM_RATE * Time.deltaTime, Camera.main.transform.localPosition.x, ZoomLevel2.x);
            float y = Mathf.Clamp(Camera.main.transform.localPosition.y + Y_ZOOM_RATE * Time.deltaTime, Camera.main.transform.localPosition.y, ZoomLevel2.y);
            Camera.main.transform.localPosition = new Vector3(x, y, Camera.main.transform.localPosition.z);

            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + SIZE_ZOOM_RATE * Time.deltaTime, Camera.main.orthographicSize, ZoomLevel2.z);

            if ((x == ZoomLevel2.x) && (y == ZoomLevel2.y) && (Camera.main.orthographicSize == ZoomLevel2.z))
                zoomLevelTarget = 0;
        }
    }

    #endregion
}
