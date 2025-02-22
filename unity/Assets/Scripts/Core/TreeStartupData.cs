using Clam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TreeStartupData", order = 1)]
public class TreeStartupData : ScriptableObject
{

    public string dataName;
    public DistanceMetric distanceMetric;
    public uint cardinality;
    public bool isExpensive;
    public bool shouldLoad;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
