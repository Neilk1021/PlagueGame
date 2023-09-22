using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;
using UnityEngine.UI;

public class LevelDisplay : MonoBehaviour
{
    BaseStats baseStats;
    // Start is called before the first frame update
    void Awake()
    {
        baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Text>().text = string.Format("{0}", baseStats.GetLevel());
    }
}
