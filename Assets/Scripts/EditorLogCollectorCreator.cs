using System.Collections;
using System.Collections.Generic;
using Ubiq.Logging;
using Ubiq;
using UnityEngine;

public class EditorLogCollectorCreator : MonoBehaviour
{
    private LogCollector collector;

    private void Awake() 
    {
        DontDestroyOnLoad(this.gameObject);
//        Ubiq.Extensions.MonoBehaviourExtensions.DontDestroyOnLoadGameObjects.Add(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        if(Application.isEditor)
        {
            collector = gameObject.AddComponent<LogCollector>() as LogCollector;
            StartCoroutine(StartLogCollection());
        }
    }

    IEnumerator StartLogCollection() {
        yield return 0;
        collector.StartCollection();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
