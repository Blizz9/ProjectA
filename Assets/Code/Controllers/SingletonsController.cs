using System.Linq;
using UnityEngine;

[AddComponentMenu("ProjectA/Controllers/Singletons Controller")]
public class SingletonsController : MonoBehaviour
{
    #region MonoBehaviour

    public void Awake()
    {
        Singleton.ProvideState(gameObject.SearchHierarchy<StateController>(HierarchySearchType.Children, false).First());
        Singleton.ProvideMainCamera(gameObject.SearchHierarchy(HierarchySearchType.Siblings, false, "Camera").First().GetComponent<Camera>());
    }

    #endregion
}
