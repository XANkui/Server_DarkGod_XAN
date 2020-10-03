

using Protocal;
/// <summary>
/// 引导业务系统
/// </summary>
public class GuideSys
{
    private static GuideSys instance = null;
    public static GuideSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GuideSys();
            }

            return instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;

        Common.Log("GuideSys Init Done.");
    }

    public void ReqGuide(MsgPack pack) {
        ReqGuide data = pack.msg.reqGuide;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspGuide
        };

        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        GuideCfg gc = cfgSvc.GetGuideCfg(data.guideid);
        // 更新引导ID
        if (pd.guideid == data.guideid)
        {
            // 检测是否是智者点拨任务
            if (pd.guideid == 1001)
            {
                TaskSys.Instance.CalcTaskPrgs(pd,1);
            }


            pd.guideid += 1;

            // 更新玩家数据
            pd.coin += gc.coin;
            Common.CalcExp(pd,gc.exp);

            // 数据更新数据库
            if (cacheSvc.UpdatePlayerData(pd.id, pd) == false)
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else {
                msg.rspGuide = new RspGuide
                {
                    guideid = pd.guideid,
                    coin = pd.coin,
                    lv = pd.lv,
                    exp = pd.exp
                };
            }
        }
        else {
            msg.err = (int)ErrorCode.ServerDataError;
        }

        pack.session.SendMsg(msg);
    }

    
}

