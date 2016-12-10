using UnityEngine;

[AddComponentMenu("Ludum Dare 37/Controllers/Wall Tile Controller")]
public class WallTileController : MonoBehaviour
{
    #region MonoBehaviour

    public void Update()
    {
        Color currentColor = GetComponent<SpriteRenderer>().color;

        if (currentColor.a < 1f)
        {
            float alpha = Mathf.Clamp01(currentColor.a + 0.75f * Time.deltaTime);
            GetComponent<SpriteRenderer>().color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
        }
    }

    #endregion
}
