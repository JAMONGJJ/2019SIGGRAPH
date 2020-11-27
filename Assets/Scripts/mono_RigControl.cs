using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.IO;

public class mono_RigControl : MonoBehaviour
{

    int jointCount = 25;


    public bool mirror = false;


    public List<float> aset = new List<float>();

    public int count;
    int rdcount;
    int frame;


    int num_frame;
    int interpolation_frame = 3;

    public bool doing_action = true;
    float init_x, init_y, init_z;

    string[] xyz = new string[3];
    string[] next_xyz = new string[3];
    double[,] ret;

    //int total_file_cnt;
    int filecount = 1;
    string filename = "";
    string next_filename = "";

    Vector3[] criteria;

    float y = 0.1f;

    CharacterSkeleton skeleton;

    // 이코드를 갖고있는 object들한테만 적용되는 코드.
    void Start()
    {
        //kinect.getKinect();

        ret = Blur.CalculateNormalized1DSampleKernel(1);//0.054 0.244 0.4026
        num_frame = 0;
        frame = 0;
        count = 0;

        doing_action = true;
        mirror = false;

        skeleton = new CharacterSkeleton(gameObject);
        //    total_file_cnt = Directory.GetFiles(Path.Combine(@"C:\Obj_Kinect_Opengl_Predefined_Movement_Frame_ShoulderOK\glut\", gameObject.transform.name.Split('_')[0]), "*.txt").Length;
        //total_file_cnt = Directory.GetFiles(Path.Combine(@"C:\Users\User\Documents\New Unity Project\Assets\data\", gameObject.transform.name.Split('_')[0]), "*.txt").Length;
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.position = init_position;
        if (doing_action)
        {
            System.Random rd = new System.Random();
            filecount = rd.Next(1, 100);


            //파일 선택
            if (filecount < 9)
            {
                filename = "0" + filecount.ToString();
                next_filename = "0" + (filecount + 1).ToString();
            }
            else if (filecount == 9)
            {
                filename = "0" + filecount.ToString();
                next_filename = "10";
            }
            else
            {
                filename = filecount.ToString();
                next_filename = filecount.ToString();
            }



            one_action("cube(" + filename + ")");//throw 7



            doing_action = false;
            rdcount = rd.Next(5, 8);
            count = rdcount;
        }
        else
        {
            if (count == rdcount && frame < num_frame)//num_frame
            {
                Debug.Log("Update is called");
                float[] data = new float[jointCount * 3]; // 25*3


                count = 0;

                for (int i = 0; i < 25 * 3; i++)
                {

                    if (2 < frame && frame < num_frame - 2)
                    {
                        data[i] = aset[25 * 3 * frame + i] * (float)ret[2, 0] + (aset[25 * 3 * (frame - 1) + i] + aset[25 * 3 * (frame + 1) + i]) * (float)ret[1, 0] + (aset[25 * 3 * (frame - 2) + i] + aset[25 * 3 * (frame + 2) + i]) * (float)ret[0, 0];
                    }
                    else
                    {
                        data[i] = aset[25 * 3 * frame + i];
                    }
                    //Debug.Log(data[i]);
                }

                //criteria = skeleton.set(data, mirror, frame, criteria);
                criteria = skeleton.set(data, mirror, frame, criteria);

                //Debug.Log(frame);




                Array.Clear(data, 0, jointCount * 3);
                frame++;

            }

            if (frame % num_frame == 0) // % (큐브 하나의 총 프레임 수)
            {
                doing_action = true;
            }
            count++;
        }

    }


    public void one_action(string filename)
    {
        Debug.Log("one action is called");
        // doing_action = true;

        doing_action = false;
        string objname = gameObject.transform.name.Split('_')[0];
        filename += ".txt";

        string path = Path.Combine(@"C:\Obj_Kinect_Opengl_Predefined_Movement_Frame_ShoulderOK\glut\", objname, filename);
        //string path = Path.Combine(@"C:\Users\user\Downloads\game\Assets\data\", objname, filename);
        string[] line = File.ReadAllLines(path);


        int count = 0;
        foreach (string show in line)
        {
            xyz = show.Split(' ');

            aset.Add(Convert.ToSingle(Math.Truncate((float.Parse(xyz[1])) * 10000.0) / 10000.0));
            aset.Add(Convert.ToSingle(Math.Truncate((float.Parse(xyz[2])) * 10000.0) / 10000.0));
            aset.Add(Convert.ToSingle(Math.Truncate((float.Parse(xyz[0])) * 10000.0) / 10000.0));

            count++;
        }

        num_frame += count / 25;
        //Debug.Log(num_frame);

    }
}