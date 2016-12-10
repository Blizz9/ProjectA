using UnityEngine;

[AddComponentMenu("Ludum Dare 37/Controllers/Camera Controller")]
public class CameraController : MonoBehaviour
{
    private const float X_ZOOM_RATE = 1.777778f;
    private const float Y_ZOOM_RATE = 1f;
    private const float SIZE_ZOOM_RATE = 1f;

    public Vector3 ZoomLevel1;
    public Vector3 ZoomLevel2;
    public Vector3 ZoomLevel3;
    public Vector3 ZoomLevel4;
    public Vector3 ZoomLevel7;

    private int zoomLevelTarget;

    #region MonoBehaviour

    public void Awake()
    {
        Camera.main.transform.localPosition = new Vector3(ZoomLevel1.x, ZoomLevel1.y, Camera.main.transform.localPosition.z);

        Camera.main.orthographicSize = ZoomLevel1.z;
    }

    public void Update()
    {
        if (zoomLevelTarget == 0)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (Camera.main.orthographicSize == ZoomLevel1.z)
                    zoomLevelTarget = 2;
                else if (Camera.main.orthographicSize == ZoomLevel2.z)
                    zoomLevelTarget = 3;
                else if (Camera.main.orthographicSize == ZoomLevel3.z)
                    zoomLevelTarget = 4;
            }
        }
        else
        {
            Vector3 zoomValues = Vector3.zero;
            switch (zoomLevelTarget)
            {
                case 2:
                    zoomValues = ZoomLevel2;
                    break;

                case 3:
                    zoomValues = ZoomLevel3;
                    break;

                case 4:
                    zoomValues = ZoomLevel4;
                    break;
            }

            float x = Mathf.Clamp(Camera.main.transform.localPosition.x + X_ZOOM_RATE * Time.deltaTime, Camera.main.transform.localPosition.x, zoomValues.x);
            float y = Mathf.Clamp(Camera.main.transform.localPosition.y + Y_ZOOM_RATE * Time.deltaTime, Camera.main.transform.localPosition.y, zoomValues.y);
            Camera.main.transform.localPosition = new Vector3(x, y, Camera.main.transform.localPosition.z);

            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + SIZE_ZOOM_RATE * Time.deltaTime, Camera.main.orthographicSize, zoomValues.z);

            if ((x == zoomValues.x) && (y == zoomValues.y) && (Camera.main.orthographicSize == zoomValues.z))
                zoomLevelTarget = 0;
        }
    }

    #endregion
}
