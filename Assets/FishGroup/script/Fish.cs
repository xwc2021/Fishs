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
    static float alignmentStrength = 1.0f;
    public Guide guide;
    public Renderer render;
    public Material materail;

    public Vector3 nowVelocity;
    public float nowVelocityStrength;
    Rigidbody rigid;
    // Use this for initialization
    void Awake () {
        rigid = GetComponent<Rigidbody>();
        materail = render.material;
    }

    Collider[] gs = new Collider[10];//大小看需求自己設定
    static float findingGravitySensorR = 5;

    public float cohesionStrengthScale;
    void calculusVelocity()
    {
        Vector3 cohesionVec = (guide.centerPos - transform.position);

        //離中心點遠就遊快一點
        float D = cohesionVec.magnitude;
        cohesionStrengthScale = D / cohesionRadius;

        if(cohesionStrengthScale>1)//離太遠就加速
            cohesionStrengthScale = Mathf.Pow(cohesionStrengthScale, 5);

        cohesionVec = cohesionVec * (cohesionStrengthScale / D);

        Vector3 alignment = guide.averageVeloctiy-nowVelocity;

        int layerMask = 1<<8;
        int overlapCount = Physics.OverlapSphereNonAlloc(transform.position, findingGravitySensorR, gs, layerMask);

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
                float seperateStrengthScale = ( 1.0f - d / seperateRadius)* guideStrength;//和guideStrength成正比
                seperateVec += seperateDiff * (seperateStrengthScale / d);
            }
            
        }

        Vector3 followTarget = guide.transform.position - transform.position;
        followTarget.Normalize();

        Vector3 randomVec = new Vector3(Random.value * 2 - 1, Random.value * 2 - 1, Random.value * 2 - 1);
        nowVelocity =
            randomVec                        //亂數
            + guideStrength* followTarget    //況目標前進
            //+ alignmentStrength*alignment   //感覺作用不大  
            + cohesionStrength * cohesionVec //集中
            + seperateStrength * seperateVec;//分開
 
        nowVelocityStrength = nowVelocity.magnitude;

        if (nowVelocityStrength > maxSpeed)
        {
            nowVelocity = nowVelocity * (maxSpeed / nowVelocityStrength);
            nowVelocityStrength = maxSpeed;
        }

        if (nowVelocityStrength < minSpeed)
        {
            nowVelocity = nowVelocity * (minSpeed / nowVelocityStrength);
            nowVelocityStrength = minSpeed;
        }

        //Debug.DrawLine(transform.position, transform.position + nowVelocity, Color.blue);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nowVelocity), rotaionSpeed*Time.fixedDeltaTime);
        rigid.velocity = nowVelocity;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        calculusVelocity();

    }
}
