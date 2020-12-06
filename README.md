# 2019 SIGGRAPH
###### <키넥트를 통해 자신이 그린 그림을 직접 움직이는 컴퓨터 비전 프로젝트>와, <3D 캐릭터에게 정해진 몇몇 행동을 학습시키고 캐릭터가 해당 행동들을 얼마나 잘 블렌딩해서 자연스럽게 수행하는지를 보여주는 딥러닝 프로젝트>를 효과적으로 시연할 수 있게 해주는 게임 클라이언트 제작
###### 게임의 플레이어는 1) 자신의 아바타를 그려 이미지 파일로 저장하고, 2) 알고리즘을 통해 이미지를 기반으로 머리, 가슴, 팔, 다리 등 인체의 여러 부분을 나눠 인식해, 2) 키넥트 앞에서 원하는 행동을 하여 이에 상응하는 아바타의 신체 부위를 조종하여 3) 학습된 행동을 하는 캐릭터들을 터치해서 모두 없애면 게임 승리
###### 위의 두 프로젝트와 해당 게임 클라이언트는 모두 SIGGRAPH 2019 학회 전시회에서 전시를 진행함

----
#### 사용언어 및 툴 : C#, Unity
#### 작업 인원 : 3인 프로젝트(프로그래머 3명)

----
## < 작업 내용 >
> ###### * 키넥트를 통해 촬영된 플레이어의 몸짓을 기반으로 움직이는 아바타의 obj파일을 폴더에 저장하고, 게임 클라이언트에서 해당 폴더의 obj파일들을 OBJLoader Asset을 이용해 짧은 시간차를 두고 매프레임 읽어서 플레이어가 움직이는대로 게임 속 플레이어의 캐릭터가 움직이게끔 설계
        facePath = "D:\\SIGGRAPH\\Obj_Kinect_Opengl_Frame_human_deco\\glut\\personF.txt";
        uvPath = "D:\\SIGGRAPH\\Obj_Kinect_Opengl_Frame_human_deco\\glut\\\\personT.txt";
        imgPath = "D:\\SIGGRAPH\\Obj_Kinect_Opengl_Frame_human_deco\\glut\\person.bmp";
        piPath = "D:\\SIGGRAPH\\Obj_Kinect_Opengl_Frame_human_deco\\glut\\personP.txt";
        objPath = "D:\\SIGGRAPH\\Obj_Kinect_Opengl_Frame_human_deco\\glut\\frame\\person" + count + ".obj";
        flagPath = "D:\\SIGGRAPH\\Obj_Kinect_Opengl_Frame_human_deco\\glut\\frame\\flag" + count + ".txt";
        ...
        if(File.Exists(objPath) && File.Exists(facePath) && File.Exists(uvPath) && File.Exists(piPath))
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
        
> ###### * obj파일에서 index를 통해 인체 부분의 어디인지 파악할 수 있음. 그 중 오른손과 왼손에 Mesh Collider를 추가해 충돌검사 수행
> ###### * 학습한 행동을 블렌딩하여 수행하는 3D 캐릭터는 유니티쨩으로 결정
