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

    public void SetAll(string dataName, DistanceMetric metric, uint cardinality, bool isExpensive, bool shouldLoad)
    {
        this.dataName = dataName;
        this.distanceMetric = metric;
        this.cardinality = cardinality;
        this.isExpensive = isExpensive;
        this.shouldLoad = shouldLoad;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
