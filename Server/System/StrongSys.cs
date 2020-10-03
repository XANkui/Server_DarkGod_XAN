

using Protocal;
/// <summary>
/// 强化升级业务系统
/// </summary>
public class StrongSys
{
    private static StrongSys instance = null;
    public static StrongSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new StrongSys();
            }

            return instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;

        Common.Log("StrongSys Init Done.");
    }

    public void ReqStrong(MsgPack pack) {
        ReqStrong data = pack.msg.reqStrong;

        GameMsg msg = new GameMsg {
            cmd = (int)CMD.RspStrong
        };

        PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);
        int curStarLv = playerData.strongArr[data.pos];

        StrongCfg nextSc = CfgSvc.Instance.GetStrongCfg(data.pos,curStarLv +1);

        // 条件判断
        if (playerData.lv < nextSc.minlv)
        {
            msg.err = (int)ErrorCode.LackLevel;
        }
        else if (playerData.coin < nextSc.coin)
        {
            msg.err = (int)ErrorCode.LackCoin;
        }
        else if (playerData.crystal < nextSc.crystal)
        {
            msg.err = (int)ErrorCode.LackCrystal;
        }
        else {

            
            // 资源扣除
            playerData.coin -= nextSc.coin;
            playerData.crystal -= nextSc.crystal;

            // 属性修改
            playerData.strongArr[data.pos] += 1;
            playerData.hp += nextSc.addhp;
            playerData.ad += nextSc.addhurt;
            playerData.ap += nextSc.addhurt;
            playerData.addef += nextSc.adddef;
            playerData.apdef += nextSc.adddef;

            // 任务进度更新
            TaskSys.Instance.CalcTaskPrgs(playerData, 3);
        }


        // 更新数据库
        if (cacheSvc.UpdatePlayerData(playerData.id, playerData) == false)
        {
            msg.err = (int)ErrorCode.UpdateDBError;
        }
        else {
            msg.rspStrong = new RspStrong {
                coin = playerData.coin,
                crystal = playerData.crystal,
                hp = playerData.hp,
                ad = playerData.ad,
                ap = playerData.ap,
                addef = playerData.addef,
                apdef = playerData.apdef,
                strongArr = playerData.strongArr
            };
        }

        pack.session.SendMsg(msg);
    }
}

