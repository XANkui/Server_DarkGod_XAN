

using Protocal;
using System.Collections.Generic;
/// <summary>
/// 聊天业务系统
/// </summary>
public class ChatSys
{
    private static ChatSys instance = null;
    public static ChatSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ChatSys();
            }

            return instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;

        Common.Log("ChatSys Init Done.");
    }

    public void SndChat(MsgPack pack) {
        SndChat data = pack.msg.sndChat;

        PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);

        // 任务进度更新
        TaskSys.Instance.CalcTaskPrgs(playerData, 6);

        GameMsg msg = new GameMsg {
            cmd = (int)CMD.PshChat,
            pshChat = new PshChat {
                name = playerData.name,
                chat= data.chat
            }
        };

        // 广播所有在线客户端
        List<ServerSession> lst = cacheSvc.GetAllOnlineServerSessions();
        // 避免重复多次的Cpu自己的序列化二进制 消息（所以这里优先二进制，避免cpu重复多次浪费性能）
        byte[] bytes = PENet.PETool.PackNetMsg(msg);
        for (int i = 0; i < lst.Count; i++)
        {
            lst[i].SendMsg(bytes);
        }

    }
}

