
using Protocal;

public class FubenSys
{
    private static FubenSys instance = null;
    public static FubenSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new FubenSys();
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



        Common.Log("FubenSys Init Done.");
    }

    public void ReqFBFight(MsgPack pack) {
        ReqFBFight data = pack.msg.reqFBFight;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspFBFight
        };

        PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);
        int power = cfgSvc.GetMapCfg(data.fbId).power;

        // 判断发过来的副本id是否合法
        if (data.fbId>playerData.fuben)
        {
            msg.err = (int)ErrorCode.ClientDataError;
        }
        else if (playerData.power < power)
        {
            msg.err = (int)ErrorCode.LackPower;
        }else
        {
            playerData.power -= power;
            if (cacheSvc.UpdatePlayerData(playerData.id,playerData)==false)
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                RspFBFight rspFBFight = new RspFBFight {
                    fbId = data.fbId,
                    power = playerData.power
                };

                msg.rspFBFight = rspFBFight;
            }
        }

        pack.session.SendMsg(msg);
    }

    public void ReqFBFightEnd(MsgPack pack) {
        ReqFBFightEnd data = pack.msg.reqFBFightEnd;

        GameMsg msg = new GameMsg {
            cmd = (int)CMD.RspFBFightEnd
        };

        // 校验战斗是否合法
        if (data.win == true)
        {
            if (data.costtime >0 && data.resthp > 0)
            {
                // 根据对应副本ID获取奖励
                MapCfg rd = cfgSvc.GetMapCfg(data.fbId);
                PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

                // 任务进度更新
                TaskSys.Instance.CalcTaskPrgs(pd, 2);

                pd.coin += rd.coin;
                pd.crystal += rd.exp;
                Common.CalcExp(pd, rd.exp);

                if (pd.fuben == data.fbId)
                {
                    pd.fuben += 1;
                }

                // 更新数据库
                if (cacheSvc.UpdatePlayerData(pd.id, pd) == false)
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else {
                    RspFBFightEnd rspFBFightEnd = new RspFBFightEnd {
                        win =data.win,
                        fbId = data.fbId,
                        resthp =data.resthp,
                        costtime = data.costtime,

                        coin = pd.coin,
                        lv = pd.lv,
                        exp = pd.exp,
                        crystal = pd.crystal,
                        fuben = pd.fuben
                    };

                    msg.rspFBFightEnd = rspFBFightEnd;
                }

            }
        }
        else
        {
            msg.err = (int)ErrorCode.ClientDataError;
        }

        pack.session.SendMsg(msg);
    }
}
