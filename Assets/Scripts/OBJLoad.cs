using Dummiesman;
using System.IO;
using System.Text;
using UnityEngine;
using B83.Image.BMP;

public class OBJLoad : MonoBehaviour
{
    private int count = 1;

    string objPath = string.Empty;
    string facePath = string.Empty;
    string uvPath = string.Empty;
    string imgPath = string.Empty;
    string piPath = string.Empty;
    string flagPath = string.Empty;

    GameObject loadedObject;
    Transform tf;
    MeshCollider mc;
    MeshFilter mf;
    Texture2D texture;
    Rigidbody rb;
    GameObject plane;
    public GameObject rightWeapon;
    public GameObject leftWeapon;
    public GameObject indicator;
    public bool Hit;

    private void Start()
    {
        // Kinect dependent Pathes
        //facePath = "D:\\SIGGRAPH\\Obj_Kinect_Opengl_Frame_human_deco\\glut\\personF.txt";
        //uvPath = "D:\\SIGGRAPH\\Obj_Kinect_Opengl_Frame_human_deco\\glut\\\\personT.txt";
        //imgPath = "D:\\SIGGRAPH\\Obj_Kinect_Opengl_Frame_human_deco\\glut\\person.bmp";
        //piPath = "D:\\SIGGRAPH\\Obj_Kinect_Opengl_Frame_human_deco\\glut\\personP.txt";

        // recorded Files Pathes
        facePath = "C:\\Obj_Kinect_Opengl_Predefined_Movement_Frame_ShoulderOK\\glut\\personF.txt";
        uvPath = "C:\\Obj_Kinect_Opengl_Predefined_Movement_Frame_ShoulderOK\\glut\\personT.txt";
        imgPath = "C:\\Obj_Kinect_Opengl_Predefined_Movement_Frame_ShoulderOK\\glut\\person.bmp";
        piPath = "C:\\Obj_Kinect_Opengl_Predefined_Movement_Frame_ShoulderOK\\glut\\personP.txt";

        // local Pathes
        //facePath = "C:\\Users\\parkj\\Desktop\\Object\\personF.txt";
        //uvPath = "C:\\Users\\parkj\\Desktop\\Object\\personT.txt";
        //imgPath = "C:\\Users\\parkj\\Desktop\\Object\\person.bmp";
        //piPath = "C:\\Users\\parkj\\Desktop\\Object\\personP.txt";

        LoadBMP(imgPath);
        plane = Instantiate(indicator, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        plane.name = "Character Indicator";
        Hit = false;
    }

    bool LoadFile()
    {
       if(File.Exists(objPath) && File.Exists(facePath) && File.Exists(uvPath) && File.Exists(piPath))
        //if (File.Exists(flagPath) && File.Exists(facePath) && File.Exists(uvPath) && File.Exists(piPath))
            {
            if (loadedObject != null)
            {
                Destroy(loadedObject);
            }
            Resources.UnloadUnusedAssets();
            loadedObject = new OBJLoader().Load(objPath, facePath, uvPath, piPath);
            SetupObject();
            Add_Weapon();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void FixedUpdate()
    {
        //objPath = "D:\\SIGGRAPH\\Obj_Kinect_Opengl_Frame_human_deco\\glut\\frame\\person" + count + ".obj";
        //flagPath = "D:\\SIGGRAPH\\Obj_Kinect_Opengl_Frame_human_deco\\glut\\frame\\flag" + count + ".txt";

        objPath = "C:\\Obj_Kinect_Opengl_Predefined_Movement_Frame_ShoulderOK\\glut\\frame\\person" + count + ".obj";
        flagPath = "C:\\Obj_Kinect_Opengl_Predefined_Movement_Frame_ShoulderOK\\glut\\frame\\flag" + count + ".txt";

        //objPath = "C:\\Users\\parkj\\Desktop\\Object\\frame\\person" + count + ".obj";
        if (LoadFile())
        {
            Debug.Log("Load Succeeded");
            StreamWriter textWrite = File.CreateText(flagPath);
            textWrite.Dispose();
            count++;
            if (count == 6)
                count = 1;
        }
        else
        {
            Debug.Log("Load failed");
        }
    }
    
    void Add_Weapon()
    {
        Transform rw = loadedObject.transform.GetChild(1);
        rightWeapon.transform.parent = loadedObject.transform;
        rightWeapon.transform.position = rw.transform.position;
        rightWeapon.transform.rotation = rw.transform.rotation;

        Transform lw = loadedObject.transform.GetChild(2);
        leftWeapon.transform.parent = loadedObject.transform;
        leftWeapon.transform.position = lw.transform.position;
        leftWeapon.transform.rotation = lw.transform.rotation;
    }

    // 불러온 오브젝트의 자식 오브젝트에 Mesh Collider를 추가하고 텍스처 입히기
    void SetupObject()
    {
        Debug.Log("Hit : " + Hit);
        tf = loadedObject.transform.GetChild(0);
        mc = tf.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        mf = tf.gameObject.GetComponent<MeshFilter>();
        if (!Hit)
        {
            rb = tf.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
            rb.useGravity = false;
        }
        tf.gameObject.tag = "Body1";
        mc.convex = true;
        mc.isTrigger = true;
        mc.sharedMesh = mf.sharedMesh;
        tf.GetComponent<Renderer>().material.mainTexture = texture;
        tf = loadedObject.transform.GetChild(3);
        plane.transform.position = tf.position;
    }

    // 텍스처 파일 .jpg 로드
    void LoadJPG(string path)
    {
        byte[] fileData;

        fileData = File.ReadAllBytes(path);
        texture = new Texture2D(1, 1);
        texture.LoadImage(fileData);
    }

    // 텍스처 파일 .bmp 로드
    void LoadBMP(string path)
    {
        BMPLoader loader = new BMPLoader();
        BMPImage bmpㅑmg = loader.LoadBMP(path);
        texture = bmpㅑmg.ToTexture2D();
    }
    
    public void ReLoad_Texture()
    {
        LoadBMP(imgPath);
    }

    public void UnLoadALL()
    {
        Resources.UnloadUnusedAssets();
    }

    public void characterHit()
    {
        Hit = true;
        Invoke("HittoFalse", 3.0f);
    }

    void HittoFalse()
    {
        Hit = false;
    }
}
