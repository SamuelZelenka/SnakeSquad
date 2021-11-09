using UnityEngine;

public class SceneCamera : MonoBehaviour
{
    public static SceneCamera instance;
    private Camera _mainCamera;

    public static Camera MainCamera => instance._mainCamera;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _mainCamera = Camera.main;
    }

    public static Rect GetCameraBoundsInWorld()
    {
        Vector2 minPos = MainCamera.ScreenToWorldPoint(Vector2.zero);
        Vector2 size = (Vector2)MainCamera.ScreenToWorldPoint(new Vector2(MainCamera.pixelWidth, MainCamera.pixelHeight)) - minPos;
        return new Rect(minPos.x,minPos.y, size.x, size.y);
    }
}
