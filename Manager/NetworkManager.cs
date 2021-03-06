﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using UDPClientModule;
using UDPServerModule;

public class NetworkManager : MonoBehaviour {

	//from UDP Socket API
	private UDPClientComponent udpClient;

	//Variable that defines comma character as separator
	static private readonly char[] Delimiter = new char[] {','};

	//useful for any gameObject to access this class without the need of instances her or you declare her
	public static NetworkManager instance;

	//flag which is determined the player is logged in the arena
	public bool onLogged = false;

	//store localPlayer
	public GameObject myPlayer;

	//local player id
	public string myId = string.Empty;

	//local player id
	public string local_player_id;

	//store all players in game
	public Dictionary<string, PlayerManager> networkPlayers = new Dictionary<string, PlayerManager>();

	//store the local players' models
	public GameObject[] localPlayersPrefabs;

	//store the networkplayers' models
	public GameObject[] networkPlayerPrefabs;

	//stores the spawn points 
	public Transform[] spawnPoints;

	//camera prefab
	public GameObject camRigPref;

	public GameObject camRig;

	public int serverPort = 3310;
	
	public int clientPort = 3000;

	public bool tryJoinServer;

	public bool waitingAnswer;

	public bool serverFound;

	public bool waitingSearch;

	public bool gameIsRunning;

	public int maxReconnectTimes = 10;

	public int contTimes;

	public float maxTimeOut;

	public float timeOut;

	public List<string> _localAddresses { get; private set; }

    public TitleButton titlebutton;

    public bool[] CharSelect;
    
    public int Player_num = 0;
    public Text[] players_name_text;
    public string[] players_name_string;
    public Text current_player_count;
    public Text player_name;
    private List<String> player_names;
    public string[,] player_name_list = new string[4, 4];

    public bool is_host = false;
    private bool is_ready = false;

    public bool is_multi = false;
    public int boss_index;
    public int level_index;
    public int[,] char_index;
    public bool EveryOneGameReady = false;
    public int my_index;
    public GameObject My_character;

    public Vector2[] Skill_pos;

    public Vector2[] character_pos;
    public float[] character_speed;
    private ParticleSystem ps;
    public bool[] boss_level;
    public bool is_allDead;
    private bool[] is_dead;
    private bool is_ConnectAgain;
    private bool is_firstConnect;

    private List<GameObject> Boss2_NormalAttack_Obj;

    // Use this for initialization
    void Start() {
        my_index = -1;
        is_firstConnect = true;
        is_dead = new bool[4];
        is_ConnectAgain = false;
        for (int i=0;i<is_dead.Length;i++)
        {
            is_dead[i] = false;
        }
        CharSelect = new bool[4];
        char_index = new int[4, 2];
        Skill_pos = new Vector2[4];
        character_pos = new Vector2[4];
        players_name_string = new string[4];
        character_speed = new float[4];
        boss_level = new bool[80];
        Boss2_NormalAttack_Obj = new List<GameObject>();
        // if don't exist an instance of this class
        if (instance == null) {

            //it doesn't destroy the object, if other scene be loaded
            DontDestroyOnLoad(this.gameObject);

            instance = this;// define the class as a static variable

	
			udpClient = gameObject.GetComponent<UDPClientComponent>();
		
			//find any  server in others hosts
			ConnectToUDPServer(serverPort, clientPort);

			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

			string address = string.Empty;

			string subAddress = string.Empty;

			_localAddresses = new List<string>();

		}
		else
		{
			//it destroys the class if already other class exists
			Destroy(this.gameObject);
		}

    }
    public void SwitchPlayer()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "SwitchPlayer";

