using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : MonoBehaviour {

    public Vector3 centerPos;
    public int count=10;
    public Fish fishPrefab;

    public Bounds bounds;
    public float minR=2;
    public float maxR = 10;

    public Color[] colorList;

    Vector3[] newVelocity;
    public List<Fish> list;
    // Use this for initialization
    Vector3 initPos;
    void Awake () {

        newVelocity = new Vector3[count];

        list = new List<Fish>();
        for(int i = 0; i < count; i++){
            Fish fish =GameObject.Instantiate<Fish>(fishPrefab);
            fish.transform.position = transform.position+Random.Range(minR, maxR) *Random.onUnitSphere;
            fish.transform.rotation = transform.rotation;
            fish.guide = this;
            fish.name = i.ToString();

            int index = (int)Random.Range(0, colorList.Length);
            fish.materail.color = colorList[index];

            list.Add(fish);
        }

        initPos = transform.position;

        StartCoroutine("WaitAndChangePos", 8.0F);
    }

    float sign = 1;
    private IEnumerator WaitAndChangePos(float waitTime)
    {
        while (true)
        {
            transform.position = initPos + sign*transform.forward * 500;
            sign = sign * -1;
            yield return new WaitForSeconds(waitTime);
        }
    }


    // Update is called once per frame
    void FixedUpdate () {

        Vector3 tempCeterPos = Vector3.zero;
        for(int i = 0; i < count; i++)
        {
            Fish fish = list[i];
            tempCeterPos += fish.transform.position;
        }
        centerPos = tempCeterPos / count;

        
        //其實和「直接在Fish的FixedUPdate裡更新」沒什麼差別也
        //重新計算
        for (int i = 0; i < count; i++)
        {
            Fish fish = list[i];
            newVelocity[i] = fish.calculusVelocity();
        }

        //更新
        for (int i = 0; i < count; i++)
        {
            Fish fish = list[i];
            fish.setVelocity(newVelocity[i]);
        }
    }
}
