using UnityEngine;

public static class Singleton
{
    private static StateController _state;
    private static Camera _mainCamera;

    #region Singletons

    public static StateController State
    {
        get
        {
            if (_state == null)
            {
                Debug.Log("ERROR | Singleton: State not yet provided");
                Debug.Break();
            }

            return (_state);
        }
    }

    public static Camera MainCamera
    {
        get
        {
            if (_mainCamera == null)
            {
                Debug.Log("ERROR | Singleton: Main camera not yet provided");
                Debug.Break();
            }

            return (_mainCamera);
        }
    }

    #endregion

    #region Provide Routines

    public static void ProvideState(StateController state)
    {
        if (_state == null)
        {
            _state = state;
        }
        else
        {
            Debug.Log("ERROR | Singleton: State already provided");
            Debug.Break();
        }
    }

    public static void ProvideMainCamera(Camera mainCamera)
    {
        if (_mainCamera == null)
        {
            _mainCamera = mainCamera;
        }
        else
        {
            Debug.Log("ERROR | Singleton: Main camera already provided");
            Debug.Break();
        }
    }

    #endregion
}
