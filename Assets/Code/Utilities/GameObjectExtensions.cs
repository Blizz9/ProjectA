using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class GameObjectExtensions
{
    #region Hierarchy Search

    public static IEnumerable<GameObject> SearchHierarchy(this GameObject self, HierarchySearchType searchType, bool doSearchInactive, List<string> names, List<Type> types)
    {
        bool doSearchParent = (searchType & HierarchySearchType.Parent) != 0;
        bool doSearchAncestors = (searchType & HierarchySearchType.Ancestors) != 0;
        bool doSearchChildren = (searchType & HierarchySearchType.Children) != 0;
        bool doSearchDescendants = (searchType & HierarchySearchType.Descendants) != 0;
        bool doSearchSiblings = (searchType & HierarchySearchType.Siblings) != 0;
        bool doSearchNiblings = (searchType & HierarchySearchType.Niblings) != 0;
        bool doSearchAll = (searchType & HierarchySearchType.All) != 0;

        List<GameObject> foundGameObjects = new List<GameObject>();

        if (doSearchAll)
        {
            if (!doSearchInactive && (names != null) && (names.Count == 1) && ((types == null) || !types.Any()))
            {
                foundGameObjects.Add(GameObject.Find(names.First()));
            }
            else
            {
                searchRoots(true, true, doSearchInactive, names, types, self, foundGameObjects);
            }
        }
        else
        {
            GameObject parent = null;

            if (doSearchParent || doSearchAncestors || doSearchSiblings || doSearchNiblings)
            {
                if (self.transform.parent != null)
                {
                    parent = self.transform.parent.gameObject;

                    if (doSearchParent || doSearchAncestors)
                        addGameObjectIfValid(parent, doSearchInactive, names, types, self, foundGameObjects);
                }

                if (doSearchSiblings || doSearchNiblings)
                {
                    if (parent == null)
                        searchRoots(doSearchNiblings, false, doSearchInactive, names, types, self, foundGameObjects);
                    else
                        searchChildren(parent, doSearchNiblings, false, doSearchInactive, names, types, self, foundGameObjects);
                }

                if (doSearchAncestors && (parent != null))
                    searchAncestors(parent, doSearchInactive, names, types, self, foundGameObjects);
            }

            if (doSearchChildren || doSearchDescendants)
                searchChildren(self, doSearchDescendants, false, doSearchInactive, names, types, self, foundGameObjects);
        }

        return (foundGameObjects);
    }

    public static IEnumerable<GameObject> SearchHierarchy(this GameObject self, HierarchySearchType searchType, bool doSearchInactive, params string[] names)
    {
        return (SearchHierarchy(self, searchType, doSearchInactive, names.ToList(), null));
    }

    public static IEnumerable<T> SearchHierarchy<T>(this GameObject self, HierarchySearchType searchType, bool doSearchInactive)
    {
        return (SearchHierarchy(self, searchType, doSearchInactive, null, new List<Type>() { typeof(T) }).Select(m => m.GetComponent<T>()));
    }

    public static T SearchHierarchyForFirst<T>(this GameObject self, HierarchySearchType searchType, bool doSearchInactive)
    {
        return (SearchHierarchy(self, searchType, doSearchInactive, null, new List<Type>() { typeof(T) }).First().GetComponent<T>());
    }

    private static bool addGameObjectIfValid(GameObject foundGameObject, bool doSearchInactive, List<string> names, List<Type> types, GameObject self, List<GameObject> foundGameObjects)
    {
        if (!doSearchInactive && !foundGameObject.activeInHierarchy)
            return (false);

        bool gameObjectValid = true;

        if (foundGameObject == self)
        {
            gameObjectValid = false;
        }
        else if (foundGameObjects.Contains(foundGameObject))
        {
            gameObjectValid = false;
        }
        else
        {
            if ((names != null) && !names.Contains(foundGameObject.name))
            {
                gameObjectValid = false;
            }
            else if (types != null)
            {
                bool typeFound = false;

                foreach (Type type in types)
                {
                    if (foundGameObject.GetType() == type)
                    {
                        typeFound = true;
                        break;
                    }

                    if (foundGameObject.GetComponent(type) != null)
                    {
                        typeFound = true;
                        break;
                    }
                }

                if (!typeFound)
                    gameObjectValid = false;
            }
        }

        if (gameObjectValid)
            foundGameObjects.Add(foundGameObject);

        return (true);
    }

    private static void searchAncestors(GameObject parent, bool doSearchInactive, List<string> names, List<Type> types, GameObject self, List<GameObject> foundGameObjects)
    {
        if (parent.transform.parent != null)
        {
            GameObject grandParent = parent.transform.parent.gameObject;

            addGameObjectIfValid(grandParent, doSearchInactive, names, types, self, foundGameObjects);

            searchAncestors(grandParent, doSearchInactive, names, types, self, foundGameObjects);
        }
    }

    private static void searchChildren(GameObject parent, bool doSearchDescendants, bool doSearchAll, bool doSearchInactive, List<string> names, List<Type> types, GameObject self, List<GameObject> foundGameObjects)
    {
        for (int index = 0; index < parent.transform.childCount; index++)
        {
            GameObject child = parent.transform.GetChild(index).gameObject;

            if (addGameObjectIfValid(child, doSearchInactive, names, types, self, foundGameObjects))
            {
                if (doSearchDescendants)
                    searchChildren(child, true, doSearchAll, doSearchInactive, names, types, self, foundGameObjects);
            }
            else if (doSearchAll)
            {
                searchChildren(child, true, doSearchAll, doSearchInactive, names, types, self, foundGameObjects);
            }
        }
    }

    private static void searchRoots(bool doSearchNiblings, bool doSearchAll, bool doSearchInactive, List<string> names, List<Type> types, GameObject self, List<GameObject> foundGameObjects)
    {
        foreach (GameObject root in getRootGameObjects())
        {
            if (addGameObjectIfValid(root, doSearchInactive, names, types, self, foundGameObjects))
            {
                if (doSearchNiblings)
                    searchChildren(root, true, doSearchAll, doSearchInactive, names, types, self, foundGameObjects);
            }
            else if (doSearchAll)
            {
                searchChildren(root, true, doSearchAll, doSearchInactive, names, types, self, foundGameObjects);
            }
        }
    }

    private static List<GameObject> getRootGameObjects()
    {
        Scene activeScene = SceneManager.GetActiveScene();

        if (activeScene.isLoaded)
        {
            return (activeScene.GetRootGameObjects().ToList());
        }
        else
        {
            List<GameObject> rootGameObjects = new List<GameObject>();

            GameObject[] allGameObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
            foreach (GameObject possibleRootGameObject in allGameObjects)
            {
                if (possibleRootGameObject.transform.parent != null)
                    continue;

                if ((possibleRootGameObject.hideFlags == HideFlags.NotEditable) || (possibleRootGameObject.hideFlags == HideFlags.HideAndDontSave))
                    continue;

                if (string.IsNullOrEmpty(possibleRootGameObject.scene.name))
                    continue;

#if UNITY_EDITOR
                if (Application.isEditor)
                {
                    string assetPath = AssetDatabase.GetAssetPath(possibleRootGameObject.transform.root.gameObject);
                    if (!string.IsNullOrEmpty(assetPath))
                        continue;
                }
#endif

                rootGameObjects.Add(possibleRootGameObject);
            }

            return (rootGameObjects);
        }
    }

    #endregion
}
