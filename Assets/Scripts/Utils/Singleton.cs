using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                T[] instanceArray = FindObjectsByType<T>(FindObjectsSortMode.None);

                if (instanceArray.Length > 0)
                {
                    instance = instanceArray[0];
                }

                if (instanceArray.Length > 1)
                {
                    Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                }

                if (instance == null)
                {
                    GameObject obj = new()
                    {
                        name = string.Format("_{0}", typeof(T).Name)
                    };
                    instance = obj.AddComponent<T>();
                }
            }

            return instance;
        }
    }

    private static T instance;
}
