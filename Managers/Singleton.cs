using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            // get instance of singleton
            if (instance == null)
            {
                // 할당만 되어 있지 않은 경우 
                instance = (T)FindObjectOfType(typeof(T));

                // 생성된 오브젝트 자체가 없는 경우
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).ToString());
                    instance = singletonObject.AddComponent<T>();
                }
            }

            return instance;
        }
    }
}
