using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class CharacterSkeleton
{
    public GameObject pointer;
    public GameObject light;

    Dictionary<string, Color> pointercolor = new Dictionary<string, Color>() {
        {"clap", Color.red },
        {"walk", Color.green },
        {"throw", Color.blue },
        {"kick", Color.yellow },
        {"dance", Color.white },
        {"salute", Color.black },
        {"unitychan",Color.red}
    };



    public const int

      // number of clsss
      Classnum = 6,

      // JointType
      JointType_SpineBase = 0,
      JointType_SpineMid = 1,
      JointType_Neck = 2,
      JointType_Head = 3,
      JointType_ShoulderLeft = 4,
      JointType_ElbowLeft = 5,
      JointType_WristLeft = 6,
      JointType_HandLeft = 7,
      JointType_ShoulderRight = 8,
      JointType_ElbowRight = 9,
      JointType_WristRight = 10,
      JointType_HandRight = 11,
      JointType_HipLeft = 12,
      JointType_KneeLeft = 13,
      JointType_AnkleLeft = 14,
      JointType_FootLeft = 15,
      JointType_HipRight = 16,
      JointType_KneeRight = 17,
      JointType_AnkleRight = 18,
      JointType_FootRight = 19,
      JointType_SpineShoulder = 20,
      JointType_HandTipLeft = 21,
      JointType_ThumbLeft = 22,
      JointType_HandTipRight = 23,
      JointType_ThumbRight = 24,
      // TrackingState
      TrackingState_NotTracked = 0,
      TrackingState_Inferred = 1,
      TrackingState_Tracked = 2,
      // Number
      jointCount = 25;



    private static int[] jointSegment = new int[] {
//16 19

    //JointType_SpineBase, JointType_SpineBase,
    JointType_SpineBase, JointType_SpineMid,             // Spine
    JointType_SpineMid, JointType_SpineShoulder,         //  추가 upperchest
    //JointType_SpineShoulder, JointType_Neck,             // 추가 neck
    //JointType_Neck, JointType_Head,                      // Head
   
    // right
    //JointType_SpineShoulder, JointType_ShoulderRight,//추가
    JointType_ShoulderRight, JointType_ElbowRight,       // RightUpperArm
    JointType_ElbowRight, JointType_WristRight,          // RightLowerArm
    //JointType_WristRight, JointType_HandRight,           // RightHand


    //JointType_SpineMid, JointType_HipRight,              // 
    JointType_HipRight, JointType_KneeRight,             // 
    JointType_KneeRight, JointType_AnkleRight,           // 
    //JointType_AnkleRight, JointType_FootRight,           // 

     // left
    //JointType_SpineShoulder, JointType_ShoulderLeft,//추가
    JointType_ShoulderLeft, JointType_ElbowLeft,         // LeftUpperArm
    JointType_ElbowLeft, JointType_WristLeft,            // LeftLowerArm
   // JointType_WristLeft, JointType_HandLeft,             // LeftHand


    //JointType_SpineMid, JointType_HipLeft,               // 
    JointType_HipLeft, JointType_KneeLeft,               //         
    JointType_KneeLeft, JointType_AnkleLeft,             // 
    //JointType_AnkleLeft, JointType_FootLeft,             // 

  };

    public Vector3[] joint;
    public int[] jointState;

    Dictionary<HumanBodyBones, Vector3> trackingSegment = null;
    Dictionary<HumanBodyBones, int> trackingState = null;



    int[] humanJoint = { 0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
    private static HumanBodyBones[] humanBone = new HumanBodyBones[] {
//19개

    HumanBodyBones.Hips,
    HumanBodyBones.Spine,
    HumanBodyBones.Neck,
    HumanBodyBones.Head,

    //HumanBodyBones.LeftShoulder,//
    HumanBodyBones.LeftUpperArm,
    HumanBodyBones.LeftLowerArm,
    HumanBodyBones.LeftHand,
    //HumanBodyBones.LeftMiddleDistal,//7

    //HumanBodyBones.RightShoulder,//
    HumanBodyBones.RightUpperArm,
    HumanBodyBones.RightLowerArm,
    HumanBodyBones.RightHand,
    //HumanBodyBones.RightMiddleDistal,//11

    HumanBodyBones.LeftUpperLeg,
    HumanBodyBones.LeftLowerLeg,
    HumanBodyBones.LeftFoot,
    HumanBodyBones.LeftToes,//15

    HumanBodyBones.RightUpperLeg,
    HumanBodyBones.RightLowerLeg,
    HumanBodyBones.RightFoot,
    HumanBodyBones.RightToes,//19

    HumanBodyBones.UpperChest,//20

    //HumanBodyBones.LeftMiddleDistal,
    //HumanBodyBones.LeftThumbDistal,//23

    ///HumanBodyBones.RightMiddleDistal,
    //HumanBodyBones.RightThumbDistal,//25
  };

    private static HumanBodyBones[] targetBone = new HumanBodyBones[] {

    //HumanBodyBones.Hips,    
    HumanBodyBones.Spine,
    //HumanBodyBones.Chest,
    HumanBodyBones.UpperChest,//
    //HumanBodyBones.Neck,
    //HumanBodyBones.Head,

    //  HumanBodyBones.RightShoulder,
    HumanBodyBones.RightUpperArm,
    HumanBodyBones.RightLowerArm,
    //HumanBodyBones.RightHand,//손목
    //HumanBodyBones.RightThumbProximal,

    HumanBodyBones.RightUpperLeg,
    HumanBodyBones.RightLowerLeg,
    //HumanBodyBones.RightFoot,//발목
    //HumanBodyBones.RightToes,

//    HumanBodyBones.LeftShoulder,
    HumanBodyBones.LeftUpperArm,
    HumanBodyBones.LeftLowerArm,
    //HumanBodyBones.LeftHand,//손목
    //HumanBodyBones.LeftThumbProximal,

    HumanBodyBones.LeftUpperLeg,
    HumanBodyBones.LeftLowerLeg,
    //HumanBodyBones.LeftFoot,//발목
    //HumanBodyBones.LeftToes,


  };

    public GameObject humanoid;
    private Dictionary<HumanBodyBones, RigBone> rigBone = null;


    public int ran = UnityEngine.Random.Range(-18, 18);

    Vector3[] criteriaVec;

    public CharacterSkeleton(GameObject h)
    {
        humanoid = h;

        pointer = GameObject.Find(humanoid.name + "_pointer");
        rigBone = new Dictionary<HumanBodyBones, RigBone>();

        joint = new Vector3[jointCount];
        jointState = new int[jointCount];

        foreach (HumanBodyBones bone in humanBone)
        {
            rigBone[bone] = new RigBone(humanoid, bone);
        }


        trackingSegment = new Dictionary<HumanBodyBones, Vector3>(targetBone.Length);
        trackingState = new Dictionary<HumanBodyBones, int>(targetBone.Length);

        criteriaVec = new Vector3[19];
    }

    public Vector3[] set(float[] jt, bool mirrored, int frame, Vector3[] criteria)
    {

        for (int i = 0; i < jointCount; i++)
        {
            int j = i;//0 + i
                      //if (jt[j * 3])
            joint[i] = new Vector3(-jt[j * 3], jt[j * 3 + 1], -jt[j * 3 + 2]);


        }

        for (int i = 0; i < targetBone.Length; i++)
        {
            int s = jointSegment[2 * i], e = jointSegment[2 * i + 1];
            trackingSegment[targetBone[i]] = joint[e] - joint[s];
        }



        humanoid.transform.rotation = Quaternion.identity;//초기화

        rigBone[HumanBodyBones.Hips].transform.rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.left);




        rigBone[HumanBodyBones.Chest] = new RigBone(humanoid, HumanBodyBones.Chest);
        // rigBone[HumanBodyBones.Chest].transform.rotation = Quaternion.FromToRotation(Vector3.left, joint[20] - (joint[20] + joint[1]) / 2.0f); //upperchest 좌표 - (upperchest + spinemid) 중간 좌표


        foreach (HumanBodyBones bone in targetBone)//targetBone
        {
            // 로테이션
            rigBone[bone].transform.rotation = Quaternion.FromToRotation(Vector3.left, trackingSegment[bone]);

        }


        humanoid.transform.rotation = Quaternion.Euler(90, 180, 0);



        //위치 랜덤 속도로 이동
        int speed = Random.Range(5, 30);
        //y = 0 고정
        humanoid.transform.position += new Vector3(Random.Range(-90, 90), 0, Random.Range(-90, 90)) * Time.deltaTime * speed;

        //스크린 내 이동범위 제한
        Vector3 pos = Camera.main.WorldToViewportPoint(humanoid.transform.position);
        pos.x = Mathf.Clamp(pos.x, 0.2f, 0.8f);
        pos.y = Mathf.Clamp(pos.y, 0.2f, 0.8f);
        humanoid.transform.position = Camera.main.ViewportToWorldPoint(pos);




        return criteriaVec;


    }
}