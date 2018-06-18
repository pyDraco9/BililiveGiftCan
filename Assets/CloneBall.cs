using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using BiliDMLib;
using BilibiliDM_PluginFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CloneBall : MonoBehaviour
{
	public Queue queue;
    public GameObject Ball;
    public Text text_id;
    public Text text_count;
	public Text text_price;
    private Sprite[] SpriteArr = new Sprite[999];
	public bool bomb = false;
	public int count = 0;
	public int roomId = 12720;
	public string ChatHost = "broadcastlv.chat.bilibili.com";
	public int ChatPort = 2243;
	public string penquanCommand = ">喷泉";
	private string[] giftListData = new string[99999];
	public string giftListUrl = "https://api.live.bilibili.com/gift/v3/live/gift_config";
	private DanmakuModel tmp = new DanmakuModel();

    GameObject[] obj;
    // Use this for initialization
	IEnumerator Start()
    {
		tmp.giftId = "1";
		tmp.GiftName = "人造辣条";
		tmp.GiftCount = 1;
		tmp.giftPrice = 100;
		tmp.MsgType = MsgTypeEnum.GiftSend;

		Screen.SetResolution(900, 900, false);
		queue = new Queue(); 
		GameObject BottlePIC = GameObject.Find ("BottlePIC");
		BottlePIC.GetComponent<Renderer> ().sortingOrder = 12451;

		WWW www = new WWW(giftListUrl);
		yield return www;
		if (www != null && string.IsNullOrEmpty(www.error))
		{
			var obj = JObject.Parse(www.text);
			foreach (var data in obj ["data"])
			{
				giftListData [int.Parse(data["id"].ToString())] = data["img_basic"].ToString();
				//UnityEngine.Debug.Log (giftListData [int.Parse(data["id"].ToString())]);
			}

		}
    }

	public void humanGift(){
		queue.Enqueue(tmp);
	}

	public void ContralPenquan(bool contral)
	{
		GameObject Cube_left_bomb = GameObject.Find ("Cube_left_bomb");
		GameObject Cube_right_bomb = GameObject.Find ("Cube_right_bomb");

		Cube_left_bomb.GetComponent<BoxCollider2D>().usedByEffector = contral;
		Cube_right_bomb.GetComponent<BoxCollider2D>().usedByEffector = contral;
		Cube_left_bomb.GetComponent<Rigidbody2D>().simulated = contral;
		Cube_right_bomb.GetComponent<Rigidbody2D>().simulated = contral;
		bomb = contral;
	}

    // Update is called once per frame
    void Update()
    {
		try
		{
			obj = FindObjectsOfType(typeof(GameObject)) as GameObject[];
			if (obj.Length < 500 && bomb == true) {
				ContralPenquan(false);
			}
			int i = 0;
			foreach (GameObject child in obj)
			{
				i++;
				if (child.gameObject.transform.position.y < -20)
				{
					child.gameObject.SetActive(false);
					Destroy(child.gameObject);
				}
			}

			if (queue.Count > 0)
			{
				DanmakuModel dama = (DanmakuModel)queue.Dequeue();
				switch(dama.MsgType)
				{
				case MsgTypeEnum.GiftSend:
					{
						for (i = 1; i <= dama.GiftCount; i++)
						{
							addGift(dama);
						}
						break;
					}
				case MsgTypeEnum.Comment:
					{
						if(dama.isAdmin && dama.CommentText == penquanCommand && bomb == false){
							ContralPenquan(true);
						}
						break;
					}
				default:
					{
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			this.Error = ex;
			return;
		}
    }

    public void Button_OnClick_clone()
    {
		int i = 0;
		DanmakuModel tmp = new DanmakuModel();
		tmp.giftId = text_id.text;
		tmp.GiftName = "测试物品";
		tmp.GiftCount = int.Parse(text_count.text);
		tmp.giftPrice = int.Parse(text_price.text);
		for (i = 1; i <= tmp.GiftCount; i++)
        {
            addGift(tmp);
        }
    }

	public void Button_OnClick_Bomb(){
		GameObject Cube_left_bomb = GameObject.Find ("Cube_left_bomb");
		GameObject Cube_right_bomb = GameObject.Find ("Cube_right_bomb");

		bool open = !Cube_left_bomb.GetComponent<BoxCollider2D> ().usedByEffector;

		Cube_left_bomb.GetComponent<BoxCollider2D>().usedByEffector = open;
		Cube_right_bomb.GetComponent<BoxCollider2D>().usedByEffector = open;
		Cube_left_bomb.GetComponent<Rigidbody2D>().simulated = open;
		Cube_right_bomb.GetComponent<Rigidbody2D>().simulated = open;
	}

	public void SetBomb(bool simulated){

	}

	void addGift(DanmakuModel gift)
    {
		if (!SpriteArr [int.Parse(gift.giftId)]) {
			StartCoroutine (GetGiftPic (gift));
		} else {
			MakeClone(gift);
		}
    }

	public void MakeClone(DanmakuModel gift)
    {
		count++;
        GameObject clone = Instantiate(Ball, new Vector3(0, 6, 0), Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f))) as GameObject;
		clone.GetComponent<SpriteRenderer>().sprite = SpriteArr[int.Parse(gift.giftId)];
		clone.GetComponent<Rigidbody2D>().name = "Ball";
		clone.GetComponent<Rigidbody2D>().mass = gift.giftPrice/4000f;
		clone.GetComponent<Renderer> ().sortingOrder = gift.giftPrice/100;
        clone.GetComponent<Rigidbody2D>().velocity = new Vector2(UnityEngine.Random.Range(-3.0f, 3.0f), UnityEngine.Random.Range(-3.0f, 3.0f));
		clone.GetComponent<Rigidbody2D>().simulated = true;
    }

	IEnumerator GetGiftPic(DanmakuModel gift)
    {
		//UnityEngine.Debug.Log ("downloadPic:" + gift.giftId);
		//string url = string.Format("https://s1.hdslb.com/bfs/static/blive/blfe-live-room/static/img/gift-images/image-png/gift-{0}.png", gift.giftId);
		UnityEngine.Debug.Log (giftListData[int.Parse(gift.giftId)].ToString());
		WWW www = new WWW(giftListData[int.Parse(gift.giftId)].ToString());//giftListData[int.Parse(gift.giftId)]["img_basic"]
        yield return www;
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            Texture2D texture = ScaleTexture(www.texture, 100, 100);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			SpriteArr[int.Parse(gift.giftId)] = sprite;
			MakeClone(gift);
        }
    }

    public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }
        result.Apply();
        return result;
	}

	public void Button_OnClick_DanmakuLoad()
	{
		var Thread = new Thread(this.Connect);
		Thread.IsBackground = true;
		Thread.Start();
	}

	private TcpClient Client;
	private NetworkStream NetStream;
	private bool Connected = false;
	public Exception Error;
	public delegate void ReceivedDanmakuEvt(object sender, ReceivedDanmakuArgs e);
	private short protocolversion = 1;

	public class ReceivedDanmakuArgs
	{
		public DanmakuModel Danmaku;
	}

	public void Connect()
    {
        try
        {
            int channelId = roomId;
            Client = new TcpClient();
			Client.Connect(ChatHost, ChatPort);
         	NetStream = Client.GetStream();
      		if (SendJoinChannel(channelId))
            {
				Connected = true;
				var HeartbeatLoopThread = new Thread(this.HeartbeatLoop);
				HeartbeatLoopThread.IsBackground = true;
				HeartbeatLoopThread.Start();
				var ReceiveMessageLoopThread = new Thread(this.ReceiveMessageLoop);
				ReceiveMessageLoopThread.IsBackground = true;
				ReceiveMessageLoopThread.Start();
				return;
            }
			return;
        }
        catch (Exception ex)
        {
            this.Error = ex;
			return;
        }
    }

	private bool SendJoinChannel(int channelId)
	{

		System.Random r=new System.Random();
		var tmpuid = (long)(1e14 + 2e14*r.NextDouble());
		var packetModel = new {roomid = channelId, uid = tmpuid};
		var playload = JsonConvert.SerializeObject(packetModel);
		SendSocketData(7, playload);
		return true;
	}

	void SendSocketData(int action, string body = "")
	{
		SendSocketData(0, 16, protocolversion, action, 1, body);
	}

	void SendSocketData(int packetlength, short magic, short ver, int action, int param = 1, string body = "")
	{
		var playload = Encoding.UTF8.GetBytes(body);
		if (packetlength == 0)
		{
			packetlength = playload.Length + 16;
		}
		var buffer = new byte[packetlength];
		using (var ms = new MemoryStream(buffer))
		{


			var b = BitConverter.GetBytes(buffer.Length).ToBE();

			ms.Write(b, 0, 4);
			b = BitConverter.GetBytes(magic).ToBE();
			ms.Write(b, 0, 2);
			b = BitConverter.GetBytes(ver).ToBE();
			ms.Write(b, 0, 2);
			b = BitConverter.GetBytes(action).ToBE();
			ms.Write(b, 0, 4);
			b = BitConverter.GetBytes(param).ToBE();
			ms.Write(b, 0, 4);
			if (playload.Length > 0)
			{
				ms.Write(playload, 0, playload.Length);
			}
			NetStream.Write(buffer, 0, buffer.Length);
			NetStream.Flush();
		}
	}

	private void HeartbeatLoop()
	{
		try
		{
			while (this.Connected)
			{
				this.SendHeartbeatAsync();
				System.Threading.Thread.Sleep(30000); 
			}
		}
		catch (Exception ex)
		{
			this.Error = ex;
			_disconnect();

		}
	}

	private void SendHeartbeatAsync()
	{
		SendSocketData(2);
		UnityEngine.Debug.Log("Message Sent: Heartbeat");
	}

	private void _disconnect()
	{
		if (Connected)
		{
			UnityEngine.Debug.Log("Disconnected");
			Connected = false;
			Client.Close();
			NetStream = null;
		}
	}

	private void ReceiveMessageLoop()
	{
		try
		{
			var stableBuffer = new byte[Client.ReceiveBufferSize];
			//UnityEngine.Debug.Log(this.Connected);
			while (this.Connected)
			{

				NetStream.ReadB(stableBuffer, 0, 4);
				var packetlength = BitConverter.ToInt32(stableBuffer, 0);
				packetlength = IPAddress.NetworkToHostOrder(packetlength);

				if (packetlength < 16)
				{
					UnityEngine.Debug.Log("协议失败: (L:" + packetlength + ")");
				}

				NetStream.ReadB(stableBuffer, 0, 2);
				NetStream.ReadB(stableBuffer, 0, 2);

				NetStream.ReadB(stableBuffer, 0, 4);
				var typeId = BitConverter.ToInt32(stableBuffer, 0);
				typeId = IPAddress.NetworkToHostOrder(typeId);

				Console.WriteLine(typeId);
				NetStream.ReadB(stableBuffer, 0, 4);
				var playloadlength = packetlength - 16;
				if (playloadlength == 0)
				{
					continue;

				}
				typeId = typeId - 1;
				var buffer = new byte[playloadlength];
				NetStream.ReadB(buffer, 0, playloadlength);
				switch (typeId)
				{
				case 0:
				case 1:
				case 2:
					{
						var viewer = BitConverter.ToUInt32(buffer.Take(4).Reverse().ToArray(), 0);
						UnityEngine.Debug.Log("online:"+viewer);
						break;
					}
				case 3:
				case 4:
					{
						var json = Encoding.UTF8.GetString(buffer, 0, playloadlength);
						//UnityEngine.Debug.Log("playerCommand:"+json);
						try
						{
							DanmakuModel dama = new DanmakuModel(json, 2);
							switch(dama.MsgType)
							{
							case MsgTypeEnum.GiftSend:
								{
									//UnityEngine.Debug.Log(dama.giftId);
									queue.Enqueue(dama);
									break;
								}
							case MsgTypeEnum.Comment:
								{
									//UnityEngine.Debug.Log(dama.CommentText);
									if(dama.isAdmin && dama.CommentText == penquanCommand){
										queue.Enqueue(dama);
									}
									break;
								}
							default:
								{
									break;
								}
							}
						}
						catch (Exception ex)
						{
							UnityEngine.Debug.Log(ex);
						}
						break;
					}
				case 5://newScrollMessage
					{

						break;
					}
				case 7:
					{

						break;
					}
				case 16:
					{
						break;
					}
				default:
					{

						break;
					}
					//                     
				}
			}
		}
		catch (NotSupportedException ex)
		{
			this.Error = ex;
			_disconnect();
		}
		catch (Exception ex)
		{
			this.Error = ex;
			_disconnect();

		}
	}
}