        string msg = my_index.ToString() + ',' + GameManage.instance.player[0].GetComponent<CharacterStat>().char_num.ToString();
        //sends to the server through socket UDP the jo package 
        udpClient.Emit(data["callback_name"], msg);
    }
    public void MaterialChange(string Material_Before, string Material_After,float timer)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Material_Change";

        data["Material"] = Material_Before+','+ Material_After;
        
        data["index"] = my_index.ToString() + ',' +timer.ToString();

        //send the position point to server
        string msg = data["Material"] + ',' + data["index"];
        //sends to the server through socket UDP the jo package 
        udpClient.Emit(data["callback_name"], msg);
    }
    public void HealEffect(bool is_heal, Vector3 pos, float Heal)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Heal_Instance";

        data["is_heal"] = is_heal.ToString();

        data["pos"] = pos.x.ToString() + ',' + pos.y.ToString() + ',' + my_index.ToString() + ',' + Heal.ToString();

        //send the position point to server
        string msg = data["is_heal"] + ',' + data["pos"];
        //sends to the server through socket UDP the jo package 
        udpClient.Emit(data["callback_name"], msg);
    }
    public void InstantiateOtherPlayerSkill(int skill, Vector3 pos,Quaternion rot, float timer=100, bool trail=false)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Skill_Instance";

        data["Skill"] = skill.ToString();

        data["transform"] = pos.x.ToString() + ',' + pos.y.ToString() + ',' + (-777 + my_index).ToString() + ',' + rot.x.ToString() + ',' + rot.y.ToString() + ',' + rot.z.ToString() + ',' + my_index.ToString() + ',' + trail.ToString() + ',' + (SkillManager.instance.skill_prefab[skill].transform.localScale.x*2.5f).ToString() + ',' + timer.ToString()+ ','+SkillManager.instance.mouse_pos.x.ToString()+',' + SkillManager.instance.mouse_pos.y.ToString() + ',' + rot.w.ToString();

        //send the position point to server
        string msg = data["Skill"] + ',' + data["transform"];
        //sends to the server through socket UDP the jo package 
        udpClient.Emit(data["callback_name"], msg);
    }
    public void InstantiateOtherPlayerSkill(int skill, Transform trans,float timer=100,bool trail = false,bool is_Small = false)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Skill_Instance";

        data["Skill"] = skill.ToString();

        data["transform"] = trans.position.x.ToString() + ',' + trans.position.y.ToString() + ',' + (-777+my_index).ToString() + ',' + trans.rotation.x.ToString() + ',' + trans.rotation.y.ToString() + ',' + trans.rotation.z.ToString() + ',' + my_index.ToString() + ',' + trail.ToString() + ',' + (trans.localScale.x * 2.5f).ToString()+','+timer.ToString() + ',' + SkillManager.instance.mouse_pos.x.ToString() + ',' + SkillManager.instance.mouse_pos.y.ToString() + ',' + trans.rotation.w.ToString() + ',' + is_Small.ToString();

        //send the position point to server
        string msg = data["Skill"] + ',' + data["transform"];
        //sends to the server through socket UDP the jo package 
        udpClient.Emit(data["callback_name"], msg);
    }
    public void InstantiateOtherPlayerAllBuff(int skill)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Buff_Instance";

        data["Skill"] = skill.ToString();

        data["index"] = my_index.ToString();

        //send the position point to server
        string msg = data["Skill"] + ',' + data["index"];
        //sends to the server through socket UDP the jo package 
        udpClient.Emit(data["callback_name"], msg);
    }
    public void DeleteObject(int skill)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Delete_Object";

        data["Skill"] = skill.ToString();

        data["index"] = my_index.ToString();

        //send the position point to server
        string msg = data["Skill"] + ',' + data["index"];
        //sends to the server through socket UDP the jo package 
        udpClient.Emit(data["callback_name"], msg);
    }
    public void CharacterDead()
    {
        is_dead[my_index] = true;

        for(int i=0;i<Player_num;i++)
        {
            if(is_dead[i])
            {
                is_allDead = true;
            }
            else
            {
                is_allDead = false;
                break;
            }
        }
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "CharacterDead";

        data["index"] = my_index.ToString() + "," + is_allDead.ToString();

        //send the position point to server
        string msg = data["index"];
        //sends to the server through socket UDP the jo package 
        udpClient.Emit(data["callback_name"], msg);
    }
    public void InstantiateBossSkill(int boss_index, int skill_index, Transform trans, float time, bool is_warning, bool is_parent)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Boss_Skill_Instance";

        data["BossIndex"] = boss_index.ToString();

        data["Skillindex"] = skill_index.ToString();

        data["Transform"] = trans.position.x.ToString() + "," + trans.position.y.ToString() + "," + trans.position.z.ToString() + "," + trans.rotation.x.ToString() + "," + trans.rotation.y.ToString() + "," + trans.rotation.z.ToString() + "," + trans.rotation.w.ToString() + "," + trans.localScale.x.ToString() + "," + trans.localScale.y.ToString() + "," + trans.localScale.z.ToString();

        data["Time"] = time.ToString();

        data["Is_warning"] = is_warning.ToString();

        data["Is_parent"] = is_parent.ToString();
        string msg = data["BossIndex"] + ',' + data["Skillindex"] + ',' + data["Transform"] + ',' + data["Time"] + ',' + data["Is_warning"] + ',' + data["Is_parent"];

        udpClient.Emit(data["callback_name"], msg);
    }

    public void InstantiateTargetingBossSkill(int boss_index, int skill_index, int player_inex, float time, bool is_warning)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Boss_Target_Skill_Instance";

        data["BossIndex"] = boss_index.ToString();

        data["Skillindex"] = skill_index.ToString();

        data["player_index"] = player_inex.ToString();

        data["Time"] = time.ToString();

        data["Is_warning"] = is_warning.ToString();
        string msg = data["BossIndex"] + ',' + data["Skillindex"] + ',' + data["player_index"] + ',' + data["Time"] + ',' + data["Is_warning"];

        udpClient.Emit(data["callback_name"], msg);
    }

    public void AccumulatePlayerToBossDamage(int player_index, bool is_first_character, float damage)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Accumulate_Player_To_Boss_Damage";

        data["player_index"] = player_index.ToString();

        data["is_first_character"] = is_first_character.ToString();

        data["damage"] = damage.ToString();

        string msg = data["player_index"] + ',' + data["is_first_character"] + ',' + data["damage"];

        udpClient.Emit(data["callback_name"], msg);
    }

    public void AccumulateBossToPlayerDamage(int player_index, bool is_first_character, float damage)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Accumulate_Boss_To_Player_Damage";

        data["player_index"] = player_index.ToString();

        data["is_first_character"] = is_first_character.ToString();

        data["damage"] = damage.ToString();

        string msg = data["player_index"] + ',' + data["is_first_character"] + ',' + data["damage"];

        udpClient.Emit(data["callback_name"], msg);
    }

    public void AccumulatePlayerHeal(int player_index, bool is_first_character, float heal)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Accumulate_Player_Heal";

        data["player_index"] = player_index.ToString();

        data["is_first_character"] = is_first_character.ToString();

        data["heal"] = heal.ToString();

        string msg = data["player_index"] + ',' + data["is_first_character"] + ',' + data["heal"];

        udpClient.Emit(data["callback_name"], msg);
    }

    public void BossGetSilent(bool flag)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Boss_Get_Silent";

        data["is_Able"] = flag.ToString();

        string msg = data["is_Able"];

        udpClient.Emit(data["callback_name"], msg);
    }

    public void BossGetBuff(int buff_index)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Boss_Get_Buff";

        data["buff_index"] = buff_index.ToString();

        string msg = data["buff_index"];

        udpClient.Emit(data["callback_name"], msg);

    }

    public void BossGetBuff(int buff_index,int player_index)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Boss_Get_Buff_Player";

        data["buff_index"] = buff_index.ToString();

        data["player_index"] = player_index.ToString();

        string msg = data["buff_index"] +','+ data["player_index"];

        udpClient.Emit(data["callback_name"], msg);

    }

    public void BossDeleteBuff(int buff_index)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Boss_Delete_Buff";

        data["buff_index"] = buff_index.ToString();

        string msg = data["buff_index"];

        udpClient.Emit(data["callback_name"], msg);

    }

    public void Boss2Magnetic(int index)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Boss2_Ult_Use";
        data["index"] = index.ToString();
        string msg = data["callback_name"] + ',' + data["index"];
        udpClient.Emit(data["callback_name"], msg);
    }

    public void Boss2Fire(Vector2 pos)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Boss2_Fire_Use";
        data["position"] = pos.x.ToString() + "," + pos.y.ToString();
        string msg = data["player_index"] + ',' + data["position"];
        udpClient.Emit(data["callback_name"], msg);
    }

    public void Boss2SameDir()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Boss2_SameDir_Use";
        string msg = string.Empty;
        udpClient.Emit(data["callback_name"], msg);
    }

    public void Boss3Pile(int skill_index,int player_index, Vector3 ran_pos)
    {
        /*
         * index = target_player's multiplay_index
         */
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Boss3_Pile_Use";

        data["target_player_index"] = player_index.ToString();

        data["skill_index"] = skill_index.ToString();

        data["ran_pos"] = ran_pos.x.ToString() + "," + ran_pos.y.ToString() + "," + ran_pos.z.ToString();

        string msg = data["target_player_index"] +',' + data["skill_index"] + ',' + data["ran_pos"];

        udpClient.Emit(data["callback_name"], msg);
    }

    public void Boss4RollingRock(int skill_index, Vector2 ran_pos)
    {
        /*
         * index = target_player's multiplay_index
         */
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Boss4_RollingRock_Use";

        data["skill_index"] = skill_index.ToString();

        data["ran_pos"] = ran_pos.x.ToString() + "," + ran_pos.y.ToString();

        string msg = data["target_player_index"] + ',' + data["skill_index"] + ',' + data["ran_pos"];

        udpClient.Emit(data["callback_name"], msg);
    }

    public void Boss6Bubble(int skill_index, Vector2 dir, float force)
    {
        /*
         * index = target_player's multiplay_index
         */
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Boss6_Bubble_Use";

        data["skill_index"] = skill_index.ToString();

        data["dir"] = dir.x.ToString() + "," + dir.y.ToString();

        data["force"] = force.ToString();

        string msg = data["target_player_index"] + ',' + data["skill_index"] + ',' + data["dir"] + ',' + data["force"];

        udpClient.Emit(data["callback_name"], msg);
    }

    public void Boss6Volcano()
    {
        /*
         * index = target_player's multiplay_index
         */
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Boss6_Volcano_Use";

        string msg = string.Empty;

        udpClient.Emit(data["callback_name"], msg);
    }

    public void Boss7Sickle(Vector2 dir)
    {
        /*
         * index = target_player's multiplay_index
         */
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Boss7_Sickle_Use";

        data["dir"] = dir.x.ToString() + "," + dir.y.ToString();

        string msg = data["target_player_index"] + ',' + data["dir"];

        udpClient.Emit(data["callback_name"], msg);
    }
    /// <summary>
    /// Connect client to any UDP server.
    /// </summary>
    public void ConnectToUDPServer(int _serverPort, int _clientPort)
	{
        /*if(!is_firstConnect)
        {
            is_firstConnect = false;
            return;
        }*/

		if (udpClient.GetServerIP () != string.Empty) {

            //connect to udp server

            udpClient.connect (udpClient.GetServerIP (), _serverPort, _clientPort);

			//The On method in simplistic terms is used to map a method name to an annonymous function.
			udpClient.On ("PONG", OnPrintPongMsg);

			udpClient.On ("Give_Index", OnGiveIndex);

			udpClient.On ("JOIN_SUCCESS", OnJoinGame);

			udpClient.On ("SPAWN_PLAYER", OnSpawnPlayer);

			udpClient.On ("UPDATE_MOVE", OnUpdatePosition);

            udpClient.On("UPDATE_BOSS_MOVE", OnUpdateBossPosition);

			udpClient.On ("UPDATE_ROTATE", OnUpdateRotation);

			udpClient.On ("UPDATE_ATACK",OnUpdateAttack);

            udpClient.On("UPDATE_AGGRO", OnUpdateAggro);

            udpClient.On ("UPDATE_PHISICS_DAMAGE",OnUpdatePlayerPhisicsDamage);

			udpClient.On ("UPDATE_PLAYER_ANIMATOR", OnUpdateAnim);

			udpClient.On ("USER_DISCONNECTED", OnUserDisconnected);		

			udpClient.On ("ReadyCount", OnUserReadyCount);

            udpClient.On("GameStart", OnUserGameStart);

            udpClient.On("BossSeleted", OnUserBossSlected);

            udpClient.On("SceneStart", OnUserSceneStart);

            udpClient.On("ServerClosed", OnUserServerClosed);

            udpClient.On("EveryChar", OnUserEveryChar);

            udpClient.On("MaterialMake", OnUserMaterialMake);

            udpClient.On("HealMake", OnUserHealMake);

            udpClient.On("SkillMake", OnUserSkillMake);

            udpClient.On("BuffMake", OnUserBuffMake);

            udpClient.On("ClearObject", OnUserClearObject);

            udpClient.On("OnePerosnDead", OnUserOnePerosnDead);

            udpClient.On("BOSS_SKILL_MAKE", OnUserBossSkillMake);

            udpClient.On("SwitchPlayer", OnUserSwitchPlayer);

            udpClient.On("UPDATE_Boss_Get_Silent", OnUserBossGetSilent);

            udpClient.On("UPDATE_Boss_Get_Buff", OnUserBossGetBuff);

            udpClient.On("UPDATE_Boss_Get_Buff_Player", OnUserBossGetBuffPlayer);

            udpClient.On("UPDATE_Boss_Delete_Buff", OnUserBossDeleteBuff);

			udpClient.On("BOSS_TARGET_SKILL_MAKE", OnUserBossTargetSkillMake);

            udpClient.On("UPDATE_DAMAGE_ACCUMULATE", OnUserDamageAccumulate);

            udpClient.On("UPDATE_DAMAGE_ACCUMULATE_From_Boss", OnUserDamageAccumulateFromBoss);

            udpClient.On("UPDATE_DAMAGE_ACCUMULATE_Player_Heal", OnUserHealAccumulate);

            udpClient.On("UPDATE_Boss2_ULT", OnUserBoss2ULT);

            udpClient.On("UPDATE_Boss2_Fire", OnUserBoss2Fire);

            udpClient.On("UPDATE_Boss2_SameDir", OnUserBoss2SameDir);

            udpClient.On("UPDATE_Boss3_Pile", OnUserBoss3Pile);

            udpClient.On("UPDATE_Boss4_RollingRock", OnUserBoss4RollingRock);

            udpClient.On("UPDATE_Boss6_Bubble", OnUserBoss6Bubble);

            udpClient.On("UPDATE_Boss6_Volcano", OnUserBoss6Volcano);

            udpClient.On("UPDATE_Boss7_Sickle", OnUserBoss7Sickle);
        }
	}

    void SetPlayerPosition()
    {
        for(int i=0;i<Player_num;i++)
        {
            if (i == my_index) continue;

            if(character_pos[i].magnitude > 1f)//갑작스런 이동
            {
                GameManage.instance.Character_other[i].transform.Translate(character_speed[i] * character_pos[i] * Time.deltaTime);
            }
            else if(character_pos[i].magnitude>0.1f)//평소 걸음
            {
                GameManage.instance.Character_other[i].transform.Translate(character_speed[i] * character_pos[i].normalized * Time.deltaTime);
            }
            //Debug.Log("characterpos magnitude : " +character_pos[i].magnitude * Time.deltaTime); //0.04

            //if (character_pos[i].magnitude > 0.5f)
            //    Debug.Log("character_pos magnitude : " + character_pos[i].magnitude); //0.009 멈춰있을때  막 달릴때 0.5까지 벌어질때도있음 1정도 유지하는게 제일좋음
            //Debug.Log("real pos : " + GameManage.instance.Character_other[i].transform.position);//얼마나 왔다갔다 거리는지 쳌     0.1정도 달라짐       

        }
    }
    void Syncro()
    {
        //먼저 주고
        //hash table <key, value>	
        EmitPlayerPosition(My_character.transform.position);
        //받고

        SetPlayerPosition();

        if (is_host && BossStatus.instance != null)    // 호스트인 사람만 보스의 정보를 업로드 한다.
        {
            EmitBossPosition();
        }
    }


    void FixedUpdate()
	{
		//if it was not found a server
		if (!serverFound) {
			
			//tries to obtain a "pong" of some local server
			StartCoroutine ("PingPong");
		}
		//found server
		else
		{
			//if the player is already in game
			if (gameIsRunning)
			{
                //maintain a connection with the server to detect disconnection
                //StartCoroutine ("PingPong");


                Syncro();

				/*************** verifies the disconnection of some player ***************/
				List<string> keys = new List<string> (networkPlayers.Keys);

                //current_player_count.text = Player_num.ToString();
                //for (int i = 0; i < Player_num; i++)
                //{
                //    players_name_text[i].text = player_names[i];
                //}

                foreach (string key in keys) {

					if (networkPlayers.ContainsKey (key)) {

						if (networkPlayers [key] != null) {

							//increases the time of wait
							networkPlayers [key].timeOut += Time.deltaTime;

							//the client is verified exceeded the time limits of wait
							if (networkPlayers [key].timeOut >= maxTimeOut) {
						
							
								//destroy network player by your id
							//	Destroy (networkPlayers [key].gameObject);

								//remove from the dictionary
							//	networkPlayers.Remove (networkPlayers [key].id);
							
							}
						}
					}

				}//END_FOREACH
			/*************************************************************************/	

			}
		}
	}


	/// <summary>
	/// corroutine called  of times in times to send a ping to the server
	/// </summary>
	/// <returns>The pong.</returns>
	private IEnumerator PingPong()
	{

		if (waitingSearch)
		{
			yield break;
		}

		waitingSearch = true;

		//sends a ping to server
		EmitPing ();

		//important to verify the server it is connected
		if (gameIsRunning)
		{
			//number of pings sent to the server without answer
			contTimes++;
		}


		// wait 1 seconds and continue
		yield return new WaitForSeconds(1);

		//if contTimes arrived to the maximum value of attempts means that the server is not more answering or it disconnected
		if (contTimes > maxReconnectTimes )
		{
			contTimes = 0;

			//restarts the game so that a new server is created
			RestartGame ();
		}

		waitingSearch = false;

	}

	//function to help to detect flaw in the connection
	public IEnumerator WaitAnswer()
	{
		if (waitingAnswer)
		{
			yield break;
		}
	
		tryJoinServer = true;

		waitingAnswer = true;

		//CanvasManager.instance.ShowLoadingImg ();

		yield return new WaitForSeconds(5f);

		//CanvasManager.instance.CloseLoadingImg ();

		waitingAnswer = false;
	   
		//if true we lost the package the servant didn't answer
		//take a look in public void OnJoinGame(SocketUDPEvent data) function
		if (tryJoinServer) {
			
			tryJoinServer = false;

            Debug.Log("LOST PACKAGE! PLEASE TRY AGAIN! ");

			//CanvasManager.instance.CloseLoadingImg();

		}


	}

	//it generates a random id for the local player
	public string generateID()
	{
		string id = Guid.NewGuid().ToString("N");

		//reduces the size of the id
		id = id.Remove (id.Length - 15);

		return id;
	}

	/// <summary>
	///  receives an answer of the server.
	/// from  void OnReceivePing(string [] pack,IPEndPoint anyIP ) in server
	/// </summary>
	public void OnPrintPongMsg(SocketUDPEvent data)
	{
		/*
		 * data.pack[0]= CALLBACK_NAME: "PONG"
		 * data.pack[1]= "pong!!!!"
		*/
		serverFound = true;

		contTimes = 0;

		//arrow the located text in the inferior part of the game screen
		CanvasManager.instance.txtSearchServerStatus.text = "------- server is running -------";	
	}

	// <summary>
	/// sends ping message to server.
	/// to  case "PING":
	///     OnReceivePing(pack,anyIP);
	///     break;
	/// take a look in UDPServer.cs script
	/// </summary>
	public void EmitPing() {
        //hash table <key, value>	
        Dictionary<string, string> data = new Dictionary<string, string>();

		//JSON package
		data["callback_name"] = "PING";

		//store "ping!!!" message in msg field
		data["msg"] = "ping!!!!";

		//The Emit method sends the mapped callback name to  the server
		udpClient.Emit (data["callback_name"] ,data["msg"]);

	}
		

	/// <summary>
	///  tries to put the player in game
	/// </summary>
	public void EmitJoin()
	{
		// verifies WiFi connection
		if (!udpClient.noNetwork) {


			if (serverFound) {
                //ConnectToUDPServer(serverPort, clientPort);
                //tries to put the player in game
                TryJoinServer ();
                //titlebutton.StartGame();
            }
			else
			{
				if (UDPServer.instance.serverRunning)
				{
                    //ConnectToUDPServer(serverPort, clientPort);
                    TryJoinServer ();
				} 
				else 
				{
                    //CanvasManager.instance.ShowAlertDialog ("PLEASE START THE SERVER");
                    Debug.Log("Start Server");
                    is_host = true;
                    UDPServer.instance.CreateServer();
				}
			}

		}//END_IF
		else
		{
			
			if (udpClient.noNetwork) {
				
				//CanvasManager.instance.ShowAlertDialog ("PLEASE CONNECT TO ANY WIFI NETWORK");
                Debug.Log("Need Connect NetWork");

            }

            else
			{
				if (serverFound) {

                    //ConnectToUDPServer(serverPort, clientPort);
                    TryJoinServer ();
				}

				else
				{
                    //CanvasManager.instance.ShowAlertDialog ("THERE NO ARE SERVER RUNNING ON NETWORK!");
                    Debug.Log("No Server");

                }

            }
		}

	}

	/// <summary>
	/// Tries the join server.
	///  case "JOIN":
	///  OnReceiveJoin(pack,anyIP);
	///  break;
	///  take a look in UDPServer.cs script
	/// </summary>
	public void TryJoinServer()
	{
        if(is_ConnectAgain)//나갔다가 다시 들어오는경우
        {
            is_ConnectAgain = false;
            //ConnectToUDPServer(serverPort, clientPort);   //더 큰범위
            //udpClient.connect(udpClient.GetServerIP(), serverPort, clientPort); //더 큰범위
            udpClient.OnListeningServer();
        }

		//hash table <key, value>	
		Dictionary<string, string> data = new Dictionary<string, string> ();
        data ["callback_name"] = "JOIN";//
        
        if(player_name==null)
        {
            titlebutton = Camera.main.GetComponent<TitleButton>();
        }

        data["player_name"] = player_name.text;

        //it is already verified an id was generated
        if (myId.Contains (string.Empty)) {

            myId = generateID ();

			data ["player_id"] = myId;
		}
		else
		{
			data ["player_id"] = myId;
		}
        local_player_id = myId;//koki

        player_name.text = null;
        //makes the draw of a point for the player to be spawn
        //int index = Random.Range (0, spawnPoints.Length);

        Vector3 position = new Vector3(0,0,0);
        data["position"] = position.x + "," + position.y + "," + position.z;
        int[] boss_clear = new int[8];
        System.Array.Clear(boss_clear, 0, boss_clear.Length);
        for(int i = 0; i <8; i++)
        {
            int t = i * 8 + 4;
            for(int j = i * 8; j< t; j++)
            {
                if (UpLoadData.boss_is_cleared[j])
                {
                    boss_clear[i]++;
                }

            }
        }

        data["boss_clear"] = boss_clear[0].ToString();

        for(int i = 1; i < 8; i++)
        {
            data["boss_clear"] += "," + boss_clear[i].ToString();
        }
        Debug.Log(data["boss_clear"]);
        //send the position point to server
        string msg = data["player_name"] + "," + data["player_id"]+"," + data["position"] + "," + data["boss_clear"];

        //sends to the server through socket UDP the jo package 

        udpClient.Emit (data ["callback_name"], msg);
        //we waited for a time to verify the connection
        StartCoroutine (WaitAnswer ());
        //ConnectToUDPServer(serverPort, clientPort);
    }
    public void BossindexPlus(bool is_On)
    {
        //hash table <key, value>	
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "Bossindex";//preenche com o id da callback receptora que está no servidor

        data["bossindex"] = boss_index.ToString();

        data["levelindex"] = UpLoadData.boss_level.ToString();

        string msg = data["bossindex"] + ',' + data["levelindex"];

        data["d"] = "a";

        if (!is_On)
        {
            data["d"] = "b";
        }
            msg += ',' + data["d"];

        //sends to the server through socket UDP the jo package 
        udpClient.Emit(data["callback_name"], msg);
    }
    /// <summary>
    /// Joins the local player in game.
    /// </summary>
    /// <param name="_data">Data.</param>
    /// 
    
    public void OnGiveIndex(SocketUDPEvent data)
    {
        /*
		 * data.data.pack[0] = CALLBACK_NAME: "give_index" from server
		 * data.data.pack[1] = index
		*/
        if (my_index != -1) return;

        my_index = int.Parse(data.pack[1]);
        Debug.Log("Take index from server , index : " +my_index);
    }

    public void OnJoinGame(SocketUDPEvent data)
	{
        /*
		 * data.data.pack[0] = CALLBACK_NAME: "JOIN_SUCCESS" from server
		 * data.data.pack[1] = id (local player id)
		 * data.data.pack[2]= name (local player name)
         * data.data.pack[3] = player num
         * data.data.pack[4] = player index
         * data.data.pack[5] = boss1_level;
         * data.data.pack[6] = boss2_level;
         * data.data.pack[7] = boss3_level;
         * data.data.pack[8] = boss4_level;
         * data.data.pack[9] = boss5_level;
         * data.data.pack[10] = boss6_level;
         * data.data.pack[11] = boss7_level;
         * data.data.pack[12] = boss8_level;
		*/

        int boss1_level = int.Parse(data.pack[5]);
        int boss2_level = int.Parse(data.pack[6]);
        int boss3_level = int.Parse(data.pack[7]);
        int boss4_level = int.Parse(data.pack[8]);
        int boss5_level = int.Parse(data.pack[9]);
        int boss6_level = int.Parse(data.pack[10]);
        int boss7_level = int.Parse(data.pack[11]);
        int boss8_level = int.Parse(data.pack[12]);

        Debug.Log(boss1_level);
        Debug.Log(boss2_level);
        Debug.Log(boss3_level);
        Debug.Log(boss4_level);
        Debug.Log(boss5_level);
        Debug.Log(boss6_level);
        Debug.Log(boss7_level);
        Debug.Log(boss8_level);

        for (int i = 4; i < 8; i++)
        {
            if (boss1_level > 0)
            {
                boss_level[i] = true;
                boss1_level--;
            }
            else
            {
                boss_level[i] = false;
            }
        }

        for (int i = 12; i < 16; i++)
        {
            if (boss2_level > 0)
            {
                boss_level[i] = true;
                boss2_level--;
            }
            else
            {
                boss_level[i] = false;
            }
        }

        for (int i = 20; i < 24; i++)
        {
            if (boss3_level > 0)
            {
                boss_level[i] = true;
                boss3_level--;
            }
            else
            {
                boss_level[i] = false;
            }
        }

        for (int i = 28; i < 32; i++)
        {
            if (boss4_level > 0)
            {
                boss_level[i] = true;
                boss4_level--;
            }
            else
            {
                boss_level[i] = false;
            }
        }

        for (int i = 36; i < 40; i++)
        {
            if (boss5_level > 0)
            {
                boss_level[i] = true;
                boss5_level--;
            }
            else
            {
                boss_level[i] = false;
            }
        }

        for (int i = 44; i < 48; i++)
        {
            if (boss6_level > 0)
            {
                boss_level[i] = true;
                boss6_level--;
            }
            else
            {
                boss_level[i] = false;
            }
        }

        for (int i = 52; i < 56; i++)
        {
            if (boss7_level > 0)
            {
                boss_level[i] = true;
                boss7_level--;
            }
            else
            {
                boss_level[i] = false;
            }
        }

        for (int i = 60; i < 64; i++)
        {
            if (boss8_level > 0)
            {
                boss_level[i] = true;
                boss8_level--;
            }
            else
            {
                boss_level[i] = false;
            }
        }
        Debug.Log("Login successful, joining game");
        //Player_num++;
        //Debug.Log(Player_num);

        players_name_text[int.Parse(data.pack[4])].text = (data.pack[2]).ToString();
        players_name_string[int.Parse(data.pack[4])] = (data.pack[2]).ToString();
        current_player_count.text = data.pack[3];
        Player_num = int.Parse(data.pack[3]);

        //titlebutton.StartGame();
        tryJoinServer = false;
        onLogged = true;


        if (!myPlayer) {

            // take a look in PlayerManager.cs script
            //PlayerManager newPlayer;
            // newPlayer = GameObject.Instantiate( local player avatar or model, spawn position, spawn rotation)
            //newPlayer = GameObject.Instantiate (localPlayersPrefabs [0],new Vector3(float.Parse(data.pack[3]), float.Parse(data.pack[4]),float.Parse(data.pack[5])),Quaternion.identity).GetComponent<PlayerManager> ();

            Debug.Log("player instantiated");

			//newPlayer.id = data.pack [1];

			//this is local player
			//newPlayer.isLocalPlayer = true;

			//now local player online in the arena
			//newPlayer.isOnline = true;

			//set local player's 3D text with his name
			//newPlayer.Set3DName(data.pack[2]);

			//puts the local player on the list
			//networkPlayers [data.pack [1]] = newPlayer;

			//myPlayer = networkPlayers [data.pack[1]].gameObject;

			local_player_id =  data.pack [1];

			//spawn cam
			//camRig = GameObject.Instantiate (camRigPref, new Vector3 (0f, 0f, 0f), Quaternion.identity);

			//set local player how  being MultipurposeCameraRig target to follow him
			//camRig.GetComponent<CameraFollow> ().SetTarget (myPlayer.transform, newPlayer.cameraTotarget);

			//CanvasManager.instance.healthSlider.value = newPlayer.gameObject.GetComponent<PlayerHealth>().health;

			//CanvasManager.instance.txtHealth.text = "HP " + newPlayer.gameObject.GetComponent<PlayerHealth>().health + " / " +
			//	newPlayer.gameObject.GetComponent<PlayerHealth>().maxHealth;
			
			//hide the lobby menu (the input field and join buton)
			//CanvasManager.instance.OpenScreen(3);

			//CanvasManager.instance.CloseLoadingImg ();

			//CanvasManager.instance.lobbyCamera.GetComponent<Camera> ().enabled = false;
			//CanvasManager.instance.CloseLoadingImg();

			//take a look in public IEnumerator WaitAnswer()
			tryJoinServer = false;

			// the local player now is logged
			onLogged = true;

			Debug.Log("player in game");
		}
	}

	/// <summary>
	/// Raises the spawn player event.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnSpawnPlayer(SocketUDPEvent data)
	{

		/*
		 * data.pack[0] = SPAWN_PLAYER
		 * data.pack[1] = id (network player id)
		 * data.pack[2]= name
		 * data.pack[3] = position.x
		 * data.pack[4] = position.y
		 * data.pack[5] = position.z
		 * data.pack[6] = rotation.x
		 * data.pack[7] = rotation.y
		 * data.pack[8] = rotation.z
		 * data.pack[9] = rotation.w
		*/
        /*
		if (onLogged ) {

		
			bool alreadyExist = false;

			//verify all players to  prevents copies
			foreach(KeyValuePair<string, PlayerManager> entry in networkPlayers)
			{
				// same id found ,already exist!!! 
				if (entry.Value.id== data.pack [1])
				{
					alreadyExist = true;
				}
			}
			if (!alreadyExist) {

				Debug.Log("creating a new player");

				PlayerManager newPlayer;

				// newPlayer = GameObject.Instantiate( network player avatar or model, spawn position, spawn rotation)
				newPlayer = GameObject.Instantiate (networkPlayerPrefabs [0],
					new Vector3(float.Parse(data.pack[3]), float.Parse(data.pack[4]), 
						float.Parse(data.pack[5])),Quaternion.identity).GetComponent<PlayerManager> ();


				//it is not the local player
				newPlayer.isLocalPlayer = false;

				//network player online in the arena
				newPlayer.isOnline = true;

				//set the network player 3D text with his name
				newPlayer.Set3DName(data.pack[2]);

				newPlayer.gameObject.name = data.pack [1];

				//puts the local player on the list
				networkPlayers [data.pack [1]] = newPlayer;
			}

		}*/

	}

	/// <summary>
	///  Update the network player position to local player.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnUpdatePosition(SocketUDPEvent data)
	{

		/*
		 * data.pack[0] = UPDATE_MOVE
		 * data.pack[1] = index
		 * data.pack[2] = position.x
		 * data.pack[3] = position.y
		 * data.pack[4] = dir.x
		 * data.pack[5] = dir.y
		 * data.pack[6] = move
		 * data.pack[7] = attack
		 * data.pack[8] = magic
		 * data.pack[9] = dead		 
		 * data.pack[10] = maxhp		 
		 * data.pack[11] = hp	 
         * data.pack[12] = maxmp		 
		 * data.pack[13] = mp	 
		 * data.pack[14] = movespeed	 
		 * data.pack[15] = magicattack	 
		 * data.pack[16] = critical 
		*/

        //it reduces to zero the accountant meaning that answer of the server exists to this moment
        contTimes = 0;

        //find network player

        //instacne한 아이의 정보를

        if (int.Parse(data.pack[1]) == my_index)return;

       // GameManage.instance.Character_other[int.Parse(data.pack[1])].transform.position = Vector2.Lerp(GameManage.instance.Character_other[int.Parse(data.pack[1])].transform.position, new Vector2(float.Parse(data.pack[2]), float.Parse(data.pack[3])), Time.deltaTime*100);
        Vector2 dir = new Vector2(float.Parse(data.pack[2]), float.Parse(data.pack[3]));
        dir -= (Vector2)GameManage.instance.Character_other[int.Parse(data.pack[1])].transform.position;

        character_pos[int.Parse(data.pack[1])] = dir;
        character_speed[int.Parse(data.pack[1])] = float.Parse(data.pack[14]);
        GameManage.instance.Character_other[int.Parse(data.pack[1])].GetComponent<Animator>().SetFloat("DirX", float.Parse(data.pack[4]));
        GameManage.instance.Character_other[int.Parse(data.pack[1])].GetComponent<Animator>().SetFloat("DirY", float.Parse(data.pack[5]));
        GameManage.instance.Character_other[int.Parse(data.pack[1])].GetComponent<Animator>().SetBool("Running", bool.Parse(data.pack[6]));
        GameManage.instance.Character_other[int.Parse(data.pack[1])].GetComponent<Animator>().SetBool("Attack", bool.Parse(data.pack[7]));
        GameManage.instance.Character_other[int.Parse(data.pack[1])].GetComponent<Animator>().SetBool("IsMagic", bool.Parse(data.pack[8]));
        GameManage.instance.Character_other[int.Parse(data.pack[1])].GetComponent<Animator>().SetBool("IsDead", bool.Parse(data.pack[9]));
        GameManage.instance.Character_other[int.Parse(data.pack[1])].GetComponent<CharacterStat>().MaxHP = float.Parse(data.pack[10]);
        GameManage.instance.Character_other[int.Parse(data.pack[1])].GetComponent<CharacterStat>().HP = float.Parse(data.pack[11]);
        GameManage.instance.Character_other[int.Parse(data.pack[1])].GetComponent<CharacterStat>().MaxMp = float.Parse(data.pack[12]);
        GameManage.instance.Character_other[int.Parse(data.pack[1])].GetComponent<CharacterStat>().MP = float.Parse(data.pack[13]);
        GameManage.instance.Character_other[int.Parse(data.pack[1])].GetComponent<CharacterStat>().MagicAttackPower = float.Parse(data.pack[15]);
        GameManage.instance.Character_other[int.Parse(data.pack[1])].GetComponent<CharacterStat>().ciritical = float.Parse(data.pack[16]);
    }

    /// <summary>
	///  Update the network boss position to local boss.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnUpdateBossPosition(SocketUDPEvent data)
    {
        /*
          * data.pack[0] = CALLBACK_NAME: "BOSS_MOVE"
          * data.pack[1] = BossIndex
          * data.pack[2] = position.x
          * data.pack[3] = position.y
          * data.pack[4] = dirx
          * data.pack[5] = diry
          * data.pack[6] = move
          * data.pack[7] = attack
          * data.pack[8] = attack_end
          * data.pack[9] = magic		              
          * data.pack[10] = magic_end		              
          * data.pack[11] = silent
          * data.pack[12] = maxhp	
          * data.pack[13] = hp		              
          * data.pack[14] = maxmp		              
          * data.pack[15] = mp 
          * data.pack[16] = movespeed 
        */
        //instacne한 아이의 정보를
        if (is_host) return;

        Vector2 dir = new Vector2(float.Parse(data.pack[2]), float.Parse(data.pack[3]));
        Vector2 boss_pos = BossStatus.instance.transform.position;
        dir -= boss_pos;
        float movespeed = float.Parse(data.pack[16]);


        if (dir.magnitude > movespeed * Time.deltaTime)
        {
            dir.Normalize();
            BossStatus.instance.gameObject.transform.Translate(movespeed * dir * Time.deltaTime);
        }
        BossStatus.instance.GetComponent<Animator>().SetFloat("DirX", float.Parse(data.pack[4]));
        BossStatus.instance.GetComponent<Animator>().SetFloat("DirY", float.Parse(data.pack[5]));
        BossStatus.instance.GetComponent<Animator>().SetBool("Running", bool.Parse(data.pack[6]));
        BossStatus.instance.GetComponent<Animator>().SetBool("Attack", bool.Parse(data.pack[7]));
        BossStatus.instance.GetComponent<Animator>().SetBool("AttackEnd", bool.Parse(data.pack[8]));
        BossStatus.instance.GetComponent<Animator>().SetBool("Magic", bool.Parse(data.pack[9]));
        BossStatus.instance.GetComponent<Animator>().SetBool("MagicEnd", bool.Parse(data.pack[10]));
        BossStatus.instance.GetComponent<Animator>().SetBool("Silent", bool.Parse(data.pack[11]));
        BossStatus.instance.GetComponent<BossStatus>().MaxHP = float.Parse(data.pack[12]);
        BossStatus.instance.GetComponent<BossStatus>().HP = float.Parse(data.pack[13]);
        BossStatus.instance.GetComponent<BossStatus>().MaxMp = float.Parse(data.pack[14]);
        BossStatus.instance.GetComponent<BossStatus>().MP = float.Parse(data.pack[15]);
        BossStatus.instance.GetComponent<BossStatus>().moveSpeed = float.Parse(data.pack[16]);

    }

    /// <summary>
    /// Emits the local player position to server.
    /// </summary>
    /// <param name="_pos">Position.</param>
    //responsible method for transmitting to the server the movement of the player associated to this client
    public void EmitPlayerPosition(Vector3 _pos)
	{
		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data["callback_name"] = "MOVE";

        data["My_index"] = my_index.ToString();

		Vector2 position = new Vector2( _pos.x,_pos.y);

        data["position"] = position.x + "," + position.y;

        data["Dir"] = My_character.GetComponent<Animator>().GetFloat("DirX").ToString() + "," + My_character.GetComponent<Animator>().GetFloat("DirY").ToString();
        
        data["Is_Move"] = My_character.GetComponent<Animator>().GetBool("Running").ToString();

        data["Is_Attack"] = My_character.GetComponent<Animator>().GetBool("Attack").ToString();

        data["Is_Magic"] = My_character.GetComponent<Animator>().GetBool("IsMagic").ToString();

        data["Is_Dead"] = My_character.GetComponent<Animator>().GetBool("IsDead").ToString();

        data["CharacterMaxHP"] = My_character.GetComponent<CharacterStat>().MaxHP.ToString();

        data["CharacterHP"] = My_character.GetComponent<CharacterStat>().HP.ToString();

        data["CharacterMaxMP"] = My_character.GetComponent<CharacterStat>().MaxMp.ToString();

        data["CharacterMP"] = My_character.GetComponent<CharacterStat>().MP.ToString();

        data["CharacterMoveSpeed"] = My_character.GetComponent<CharacterStat>().move_speed.ToString();

        data["CharacterMagicAttack"] = My_character.GetComponent<CharacterStat>().MagicAttackPower.ToString();

        data["Charactercritical"] = My_character.GetComponent<CharacterStat>().ciritical.ToString();

        //data["CharacterMoveSpeed"] = My_character.GetComponent<CharacterStat>().move_speed.ToString();

        //send the position point to server
        string msg = data["My_index"] +","+data["position"] + ","  + data["Dir"] + "," + data["Is_Move"] + "," + data["Is_Attack"] + "," + data["Is_Magic"] + "," + data["Is_Dead"] + "," + data["CharacterMaxHP"] + "," + data["CharacterHP"] + "," + data["CharacterMaxMP"] + "," + data["CharacterMP"]+","+data["CharacterMoveSpeed"] + "," + data["CharacterMagicAttack"] + "," + data["Charactercritical"];
        //sends to the server through socket UDP the jo package 
        udpClient.Emit (data["callback_name"],msg);
	}

    void EmitBossPosition()
    {
        //hash table <key, value>
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "BOSS_MOVE";

        data["boss_index"] = (UpLoadData.boss_index).ToString(); 

        data["boss_position"] = BossStatus.instance.transform.position.x + "," + BossStatus.instance.transform.position.y;

        data["Dir"] = BossStatus.instance.GetComponent<Animator>().GetFloat("DirX").ToString() + "," + BossStatus.instance.GetComponent<Animator>().GetFloat("DirY").ToString();

        data["Is_Move"] = BossStatus.instance.GetComponent<Animator>().GetBool("Running").ToString();

        data["Is_Attack"] = BossStatus.instance.GetComponent<Animator>().GetBool("Attack").ToString();

        data["Is_Attack_End"] = BossStatus.instance.GetComponent<Animator>().GetBool("AttackEnd").ToString();

        data["Is_Magic"] = BossStatus.instance.GetComponent<Animator>().GetBool("Magic").ToString();

        data["Is_Magic_End"] = BossStatus.instance.GetComponent<Animator>().GetBool("MagicEnd").ToString();

        data["Is_Silent"] = BossStatus.instance.GetComponent<Animator>().GetBool("Silent").ToString();

        data["BossMaxHP"] = BossStatus.instance.MaxHP.ToString();

        data["BossHP"] = BossStatus.instance.HP.ToString();

        data["BossMaxMP"] = BossStatus.instance.MaxMp.ToString();

        data["BossMP"] = BossStatus.instance.MP.ToString();

        data["BossSpeed"] = BossStatus.instance.moveSpeed.ToString();



        //send the position point to server
        string msg = data["boss_index"] + "," + data["boss_position"] + "," + data["Dir"] + "," + data["Is_Move"] + "," + data["Is_Attack"] + "," + data["Is_Attack_End"] + "," + data["Is_Magic"] + "," + data["Is_Magic_End"] + "," + data["Is_Silent"] + "," + data["BossMaxHP"] + "," + data["BossHP"] + "," + data["BossMaxMP"] + "," + data["BossMP"] + "," + data["BossSpeed"];
        //sends to the server through socket UDP the jo package 
        udpClient.Emit(data["callback_name"], msg);

    }

	/// <summary>
	/// Update the network player rotation to local player.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnUpdateRotation(SocketUDPEvent data)
	{
		
		/*
		 * data.pack[0] = UPDATE_ROTATE
		 * data.pack[1] = id (network player id)
		 * data.pack[2] = rotation.x
		 * data.pack[3] = rotation.y
		 * data.pack[4] = rotation.z
		 * data.pack[5] = rotation.w
		*/

		//it reduces to zero the accountant meaning that answer of the server exists to this moment
		contTimes = 0;

		if (networkPlayers [data.pack [1]] != null) {
			PlayerManager netPlayer = networkPlayers [data.pack [1]];
			netPlayer.timeOut = 0f;
			Vector4 rot = new Vector4 (
				             float.Parse (data.pack [2]), float.Parse (data.pack [3]), float.Parse (data.pack [4]),
				             float.Parse (data.pack [5]));// atualiza a posicao
			netPlayer.UpdateRotation (new Quaternion (rot.x, rot.y, rot.z, rot.w));
		}
	


	}

	/// <summary>
	/// Emits the local player rotation to Server.js.
	/// </summary>
	/// <param name="_rot">Rot.</param>
	//responsible method for transmitting to the server the movement of the player associated to this client
	public void EmitRotation(Quaternion _rot)
	{
		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();


		data["callback_name"] = "ROTATE";

		data["local_player_id"] = myPlayer.GetComponent<PlayerManager>().id;

		data["rotation"] =  _rot.x+","+_rot.y+","+_rot.z+","+_rot.w;


		//send the position point to server
		string msg = data["local_player_id"]+","+data["rotation"] ;

		//sends to the server through socket UDP the jo package 
		udpClient.Emit (data["callback_name"] ,msg);


	}



	/// <summary>
	/// Emits the local player attack to server.
	/// </summary>
	/// <param name="callback_name">Callback name.</param>
	/// <param name="_data">Data.</param>
	public void EmitAttack(string callback_name,string _data)
	{

		//sends to the server through socket UDP the _data package 
		udpClient.Emit (callback_name ,_data);



	}

	/// <summary>
	/// Update the network player attack to local player.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnUpdateAttack(SocketUDPEvent data)
	{
		/*
		 * data.pack[0] = UPDATE_ATACK
		 * data.pack[1] = id (network player id)

		*/

		if (networkPlayers [data.pack [1]] != null)
		{
			PlayerManager netPlayer = networkPlayers[data.pack[1]];

			netPlayer.UpdateAnimator ("IsAtack");

		}


	}
		

	/// <summary>
	/// Emits the local player phisicst damage to server.
	/// </summary>
	/// <param name="_shooterId">Shooter identifier.</param>
	/// <param name="_targetId">Target identifier.</param>
	public void EmitPhisicstDamage(string _shooterId, string _targetId)
	{

		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data ["shooterId"] = _shooterId;
		data ["targetId"] = _targetId;

		//join info
		string msg = data ["shooterId"]+","+data ["targetId"];

		//sends to the server through socket UDP the jo package 
		udpClient.Emit ("PHISICS_DAMAGE" ,msg );

	}


	//it updates the suffered damage to local player
	void OnUpdatePlayerPhisicsDamage (SocketUDPEvent data)
	{

		/*
		 * data.pack[0] = UPDATE_PHISICS_DAMAGE
		 * data.pack[1] = attacker.id or shooter.id (network player id)
		 * data.pack[2] = target.id (network player id)
		 * data.pack[3] = target.health
		 */



		if (networkPlayers [data.pack [2]] != null) 
		{

			PlayerManager PlayerTarget = networkPlayers[data.pack [2]];
			PlayerTarget. GetComponent<PlayerHealth> ().TakeDamage ();


			if (PlayerTarget.isLocalPlayer)// if i am target
			{
				CanvasManager.instance.healthSlider.value = float.Parse(data.pack [3]);
				CanvasManager.instance.txtHealth.text = "HP " + data.pack [3] + " / " 
					+ PlayerTarget. GetComponent<PlayerHealth> ().maxHealth;

			}



		}

		if (networkPlayers [data.pack [1]] != null) 
		{

			PlayerManager PlayerShooter = networkPlayers[data.pack [1]];

		}


	}

    // 보스한테서 자기 자신의 어그로를 받아서 서버한테 자기 어그로를 보내줌
    public void EmitAggro(float aggro)
    {
        //hash table <key, value>
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["callback_name"] = "AGGRO";

        data["my_index"] = my_index.ToString();

        data["aggro"] = aggro.ToString();

        //Debug.Log(my_index + " " + aggro);

        string msg = data["my_index"] + "," + data["aggro"];

        udpClient.Emit(data["callback_name"], msg);
    }

    //it updates the suffered damage to local player
    void OnUpdateAggro(SocketUDPEvent data)
    {


        /*
		 * data.pack[0] = Update_Aggro     
		 * data.pack[1] = attacker index (network player id)
		 */
        BossStatus.instance.target_player = BossStatus.instance.player[int.Parse(data.pack[1])];
        Debug.Log(BossStatus.instance.target_player.name);
    }

    /// <summary>
    /// Emits the local player animation to Server.js.
    /// </summary>
    /// <param name="_animation">Animation.</param>
    public void EmitAnimation(string _animation)
	{
		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		//JSON package
		data["callback_name"] = "ANIMATION";//preenche com o id da callback receptora que está no servidor

		data["local_player_id"] = myPlayer.GetComponent<PlayerManager>().id;

		data ["animation"] = _animation;

		//send the position point to server
		string msg = data["local_player_id"]+","+data ["animation"];

		//sends to the server through socket UDP the jo package 
		udpClient.Emit (data["callback_name"] ,msg);


	}

	/// <summary>
	///  Update the network player animation to local player.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnUpdateAnim(SocketUDPEvent data)
	{
		/*
		 * data.pack[0] = UPDATE_PLAYER_ANIMATOR
		 * data.pack[1] = id (network player id)
		 * data.pack[2] = animation (network player animation)
		*/

		contTimes = 0;

		//find network player by your id
		PlayerManager netPlayer = networkPlayers[data.pack[1]];
		netPlayer.timeOut = 0f;
		//updates current animation
		netPlayer.UpdateAnimator(data.pack[2]);

	}


	public void GameOver()
	{
        //hash table <key, value>
        Dictionary<string, string> data = new Dictionary<string, string>();

		//JSON package
		data["callback_name"] = "disconnect";

		data ["local_player_id"] = local_player_id;

		if (UDPServer.instance.serverRunning) {

			data ["isMasterServer"] = "true";
		}
		else 
		{
			data ["isMasterServer"] = "false";
		}
				
		//send the position point to server
		string msg = data["local_player_id"]+","+data ["isMasterServer"];

		//Debug.Log ("emit disconnect");

		//we make four attempts of similar sending of preventing the loss of packages
		udpClient.Emit (data["callback_name"] ,msg);
        /*
		udpClient.Emit (data["callback_name"] ,msg);
        
		udpClient.Emit (data["callback_name"] ,msg);

		udpClient.Emit (data["callback_name"] ,msg);
        */

        if (udpClient != null) {

			udpClient.disconnect ();
        }
        is_ConnectAgain = true;
        my_index = -1;
        if (is_host)
        {
            RestartGame();
            UDPServer.instance.CloseServer();
        }
    }


    /// <summary>
    /// inform the local player to destroy offline network player
    /// </summary>
    /// <param name="_msg">Message.</param>
    //desconnect network player
    void OnUserDisconnected(SocketUDPEvent data )
	{

		/*
		 * data.pack[0]  = USER_DISCONNECTED
		 * data.pack[1] = id (network player id)
		 * data.pack[2] = isMasterServer
		 * data.pack[3] = playernum
		 * data.pack[4] = player index
		*/
		Debug.Log ("disconnect!");

		if (bool.Parse (data.pack [2])) {			
			RestartGame ();
		}
		else
		{
            Player_num = int.Parse(data.pack[3]);
            players_name_text[int.Parse(data.pack[4])].text = "";
            current_player_count.text = Player_num.ToString();

    //        if (networkPlayers [data.pack [1]] != null) {
				//	//destroy network player by your id
				//	Destroy (networkPlayers [data.pack [1]].gameObject);

				//	//remove from the dictionary
				//	networkPlayers.Remove (data.pack [1]);
				//}
		}
	}
    void OnUserReadyCount(SocketUDPEvent data)
    {

        /*
		 * data.pack[0]  = readycount
		 * data.pack[1] = readycount
		*/
        //Debug.Log(Player_num);
        //Debug.Log(int.Parse(data.pack[1]));

        if (int.Parse(data.pack[1])>=Player_num&&Player_num>=2) //start 가능
        {
           
            //hash table <key, value>
            Dictionary<string, string> datasend = new Dictionary<string, string>();

            //JSON package

            datasend["callback_name"] = "GameStart";
           


            datasend["Start"] = "Start";
            //send the position point to server
            string msg = datasend["Start"];

            Debug.Log ("gamestart");



            //we make four attempts of similar sending of preventing the loss of packages
            udpClient.Emit(datasend["callback_name"], msg);

            udpClient.Emit(datasend["callback_name"], msg);

            udpClient.Emit(datasend["callback_name"], msg);

            udpClient.Emit(datasend["callback_name"], msg);
            

            if (udpClient != null)
            {
                udpClient.disconnect();
            }


            //titlebutton.StartGame();
        }
       

    }
    void OnUserGameStart(SocketUDPEvent data)
    {
        /*
		 * data.pack[0]  = readycount
		 * data.pack[1] = readycount
		*/
        //Debug.Log(Player_num);
        //Debug.Log(int.Parse(data.pack[1]));
        if(my_index==-1)
        {
            return;
        }
        titlebutton.StartGame();
    }
    public void RestartGame()
	{
		//CanvasManager.instance.txtSearchServerStatus.text = "PLEASE START SERVER";

		Destroy (camRig.gameObject);
		foreach(KeyValuePair<string, PlayerManager> entry in networkPlayers)
		{
			if (networkPlayers [entry.Key] != null) {
				Destroy (networkPlayers [entry.Key].gameObject);
			}
		}

		networkPlayers.Clear ();

		gameIsRunning = false;

		serverFound = false;

		myId = string.Empty;

		//CanvasManager.instance.OpenScreen (0);

	}



	public void OnApplicationQuit() {

		Debug.Log("Application ending after " + Time.time + " seconds");

		GameOver ();
			
	}
    public void ReadyOrStart()
    {
            //레디
            //hash table <key, value>	
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["callback_name"] = "Ready";//preenche com o id da callback receptora que está no servidor

            Debug.Log("Ready");


            if(is_ready)
            {
                data["Ready"] = "0";
                is_ready = false;
            }
            else
            {
                data["Ready"] = "1"; //1이 ready
                is_ready = true;
            }
            data["Player_Id"] = myId;


            string msg = data["Ready"]+','+data["Player_Id"];

            //sends to the server through socket UDP the jo package 
            udpClient.Emit(data["callback_name"], msg);

            //we waited for a time to verify the connection
            //ConnectToUDPServer(serverPort, clientPort);
            //체크되면 start가능


            //레디 취소
    }
    public void OnUserBossSlected(SocketUDPEvent data)
    {
        /*
		 * data.pack[0]  = bossslected
		 * data.pack[1] = boss index
		*/

        boss_index = int.Parse(data.pack[1]);
        level_index = int.Parse(data.pack[2]);
    }

    public void GameReady()
    {
        //hash table <key, value>	
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["callback_name"] = "GameReady";//preenche com o id da callback receptora que está no servidor

        string msg = "";

        udpClient.Emit(data["callback_name"], msg);

        //we waited for a time to verify the connection
    }
    public void OnUserServerClosed(SocketUDPEvent data)
    {
        /*
		 * data.pack[0]  = ServerClose
		*/
        //서버 닫혔으니 그냥 끄면 됨 다른애들은
        if(!is_host)
        {
            titlebutton.multiplay_lobby_panel.SetActive(false);
        }
    }
    public void OnUserSceneStart(SocketUDPEvent data)
    {
        /*
		 * data.pack[0]  = SceneStart
		*/
        //레디
        //hash table <key, value>	
        Dictionary<string, string> datasend = new Dictionary<string, string>();
        datasend["callback_name"] = "SendCharacterIndex";//preenche com o id da callback receptora que está no servidor

        datasend["PlayerIndex"] = my_index.ToString();
        datasend["Character_first"] = char_index[my_index,0].ToString();
        datasend["Character_second"] = char_index[my_index,1].ToString();

        string msg = datasend["PlayerIndex"] + ',' + datasend["Character_first"] +',' + datasend["Character_second"];

        //sends to the server through socket UDP the jo package 
        udpClient.Emit(datasend["callback_name"], msg);

        //we waited for a time to verify the connection
        //ConnectToUDPServer(serverPort, clientPort);
        //체크되면 start가능
        //레디 취소

        EveryOneGameReady = true;
    }
    public void OnUserEveryChar(SocketUDPEvent data)
    {
        /*
		 * data.pack[0]  = everychar
		 * data.pack[1] = index
		 * data.pack[2] = index first
		 * data.pack[3] = index sec
		*/
        char_index[int.Parse(data.pack[1]), 0] = int.Parse(data.pack[2]);
        char_index[int.Parse(data.pack[1]), 1] = int.Parse(data.pack[3]);
    }
    public void OnUserMaterialMake(SocketUDPEvent data)
    {
        /*
		* data.pack[0]= CALLBACK_NAME: "MaterialMake"
        * data.pack[1]= Materialbefore;
        * data.pack[2]= Materialafter
        * data.pack[3]= index
        * data.pack[4]= timer
		*/
        if (my_index == int.Parse(data.pack[3])) return;

        StartCoroutine(AfterMaterialChange(data.pack[2],float.Parse(data.pack[4]),int.Parse(data.pack[3]),data.pack[1]));
    }
    IEnumerator AfterMaterialChange(string After,float timer,int index,string Before)
    {
        GameManage.instance.Character_other[index].GetComponent<SpriteRenderer>().material = Resources.Load(After) as Material;
        
        yield return new WaitForSeconds(timer);

        GameManage.instance.Character_other[index].GetComponent<SpriteRenderer>().material = Resources.Load(Before) as Material;
    }


    public void OnUserHealMake(SocketUDPEvent data)
    {
        /*
		 * data.pack[0]  = HealMake
		 * data.pack[1] = isHeal
		 * data.pack[2] = posx
		 * data.pack[3] = posy
		 * data.pack[4] = index
		 * data.pack[5] = Heal
		*/
        if (my_index == int.Parse(data.pack[4])) return;

        Vector2 pos = new Vector2(float.Parse(data.pack[2]), float.Parse(data.pack[3]));

        if(bool.Parse(data.pack[1]))
        {
            GameUI.instance.FloatingGreenDamage(pos, float.Parse(data.pack[5]));
        }            
        else
        {
            GameUI.instance.FloatingBlueDamage(pos, float.Parse(data.pack[5]));
        }
    }
    public void OnUserSkillMake(SocketUDPEvent data)
    {
        /*
		 * data.pack[0]  = SkillMake
		 * data.pack[1] = skill manager num
		 * data.pack[2] = posx
		 * data.pack[3] = posy
		 * data.pack[4] = posz
		 * data.pack[5] = rotx
		 * data.pack[6] = roty
		 * data.pack[7] = rotz
		 * data.pack[8] = index
		 * data.pack[9] = istrail
		 * data.pack[10] = scale
		 * data.pack[11] = timer
		 * data.pack[12] = skillpos x
		 * data.pack[13] = skillpos y
		 * data.pack[14] = rotw
		 * data.pack[15] = is_small
		*/



        if (my_index == int.Parse(data.pack[8])) return;

        Vector3 pos = new Vector3(float.Parse(data.pack[2]), float.Parse(data.pack[3]), int.Parse(data.pack[4]));


        Quaternion rot = new Quaternion(float.Parse(data.pack[5]), float.Parse(data.pack[6]), float.Parse(data.pack[7]), float.Parse(data.pack[14]));

        Skill_pos[int.Parse(data.pack[8])] = new Vector2(float.Parse(data.pack[12]), float.Parse(data.pack[13]));

        GameObject skill_temp = Instantiate(SkillManager.instance.skill_prefab[int.Parse(data.pack[1])], pos, rot);

        if (bool.Parse(data.pack[15]))
        {
            skill_temp.transform.localScale = new Vector3(float.Parse(data.pack[10])/2.5f, float.Parse(data.pack[10]) / 2.5f, float.Parse(data.pack[10]) / 2.5f);
        }
        else
        {
            skill_temp.transform.localScale = new Vector3(float.Parse(data.pack[10]), float.Parse(data.pack[10]), float.Parse(data.pack[10]));
        }
        Destroy(skill_temp, float.Parse(data.pack[11]));
        if(bool.Parse(data.pack[9]))
        {
            skill_temp.transform.SetParent(GameManage.instance.Character_other[int.Parse(data.pack[8])].transform);
        }
    }
    public void OnUserBuffMake(SocketUDPEvent data)
    {
        /*
		 * data.pack[0]  = SkillMake
		 * data.pack[1] = skill manager num
		 * data.pack[2] = index
		*/
        GameObject[] skill_temp;
        skill_temp = new GameObject[4];

        for (int i=0;i<Player_num;i++) //더미먼저 씌워주고
        {
            if (i != my_index) //내가 아니면 생성
            {
                skill_temp[i] = Instantiate(SkillManager.instance.skill_prefab[int.Parse(data.pack[1])], GameManage.instance.Character_other[i].transform);
            }
        }
        if (my_index == int.Parse(data.pack[2]))
        {
            return; //내가 해머라면 로컬로 이미 씌웠으므로 패스
        }
        skill_temp[my_index] = Instantiate(SkillManager.instance.skill_prefab[int.Parse(data.pack[1])], GameManage.instance.player[0].transform); //나한테 씌우기
    }
    public void OnUserClearObject(SocketUDPEvent data)
    {
        /*
		 * data.pack[0]  = SkillMake
		 * data.pack[1] = skill num  ( 0: Mage2   1:shield)
		 * data.pack[2] = index
		*/
        if (my_index == int.Parse(data.pack[2])) return; //내가 메이지라면 로컬로 이미 지웠으므로

        switch (int.Parse(data.pack[1]))
        {
            case 0:
                if (GameManage.instance.Character_other[int.Parse(data.pack[2])].GetComponentInChildren<MageSkill2>() != null)
                {
                    Destroy(GameManage.instance.Character_other[int.Parse(data.pack[2])].GetComponentInChildren<MageSkill2>().gameObject);
                }
                break;
            case 1:
                if (GameManage.instance.Character_other[int.Parse(data.pack[2])].GetComponentInChildren<ShieldSkill4>() != null)
                {
                    Destroy(GameManage.instance.Character_other[int.Parse(data.pack[2])].GetComponentInChildren<ShieldSkill4>().gameObject);
                }
                break;
        }
    }
    public void OnUserOnePerosnDead(SocketUDPEvent data)
    {
        /*
		 * data.pack[0] = SkillMake
		 * data.pack[1] = index
		 * data.pack[2] = isalldead
		*/
        is_dead[int.Parse(data.pack[1])] = true;

        is_allDead = bool.Parse(data.pack[2]);
        if(is_allDead)
        {
            gameIsRunning = false;
            GameManage.instance.GetTargetPlayer(this.gameObject);
            GameManage.instance.Multi_Boss_Win_Game();
        }
    }
    public void OnUserBossSkillMake(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "Boss_Skill_Instance"
         * data.pack[1]= boss_idex;
         * data.pack[2]= skill_index
         * data.pack[3]= posx
         * data.pack[4]= posy
         * data.pack[5]= posz
         * data.pack[6]= rotx
         * data.pack[7]= roty
         * data.pack[8]= rotz
         * data.pack[9]= rotw
         * data.pack[10]= scalex
         * data.pack[11]= scaley
         * data.pack[12]= scalez
         * data.pack[13]= timer
         * data.pack[14]= is_warning
         * data.pack[15]= is_parent
        */
        if (is_host) return;

        Vector3 pos = new Vector3(float.Parse(data.pack[3]), float.Parse(data.pack[4]), float.Parse(data.pack[5]));
        Quaternion rot = new Quaternion(float.Parse(data.pack[6]), float.Parse(data.pack[7]), float.Parse(data.pack[8]), float.Parse(data.pack[9]));
        Vector3 scale = new Vector3(float.Parse(data.pack[10]), float.Parse(data.pack[11]), float.Parse(data.pack[12]));
        Debug.Log(int.Parse(data.pack[1]));
        switch (int.Parse(data.pack[1]))
        {
            case 0:
                if (bool.Parse(data.pack[15]))
                {
                    GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<DesertBossMove_koki>().skill_prefab[int.Parse(data.pack[2])], BossStatus.instance.transform);
                    skill_prefab.transform.localScale = scale;
                    skill_prefab.transform.localRotation = rot;
                    if (bool.Parse(data.pack[14]))
                        Destroy(skill_prefab, float.Parse(data.pack[13]));
                }
                else
                {
                    GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<DesertBossMove_koki>().skill_prefab[int.Parse(data.pack[2])], pos,rot);
                    skill_prefab.transform.localScale = scale;
                    if (bool.Parse(data.pack[14]))
                        Destroy(skill_prefab, float.Parse(data.pack[13]));
                }
                break;
            case 1:
                if (bool.Parse(data.pack[15]))
                {
                    GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss2Move>().skill_prefab[int.Parse(data.pack[2])], BossStatus.instance.transform);
                    skill_prefab.transform.localScale = scale;
                    skill_prefab.transform.localRotation = rot;
                    if (bool.Parse(data.pack[14]))
                        Destroy(skill_prefab, float.Parse(data.pack[13]));
                }
                else
                {
                    GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss2Move>().skill_prefab[int.Parse(data.pack[2])], pos, rot);
                    if(int.Parse(data.pack[2]) == 15)
                    {
                        ps = skill_prefab.GetComponent<ParticleSystem>();
                        var simulate = ps.main;
                        simulate.simulationSpeed = (float)1 / 4f;
                    }
                    if (int.Parse(data.pack[2]) == 2)
                    {
                        Boss2_NormalAttack_Obj.Add(skill_prefab);
                    }
                    skill_prefab.transform.localScale = scale;
                    if (bool.Parse(data.pack[14]))
                        Destroy(skill_prefab, float.Parse(data.pack[13]));
                }
                break;
            case 2:
                if (bool.Parse(data.pack[15]))
                {
                    GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss3Move>().skill_prefab[int.Parse(data.pack[2])], BossStatus.instance.transform);
                    skill_prefab.transform.localScale = scale;
                    skill_prefab.transform.localRotation = rot;
                    if (bool.Parse(data.pack[14]))
                        Destroy(skill_prefab, float.Parse(data.pack[13]));
                }
                else
                {
                    GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss3Move>().skill_prefab[int.Parse(data.pack[2])], pos, rot);
                    skill_prefab.transform.localScale = scale;
                    if(int.Parse(data.pack[2]) == 3)
                    {
                        skill_prefab.GetComponent<Boss3_NormalAttack>().GetDir(BossStatus.instance.target_player.transform.position);
                    }
                    if (bool.Parse(data.pack[14]))
                        Destroy(skill_prefab, float.Parse(data.pack[13]));
                }
                break;
            case 3:
                if (bool.Parse(data.pack[15]))
                {
                    GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss4Move>().skill_prefab[int.Parse(data.pack[2])], BossStatus.instance.transform);
                    skill_prefab.transform.localScale = scale;
                    skill_prefab.transform.localRotation = rot;

                    if (bool.Parse(data.pack[14]))
                        Destroy(skill_prefab, float.Parse(data.pack[13]));
                }
                else
                {
                    GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss4Move>().skill_prefab[int.Parse(data.pack[2])], pos, rot);
                    skill_prefab.transform.localScale = scale;
                    if (int.Parse(data.pack[2]) == 4)
                    {
                        skill_prefab.GetComponent<Boss4_Bomb_Together>().GetDestination(new Vector3(pos.x, pos.y - 5, pos.z));
                    }
                    if (bool.Parse(data.pack[14]))
                        Destroy(skill_prefab, float.Parse(data.pack[13]));
                }
                break;
            case 4:
                if (bool.Parse(data.pack[15]))
                {
                    GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss5Move>().skill_prefab[int.Parse(data.pack[2])], BossStatus.instance.transform);
                    skill_prefab.transform.localScale = scale;
                    skill_prefab.transform.localRotation = rot;

                    if (bool.Parse(data.pack[14]))
                        Destroy(skill_prefab, float.Parse(data.pack[13]));
                }
                else
                {
                    GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss5Move>().skill_prefab[int.Parse(data.pack[2])], pos, rot);
                    skill_prefab.transform.localScale = scale;
                    if (bool.Parse(data.pack[14]))
                        Destroy(skill_prefab, float.Parse(data.pack[13]));
                }
                break;
            case 5:
                if (bool.Parse(data.pack[15]))
                {
                    GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss6Move>().skill_prefab[int.Parse(data.pack[2])], BossStatus.instance.transform);
                    skill_prefab.transform.localScale = scale;
                    skill_prefab.transform.localRotation = rot;

                    if (bool.Parse(data.pack[14]))
                        Destroy(skill_prefab, float.Parse(data.pack[13]));
                }
                else
                {
                    GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss6Move>().skill_prefab[int.Parse(data.pack[2])], pos, rot);
                    skill_prefab.transform.localScale = scale;
                    if (bool.Parse(data.pack[14]))
                        Destroy(skill_prefab, float.Parse(data.pack[13]));
                }
                break;
            case 6:
                if (bool.Parse(data.pack[15]))
                {
                    GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss7Move>().skill_prefab[int.Parse(data.pack[2])], BossStatus.instance.transform);
                    skill_prefab.transform.localScale = scale;
                    skill_prefab.transform.localRotation = rot;

                    if (bool.Parse(data.pack[14]))
                        Destroy(skill_prefab, float.Parse(data.pack[13]));
                }
                else
                {
                    GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss7Move>().skill_prefab[int.Parse(data.pack[2])], pos, rot);
                    skill_prefab.transform.localScale = scale;
                    if (bool.Parse(data.pack[14]))
                        Destroy(skill_prefab, float.Parse(data.pack[13]));
                }
                break;
        }

    }
    public void OnUserSwitchPlayer(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "SwitchPlayer"
         * data.pack[1]= index
         * data.pack[2]= char_num
        */

        GameManage.instance.Character_other[int.Parse(data.pack[1])].GetComponent<CharacterStat>().ChangeSprite(int.Parse(data.pack[2]));
    }

    public void OnUserBossGetSilent(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "UPDATE_Boss_Get_Silent"
         * data.pack[1]= flag
        */

        bool flag = bool.Parse(data.pack[1]);

        if (flag)
        {
            BossStatus.instance.GetComponent<SpriteRenderer>().material = Resources.Load("Material/ColorRed") as Material;
        }
        else
        {
            BossStatus.instance.GetComponent<SpriteRenderer>().material = Resources.Load("Material/AlphaIntensity") as Material;
        }
    }

    public void OnUserBossGetBuff(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "UPDATE_Boss_Get_Buff"
         * data.pack[1]= buff_index;
        */

        int buff_index = int.Parse(data.pack[1]);

        BossStatus.instance.MultiGetBuff(buff_index);
    }

    public void OnUserBossGetBuffPlayer(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "UPDATE_Boss_Get_Buff_Player"
         * data.pack[1]= buff_index;
         * data.pack[2]= player_index;
        */
        int buff_index = int.Parse(data.pack[1]);
        int player_index = int.Parse(data.pack[2]);

        BossStatus.instance.MultiGetBuff(buff_index, BossStatus.instance.player[player_index]);
    }

    public void OnUserBossDeleteBuff(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "UPDATE_Boss_Delete_Buff"
         * data.pack[1]= buff_index;
        */
        int buff_index = int.Parse(data.pack[1]);

        BossStatus.instance.MultiBossBuffDelete(buff_index);
    }

    public void OnUserBossTargetSkillMake(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "BOSS_SKILL_MAKE"
         * data.pack[1]= boss_idex;
         * data.pack[2]= skill_index
         * data.pack[3]= character_index
         * data.pack[4]= timer
         * data.pack[5]= is_warning
        */
        if (is_host) return;
        GameObject skill_prefab;
        int index = int.Parse(data.pack[3]);
        //Debug.Log(index);
        switch (int.Parse(data.pack[1]))
        {
            case 0:
                skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<DesertBossMove_koki>().skill_prefab[int.Parse(data.pack[2])], BossStatus.instance.player[index].transform);
                if (bool.Parse(data.pack[5]))
                    Destroy(skill_prefab, float.Parse(data.pack[4]));
                break;
            case 2:
                skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss3Move>().skill_prefab[int.Parse(data.pack[2])], BossStatus.instance.player[index].transform);

                if (int.Parse(data.pack[2]) == 1)
                {
                    ps = skill_prefab.GetComponent<ParticleSystem>();
                    var simulate = ps.main;
                    simulate.simulationSpeed = (float)1 / 6f / (UpLoadData.boss_level + 4);
                    Destroy(skill_prefab, float.Parse(data.pack[4]));
                }
                break;
            case 3:
                skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss4Move>().skill_prefab[int.Parse(data.pack[2])], BossStatus.instance.player[index].transform);

                if (int.Parse(data.pack[2]) == 3)
                {
                    Destroy(skill_prefab, float.Parse(data.pack[4]));
                }
                break;
            case 4:
                skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss5Move>().skill_prefab[int.Parse(data.pack[2])], BossStatus.instance.player[index].transform);

                if (int.Parse(data.pack[2]) == 11 || int.Parse(data.pack[2]) == 16)
                {
                    Destroy(skill_prefab, float.Parse(data.pack[4]));
                }
                if (int.Parse(data.pack[2]) == 7)
                {
                    skill_prefab.GetComponent<Boss5_RotateMissile>().GetTargetPosition(BossStatus.instance.player[index]);
                }
                if (int.Parse(data.pack[2]) == 5)
                {
                    skill_prefab.GetComponent<SpriteRenderer>().enabled = false;
                }
                if (int.Parse(data.pack[2]) == 10)
                {
                    skill_prefab.GetComponent<Boss5_Flare_Light>().GetTartget(BossStatus.instance.player[index]);
                }
                break;
        }

    }

    public void OnUserDamageAccumulate(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "UPDATE_DAMAGE_ACCUMULATE"
         *  접속된 플레이어의 수 * 2만큼의
         *  데미지가 들어옴
        */
        //if (is_host) return;
        //Debug.Log((data.pack[0] + " " + (data.pack[1]) + " " + (data.pack[2])));
        for (int i = 0; i <Player_num; i++)
        {
            for(int j = 0; j < 2; j++)
            {
                GameManage.instance.Multi_Damage_toBoss[i, j] = float.Parse(data.pack[i * 2 + j + 1]);
            }
        }
    }

    public void OnUserDamageAccumulateFromBoss(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "UPDATE_DAMAGE_ACCUMULATE"
         *  접속된 플레이어의 수 * 2만큼의
         *  데미지가 들어옴
        */
        //if (is_host) return;
        //Debug.Log((data.pack[0] + " " + (data.pack[1])));
        Debug.Log("Here");
        for (int i = 0; i < Player_num; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                GameManage.instance.Multi_Damage_fromBoss[i, j] = float.Parse(data.pack[i * 2 + j + 1]);
                //Debug.Log("DamageFromBoss" + "player" + i + GameManage.instance.Multi_Damage_fromBoss[i, j]);
            }
        }
    }

    public void OnUserHealAccumulate(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "UPDATE_DAMAGE_ACCUMULATE"
         *  접속된 플레이어의 수 * 2만큼의
         *  힐량이 들어옴
        */
        //if (is_host) return;
        //Debug.Log((data.pack[0] + " " + (data.pack[1])));
        for (int i = 0; i < Player_num; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                GameManage.instance.Multi_Heal[i, j] = float.Parse(data.pack[i * 2 + j + 1]);
                //Debug.Log(i + " " + j);
                //Debug.Log("Heal" +"player" + i +GameManage.instance.Multi_Heal[i, j]);
            }
        }
    }

    public void OnUserBoss2ULT(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "UPDATE_Boss2_ULT"
        */
        if (is_host) return;

        int index = int.Parse(data.pack[1]);

        if (Boss2_NormalAttack_Obj[index] == null)
            return;
        else
        {
            Boss2_NormalAttack_Obj[index].GetComponent<Boss2_NormalAttack_Move>().enabled = true;
        }
   
    }

    public void OnUserBoss2Fire(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "UPDATE_Boss2_Fire"
         * data.pack[1]= posx
         * data.pack[2]= posy
        */
        if (is_host) return;
        BossStatus.instance.gameObject.GetComponent<Boss2Move>().skill_prefab[13].SetActive(true);
        BossStatus.instance.gameObject.GetComponent<Boss2Move>().skill_prefab[13].GetComponent<Boss2_Fire>().StartCor();
        BossStatus.instance.gameObject.GetComponent<Boss2Move>().skill_prefab[13].transform.position = new Vector2(float.Parse(data.pack[1]), float.Parse(data.pack[2]));
        BossStatus.instance.gameObject.GetComponent<Boss2Move>().skill_prefab[13].GetComponent<Monster>().MonsterHpbarSetActive();
    }

    public void OnUserBoss2SameDir(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "UPDATE_Boss2_SameDir"
        */
        if (is_host) return;
        GameObject[] Boss2_Normal_Attacks = GameObject.FindGameObjectsWithTag("Boss2NormalAttack");

        for (int i = 0; i < Boss2_Normal_Attacks.Length; i++)
        {
            Destroy(Boss2_Normal_Attacks[i], 0.2f);
        }
    }

    public void OnUserBoss3Pile(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "Boss3_Pile_Use"
         * data.pack[1]= skill_index
         * data.pack[2]= target_player_index
         * data.pack[3]= ranposx
         * data.pack[4]= ranposy
         * data.pack[5]= ranposz
        */
        if (is_host) return;

        int skill_index = int.Parse(data.pack[1]);
        int player_index = int.Parse(data.pack[2]);
        Vector3 ran_pos = new Vector3(float.Parse(data.pack[3]), float.Parse(data.pack[4]), float.Parse(data.pack[5]));

        GameObject skill_prefab = BossStatus.instance.gameObject.GetComponent<Boss3Move>().skill_prefab[skill_index];

        skill_prefab.SetActive(true);
        skill_prefab.GetComponent<Boss3_Pile>().GetPlayer(BossStatus.instance.player[player_index]);
        skill_prefab.GetComponent<Monster>().MonsterHpbarSetActive();
        skill_prefab.GetComponent<Boss3_Pile>().StartCoturines();
        skill_prefab.transform.position = BossStatus.instance.player[player_index].transform.position + ran_pos;
    }

    public void OnUserBoss4RollingRock(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "UPDATE_Boss4_RollingRock"
         * data.pack[1]= skill_index
         * data.pack[2]= ranposx
         * data.pack[3]= ranposy
        */

        if (is_host) return;

        int skill_index = int.Parse(data.pack[1]);
        Vector2 ran_pos = new Vector2(float.Parse(data.pack[2]), float.Parse(data.pack[3]));

        GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss4Move>().skill_prefab[skill_index], ran_pos, Quaternion.identity);
        skill_prefab.transform.position = ran_pos;
        skill_prefab.GetComponent<Monster>().MonsterHpbarSetActive();
    }

    public void OnUserBoss6Bubble(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "UPDATE_Boss6_Bubble"
         * data.pack[1]= skill_index
         * data.pack[2]= dirx
         * data.pack[3]= diry
         * data.pack[4]= force
        */

        if (is_host) return;

        int skill_index = int.Parse(data.pack[1]);
        Vector2 dir = new Vector2(float.Parse(data.pack[2]), float.Parse(data.pack[3]));
        float force = float.Parse(data.pack[4]);

        GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss6Move>().skill_prefab[skill_index], Vector3.zero, Quaternion.identity);
        skill_prefab.GetComponent<Rigidbody2D>().AddForce(dir * force, ForceMode2D.Force);
    }

    public void OnUserBoss6Volcano(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "UPDATE_Boss6_Volcano"
        */

        if (is_host) return;
        GameObject volcano = GameObject.Find("Volcatno");
        GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss6Move>().skill_prefab[9], volcano.transform);
    }

    public void OnUserBoss7Sickle(SocketUDPEvent data)
    {
        /*
         * data.pack[0]= CALLBACK_NAME: "UPDATE_Boss6_Volcano"
         * data.pack[1]= dirx
         * data.pack[2]= diry
        */

        if (is_host) return;
        Vector2 dir = new Vector2(float.Parse(data.pack[2]), float.Parse(data.pack[3]));

        GameObject skill_prefab = Instantiate(BossStatus.instance.gameObject.GetComponent<Boss7Move>().skill_prefab[10], BossStatus.instance.transform.position, Quaternion.identity);
        skill_prefab.GetComponent<Boss7_SicleRotate>().GetDir(dir);
    }
}
                