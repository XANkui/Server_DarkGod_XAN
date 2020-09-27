
using PENet;
using Protocal;

public class ServerSession:PESession<GameMsg>
{
    public int SessionID = 0;

    protected override void OnConnected()
    {
        SessionID = ServerRoot.Instance.GetSessionID();

        Common.Log("SessionID:"+ SessionID+ " Client Connect");

        
    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        Common.Log("SessionID:" + SessionID + " Client Req: cmd = " + ((CMD)msg.cmd).ToString());

        NetSvc.Instance.AddMsgQue(this,msg);
    }

    protected override void OnDisConnected()
    {
        LoginSys.Instance.ClearOfflineData(this);
        Common.Log("SessionID:" + SessionID + " Client Offline");
    }
}

