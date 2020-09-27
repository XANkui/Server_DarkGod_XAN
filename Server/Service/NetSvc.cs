using PENet;
using Protocal;
using System.Collections.Generic;

/// <summary>
/// 消息包装
/// 1、ServerSession 客户端（IP等）
/// 2、GameMsg 具体消息
/// </summary>
public class MsgPack{
    public ServerSession session;
    public GameMsg msg;

    public MsgPack(ServerSession session, GameMsg msg)
    {
        this.session = session;
        this.msg = msg;
    }
}

/// <summary>
/// 网络服务
/// </summary>
public class NetSvc
{
    private static NetSvc instance = null;
    public static NetSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetSvc();
            }

            return instance;
        }
    }

    public static readonly string obj = "lock";
    private Queue<MsgPack> msgPackQue = new Queue<MsgPack>();

    public void Init()
    {
        PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
        server.StartAsServer(SrvCfg.srvIP,SrvCfg.srvPort);

        Common.Log("NetSvc Init Done.");
    }

    public void AddMsgQue(ServerSession session, GameMsg msg) {
        lock (obj)
        {
            msgPackQue.Enqueue(new MsgPack(session,msg));
        }
    }

    public void Update() {
        if (msgPackQue.Count>0)
        {
            Common.Log("PackCount:"+msgPackQue.Count);

            lock (obj)
            {
                MsgPack pack = msgPackQue.Dequeue();
                HandQueMsg(pack);
            }
        }
    }

    private void HandQueMsg(MsgPack pack) {
        switch ((CMD)pack.msg.cmd)
        {
            
            case CMD.ReqLogin:
                LoginSys.Instance.ReqLogin(pack);
                break;
            

            case CMD.ReqRename:
                LoginSys.Instance.ReqRename(pack);
                break;
           
        }
    }
}

