using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[AddComponentMenu("Ludum Dare 37/Controllers/Intro Controller")]
public class IntroController : MonoBehaviour
{
	private bool _fadeDone;
	private bool _fading;

	#region MonoBehaviour

	public void Update()
	{
		if (Input.GetButtonDown("Jump"))
		{
			if (_fadeDone)
				SceneManager.LoadScene("MainScene");
			else
			{
				_fadeDone = true;
				_fading = true;
			}
		}

		if (_fading)
		{
			Color color = gameObject.SearchHierarchy(HierarchySearchType.All, true, "Cover").First().GetComponent<Image>().color;
			float alpha = color.a - 0.01f;
			color = new Color(color.r, color.g, color.b, alpha);

			gameObject.SearchHierarchy(HierarchySearchType.All, true, "Cover").First().GetComponent<Image>().color = color;
		}
	}

	#endregion
}
