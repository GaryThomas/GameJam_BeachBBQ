using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static T m_instance;
    public static T Instance {
        get {
            if (m_instance == null) {
                m_instance = GameObject.FindObjectOfType<T>();
                if (m_instance == null) {
                    GameObject singleton = new GameObject(typeof(T).Name);
                    singleton.AddComponent<T>();
                }
            }
            return m_instance;
        }
    }

    public virtual void Awake() {
        Debug.Log("Singleton: " + typeof(T).Name);
        if (m_instance == null) {
            m_instance = this as T;
            // DontDestroyOnLoad(gameObject);
        } else if (m_instance != this) {
            Debug.Log("Singleton: destroy extra object");
            Destroy(gameObject);
        }
    }
}
