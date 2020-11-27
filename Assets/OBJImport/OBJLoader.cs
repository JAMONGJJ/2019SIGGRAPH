/*
 * Copyright (c) 2019 Dummiesman
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
*/

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using Dummiesman;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dummiesman
{
    public enum SplitMode {
        None,
        Object,
        Material
    }
    
    public class OBJLoader
    {
        bool isFaceRead = false;
        bool isUvRead = false;
        bool isPartRead = false;

        Transform tf;
        Mesh mesh;

        //options
        /// <summary>
        /// Determines how objects will be created
        /// </summary>
        public SplitMode SplitMode = SplitMode.Object;

        //global lists, accessed by objobjectbuilder
        internal List<Vector3> Vertices = new List<Vector3>();
        internal List<Vector3> Normals = new List<Vector3>();
        internal List<Vector2> UVs = new List<Vector2>();
        internal List<Vector2> Parts = new List<Vector2>();

        //materials, accessed by objobjectbuilder
        internal Dictionary<string, Material> Materials;

        //file info for files loaded from file path, used for GameObject naming and MTL finding
        private FileInfo _objInfo;

#if UNITY_EDITOR
        [MenuItem("GameObject/Import From OBJ")]
        static void ObjLoadMenu()
        {
            string pth = EditorUtility.OpenFilePanel("Import OBJ", "", "obj");
            if (!string.IsNullOrEmpty(pth))
            {
                System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
                s.Start();

                var loader = new OBJLoader
                {
                    SplitMode = SplitMode.Object,
                };
                loader.Load(pth, null, null, null);

                Debug.Log($"OBJ import time: {s.ElapsedMilliseconds}ms");
                s.Stop();
            }
        }
#endif

        /// <summary>
        /// Helper function to load mtllib statements
        /// </summary>
        /// <param name="mtlLibPath"></param>
        private void LoadMaterialLibrary(string mtlLibPath)
        {
            if (_objInfo != null)
            {
                if (File.Exists(Path.Combine(_objInfo.Directory.FullName, mtlLibPath)))
                {
                    Materials = new MTLLoader().Load(Path.Combine(_objInfo.Directory.FullName, mtlLibPath));
                    return;
                }
            }

            if (File.Exists(mtlLibPath))
            {
                Materials = new MTLLoader().Load(mtlLibPath);
                return;
            }
        }

        /// <summary>
        /// Load an OBJ file from a stream. No materials will be loaded, and will instead be supplemented by a blank white material.
        /// </summary>
        /// <param name="input">Input OBJ stream</param>
        /// <returns>Returns a GameObject represeting the OBJ file, with each imported object as a child.</returns>
        public GameObject Load(Stream input1, Stream input2, Stream input3, Stream input4)
        {
            var reader1 = new StreamReader(input1);
            var reader2 = new StreamReader(input2);
            var reader3 = new StreamReader(input3);
            var reader4 = new StreamReader(input4);
            //var reader = new StringReader(inputReader.ReadToEnd());

            Dictionary<string, OBJObjectBuilder> builderDict = new Dictionary<string, OBJObjectBuilder>();
            OBJObjectBuilder currentBuilder = null;
            string currentMaterial = "default";

            //lists for face data
            //prevents excess GC
            List<int> vertexIndices = new List<int>();
            List<int> normalIndices = new List<int>();
            List<int> uvIndices = new List<int>();

            //helper func
            Action<string> setCurrentObjectFunc = (string objectName) =>
            {
                if (!builderDict.TryGetValue(objectName, out currentBuilder))
                {
                    currentBuilder = new OBJObjectBuilder(objectName, this);
                    builderDict[objectName] = currentBuilder;
                }
            };

            //create default object
            setCurrentObjectFunc.Invoke("object");

			//var buffer = new DoubleBuffer(reader, 256 * 1024);
			var buffer1 = new CharWordReader(reader1, 4 * 1024);
            var buffer2 = new CharWordReader(reader2, 4 * 1024);
            var buffer3 = new CharWordReader(reader3, 4 * 1024);
            var buffer4 = new CharWordReader(reader4, 4 * 1024);
            
            //reading 'v' and others
            while (true)
            {
				buffer1.SkipWhitespaces();

				if (buffer1.endReached == true) {
					break;
				}

				buffer1.ReadUntilWhiteSpace();

                //comment or blank
                if (buffer1.Is("#"))
                {
					buffer1.SkipUntilNewLine();
                    continue;
                }
				
                if (buffer1.Is("v")) {
					Vertices.Add(buffer1.ReadVector());
					continue;
				}
                
				buffer1.SkipUntilNewLine();
            }
            
            // reading 'vt'
            while (!isUvRead)
            {
                buffer3.SkipWhitespaces();

                if (buffer3.endReached == true)
                {
                    isUvRead = true;
                    break;
                }

                buffer3.ReadUntilWhiteSpace();

                //comment or blank
                if (buffer3.Is("#"))
                {
                    buffer3.SkipUntilNewLine();
                    continue;
                }

                //uv
                if (buffer3.Is("vt"))
                {
                    UVs.Add(buffer3.ReadVector());
                    continue;
                }

                buffer3.SkipUntilNewLine();
            }

            //reading 'f'
            while (!isFaceRead)
            {
                buffer2.SkipWhitespaces();

                if (buffer2.endReached == true)
                {
                    isFaceRead = true;
                    break;
                }

                buffer2.ReadUntilWhiteSpace();

                //comment or blank
                if (buffer2.Is("#"))
                {
                    buffer2.SkipUntilNewLine();
                    continue;
                }

                //face data (the fun part)
                if (buffer2.Is("f"))
                {
                    //loop through indices
                    while (true)
                    {
                        bool newLinePassed;
                        buffer2.SkipWhitespaces(out newLinePassed);
                        if (newLinePassed == true)
                        {
                            break;
                        }

                        int vertexIndex = int.MinValue;
                        int normalIndex = int.MinValue;
                        int uvIndex = int.MinValue;

                        vertexIndex = buffer2.ReadInt();
                        normalIndex = uvIndex = vertexIndex;
                        /*if (buffer2.currentChar == '/')
                        {
                            buffer2.MoveNext();
                            if (buffer2.currentChar != '/')
                            {
                                uvIndex = buffer2.ReadInt();
                            }
                            if (buffer2.currentChar == '/')
                            {
                                buffer2.MoveNext();
                                normalIndex = buffer2.ReadInt();
                            }
                        }*/

                        //"postprocess" indices
                        if (vertexIndex > int.MinValue)
                        {
                            if (vertexIndex < 0)
                                vertexIndex = Vertices.Count - vertexIndex;
                            vertexIndex--;
                        }
                        if (normalIndex > int.MinValue)
                        {
                            if (normalIndex < 0)
                                normalIndex = Normals.Count - normalIndex;
                            normalIndex--;
                        }
                        if (uvIndex > int.MinValue)
                        {
                            if (uvIndex < 0)
                                uvIndex = UVs.Count - uvIndex;
                            uvIndex--;
                        }

                        //set array values
                        vertexIndices.Add(vertexIndex);
                        normalIndices.Add(normalIndex);
                        uvIndices.Add(uvIndex);
                    }

                    //push to builder
                    currentBuilder.PushFace(currentMaterial, vertexIndices, normalIndices, uvIndices);

                    //clear lists
                    vertexIndices.Clear();
                    normalIndices.Clear();
                    uvIndices.Clear();

                    continue;
                }

                buffer2.SkipUntilNewLine();
            }

            // reading 'pi"
            while (!isPartRead)
            {
                buffer4.SkipWhitespaces();

                if(buffer4.endReached == true)
                {
                    isPartRead = true;
                    break;
                }

                buffer4.ReadUntilWhiteSpace();

                //comment or blank
                if (buffer4.Is("#"))
                {
                    buffer4.SkipUntilNewLine();
                    continue;
                }

                //uv
                if (buffer4.Is("pi"))
                {
                    Parts.Add(buffer4.ReadVector());
                    continue;
                }

                buffer4.SkipUntilNewLine();
            }

            //finally, put it all together
            GameObject obj = new GameObject("person");
            obj.transform.localScale = new Vector3(-1f, 1f, 1f);

            foreach (var builder in builderDict)
            {
                //empty object
                if (builder.Value.PushedFaceCount == 0)
                    continue;

                var builtObj = builder.Value.Build();
                builtObj.transform.SetParent(obj.transform, false);
            }

            tf = obj.transform.GetChild(0);
            mesh = tf.GetComponent<MeshFilter>().mesh;
            mesh.RecalculateNormals();

            //foreach (var vertex in Vertices)
            //{
            //    mesh.vertices[num].x = vertex.x;
            //    mesh.vertices[num].y = vertex.y;
            //    mesh.vertices[num].z = vertex.z;
            //    num++;
            //}

            //if (!isFaceMapped)
            //{
            //    num = 0;
            //    foreach (var index in vertexIndices)
            //    {
            //        mesh.triangles[num] = index;
            //        num++;
            //    }
            //    isFaceMapped = true;
            //}

            //if (!isUvMapped)
            //{
            //    num = 0;
            //    foreach (var uv in UVs)
            //    {
            //        mesh.uv[num].x = uv.x;
            //        mesh.uv[num].y = uv.y;
            //        num++;
            //    }
            //    isUvMapped = true;
            //}

            reader1.Close();
            reader2.Close();
            reader3.Close();
            reader4.Close();
            Find_Hand(obj);
            Find_Center(obj);

            return obj;
        }
        
        /// <summary>
        /// Load an OBJ and MTL file from a file path.
        /// </summary>
        /// <param name="path">Input OBJ path</param>
        /// /// <param name="mtlPath">Input MTL path</param>
        /// <returns>Returns a GameObject represeting the OBJ file, with each imported object as a child.</returns>
        public GameObject Load(string vertexPath, string facePath, string uvPath, string partsPath, string mtlPath)
        {
            GameObject obj;
            _objInfo = new FileInfo(vertexPath);
            if (!string.IsNullOrEmpty(mtlPath) && File.Exists(mtlPath))
            {
                var mtlLoader = new MTLLoader();
                Materials = mtlLoader.Load(mtlPath);

                using (var fs1 = new FileStream(vertexPath, FileMode.Open))
                {
                    return Load(fs1, null, null, null);
                }
            }
            else
            {
                using (var fs1 = new FileStream(vertexPath, FileMode.Open))
                {
                    using (var fs2 = new FileStream(facePath, FileMode.Open))
                    {
                        using (var fs3 = new FileStream(uvPath, FileMode.Open))
                        {
                            using (var fs4 = new FileStream(partsPath, FileMode.Open))
                            {
                                obj = Load(fs1, fs2, fs3, fs4);
                                fs1.Close();
                                fs2.Close();
                                fs3.Close();
                                fs4.Close();
                                return obj;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Load an OBJ file from a file path. This function will also attempt to load the MTL defined in the OBJ file.
        /// </summary>
        /// <param name="path">Input OBJ path</param>
        /// <returns>Returns a GameObject represeting the OBJ file, with each imported object as a child.</returns>
        public GameObject Load(string vertexPath, string facePath, string uvPath, string partsPath)
        {
            return Load(vertexPath, facePath, uvPath, partsPath, null);
        }

        public GameObject Add_Collider(GameObject obj, int index)
        {
            Vector3 pos = new Vector3();
            string collider_name = string.Empty;
            BoxCollider bc;
            int count = 0;
            float max_x = -10000.0f, min_x = 10000.0f;
            float max_y = -10000.0f, min_y = 10000.0f;
            float max_z = -10000.0f, min_z = 10000.0f;

            // Parts 리스트에서 index값에 맞는 vertex들의 좌표들의 중심점을 pos에 저장
            foreach (var part in Parts)
            {
                if (part.y == index)
                {
                    if (Vertices[(int)part.x - 1].x > max_x)
                        max_x = Vertices[(int)part.x - 1].x;
                    if (Vertices[(int)part.x - 1].x < min_x)
                        min_x = Vertices[(int)part.x - 1].x;

                    if (Vertices[(int)part.x - 1].y > max_y)
                        max_y = Vertices[(int)part.x - 1].y;
                    if (Vertices[(int)part.x - 1].y < min_y)
                        min_y = Vertices[(int)part.x - 1].y;

                    if (Vertices[(int)part.x - 1].z > max_z)
                        max_z = Vertices[(int)part.x - 1].z;
                    if (Vertices[(int)part.x - 1].z < min_z)
                        min_z = Vertices[(int)part.x - 1].z;
                    count++;
                }
            }
            pos.x = (max_x + min_x) / 2.0f;
            pos.y = (max_y + min_y) / 2.0f;
            pos.z = (max_z + min_z) / 2.0f;

            // 콜라이더 이름 정하기
            switch (index)
           {
                case 0: // 왼발에 콜라이더
                    collider_name = "leftFoot_col"; break;
                case 1: // 오른발에 콜라이더
                    collider_name = "rightFoot_col"; break;
                case 4: // 왼손에 콜라이더
                    collider_name = "leftHand_col"; break;
                case 5: // 오른손에 콜라이더
                    collider_name = "rightHand_col"; break;
            }

            // gameObject를 하나 만들어서 mesh collider 추가 후 obj의 자식 오브젝트로 추가한 후 반환
            GameObject child = new GameObject(collider_name);
            child.transform.tag = "Hit1";
            child.transform.position = pos;
            bc = child.AddComponent(typeof(BoxCollider)) as BoxCollider;
            bc.isTrigger = true;
            child.transform.parent = obj.transform;

            return obj;
        }

        public GameObject Find_Hand(GameObject obj)
        {

            int index;
            Vector3 leftMax, leftMin, rightMax, rightMin;
            leftMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            leftMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            rightMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            rightMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            foreach (var part in Parts)
            {
                if(part.y == 4)
                {
                    index = (int)part.x - 1;
                    if (Vertices[index].z > leftMax.z)
                    {
                        leftMax = Vertices[index];
                    }
                    if(Vertices[index].z < leftMin.z)
                    {
                        leftMin = Vertices[index];
                    }
                }
                else if(part.y == 5)
                {
                    index = (int)part.x - 1;
                    if (Vertices[index].z > rightMax.z)
                    {
                        rightMax = Vertices[index];
                    }
                    if (Vertices[index].z < rightMin.z)
                    {
                        rightMin = Vertices[index];
                    }
                }
            }

            Vector3 rightCenter = (rightMax + rightMin) / 2;
            Vector3 leftCenter = (leftMax + leftMin) / 2;
            rightCenter.x = -rightCenter.x;
            leftCenter.x = -leftCenter.x;

            Vector3 rightAngle = rightMax - rightMin;
            Vector3 leftAngle = leftMax - leftMin;
            
            GameObject rightWeapon = new GameObject("rightArm");
            rightWeapon.transform.position = rightCenter;
            rightWeapon.transform.Rotate(rightAngle);
            rightWeapon.transform.parent = obj.transform;

            GameObject leftWeapon = new GameObject("leftArm");
            leftWeapon.transform.position = leftCenter;
            leftWeapon.transform.Rotate(leftAngle);
            leftWeapon.transform.parent = obj.transform;

            return obj;
        }

        public GameObject Find_Center(GameObject obj)
        {
            Vector3 Center = Vector3.zero;
            int count = 0;

            foreach(var vertex in Vertices)
            {
                Center += vertex;
                count++;
            }
            Center /= count;
            Center.x = -Center.x;

            GameObject child = new GameObject("Center");
            child.transform.position = Center;
            child.transform.parent = obj.transform;
            return obj;
        }
    }
}