using UnityEngine;

[AddComponentMenu("Ludum Dare 37/Controllers/Camera Controller")]
public class CameraController : MonoBehaviour
{
    public float ZoomRate = 1f;

    public Vector3 ZoomLevel1;
    public Vector3 ZoomLevel2;
    public Vector3 ZoomLevel3;
    public Vector3 ZoomLevel4;
	public Vector3 ZoomLevel5;

    [HideInInspector] public int ZoomLevelTarget;

    #region MonoBehaviour

    public void Awake()
    {
        Camera.main.transform.localPosition = new Vector3(ZoomLevel1.x, ZoomLevel1.y, Camera.main.transform.localPosition.z);
        Camera.main.orthographicSize = ZoomLevel1.z;
    }

    public void Update()
    {
        if (ZoomLevelTarget == 0)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                if (Camera.main.orthographicSize == ZoomLevel1.z)
                    ZoomLevelTarget = 2;
                else if (Camera.main.orthographicSize == ZoomLevel2.z)
                    ZoomLevelTarget = 3;
                else if (Camera.main.orthographicSize == ZoomLevel3.z)
                    ZoomLevelTarget = 4;
            }
        }
        else
        {
            Vector3 zoomValues = Vector3.zero;
            switch (ZoomLevelTarget)
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

				case 5:
					zoomValues = ZoomLevel5;
					break;
            }

            float xZoomRate = ZoomRate * 1.777778f;

            float x = Camera.main.transform.localPosition.x;
			float y = Camera.main.transform.localPosition.y;

			if (x < zoomValues.x)
				x = Mathf.Clamp(Camera.main.transform.localPosition.x + xZoomRate * Time.deltaTime, x, zoomValues.x);
			else
				x = Mathf.Clamp(Camera.main.transform.localPosition.x - xZoomRate * Time.deltaTime, zoomValues.x, x);

			if (y < zoomValues.y)
            	y = Mathf.Clamp(Camera.main.transform.localPosition.y + ZoomRate * Time.deltaTime, y, zoomValues.y);
			else
				y = Mathf.Clamp(Camera.main.transform.localPosition.y - ZoomRate * Time.deltaTime, zoomValues.y, y);
			
            Camera.main.transform.localPosition = new Vector3(x, y, Camera.main.transform.localPosition.z);

            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + ZoomRate * Time.deltaTime, Camera.main.orthographicSize, zoomValues.z);

            if ((x == zoomValues.x) && (y == zoomValues.y) && (Camera.main.orthographicSize == zoomValues.z))
                ZoomLevelTarget = 0;
        }
    }

    #endregion
}
