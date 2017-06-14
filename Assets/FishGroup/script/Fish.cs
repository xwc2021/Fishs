using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour {

    static float rotaionSpeed = 5.0f;

    static float maxSpeed = 50.0f;
    static float minSpeed = 2.0f;

    static float cohesionRadius = 10.0f;
    static float cohesionStrength = 1.0f;

    static float seperateRadius = 5.0f;
    static float seperateStrength = 5.0f;

    static float guideStrength = 10;
    public Guide guide;
    public Renderer render;
    public Material materail;

    public Vector3 nowVelocity;
    Rigidbody rigid;
    // Use this for initialization
    void Awake () {
        rigid = GetComponent<Rigidbody>();
        materail = render.material;
    }

    Collider[] gs = new Collider[10];//大小看需求自己設定
    static float nearRadius = 5;//調整這個值的大小，會出現不同的行為，因為沒辦法預期取得的gs是那些

    Vector3 getCohesionVec()
    {
        Vector3 cohesionVec = (guide.centerPos - transform.position);

        //離中心點遠就遊快一點
        float d = cohesionVec.magnitude;
        float cohesionStrengthScale = d / cohesionRadius;

        if (cohesionStrengthScale > 1)//離太遠就加速
            cohesionStrengthScale = Mathf.Pow(cohesionStrengthScale, 5);

        cohesionVec = cohesionVec * (cohesionStrengthScale / d);
        return cohesionVec;
    }

    Vector3 getSeperateVec()
    {

        int layerMask = 1 << 8;
        int overlapCount = Physics.OverlapSphereNonAlloc(transform.position, nearRadius, gs, layerMask);

        Vector3 seperateVec = Vector3.zero;
        for (int i = 0; i < overlapCount; i++)
        {
            Transform neighborTransfrom = gs[i].transform;
            if (neighborTransfrom == transform)
                continue;


            Vector3 neighborPos = neighborTransfrom.position;
            Vector3 seperateDiff = transform.position - neighborPos;
            float d = seperateDiff.magnitude;
            if (d < seperateRadius)
            {
                //離中心愈近，strengthScale愈強
                float seperateStrengthScale = (1.0f - d / seperateRadius) * guideStrength;//和guideStrength成正比
                seperateVec += seperateDiff * (seperateStrengthScale / d);
            }

        }

        return seperateVec;
    }

    Vector3 geGuideVec()
    {
        Vector3 guideVec = guide.transform.position - transform.position;
        return guideVec.normalized;
    }

    public Vector3 calculusVelocity()
    {
        
        Vector3 randomVec = new Vector3(Random.value * 2 - 1, Random.value * 2 - 1, Random.value * 2 - 1);

        Vector3 newVelocity =
            randomVec                        //亂數
            + guideStrength * geGuideVec()    //況目標前進
            + cohesionStrength * getCohesionVec() //集中
            + seperateStrength * getSeperateVec();//分開


        float newVelocityStrength = newVelocity.magnitude;

        if (newVelocityStrength > maxSpeed)
        {
            newVelocity = newVelocity * (maxSpeed / newVelocityStrength);
            newVelocityStrength = maxSpeed;
        }

        if (newVelocityStrength < minSpeed)
        {
            newVelocity = newVelocity * (minSpeed / newVelocityStrength);
            newVelocityStrength = minSpeed;
        }

        return newVelocity;
    }

    public void setVelocity(Vector3 v)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(v), rotaionSpeed * Time.fixedDeltaTime);
        nowVelocity = v;
        rigid.velocity = v;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //Debug.DrawLine(transform.position, transform.position + nowVelocity, Color.blue);
    }
}
