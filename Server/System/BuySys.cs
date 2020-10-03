
using Protocal;
/// <summary>
/// 交易购买系统
/// </summary>
public class BuySys
{
    private static BuySys instance = null;
    public static BuySys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BuySys();
            }

            return instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;

        Common.Log("BuySys Init Done.");
    }

    public void ReqBuy(MsgPack pack) {
        ReqBuy data = pack.msg.reqBuy;
        GameMsg msg = new GameMsg {
            cmd = (int)CMD.RspBuy
        };

        PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);

        if (playerData.diamond < data.cost)
        {
            msg.err = (int)ErrorCode.LackDiamond;
        }
        else {
            playerData.diamond -= data.cost;
            PshTaskPrgs pshTaskPrgs = null;
            switch (data.type)
            {
                case 0:
                    
                    playerData.power += 100;
                    // 任务进度更新
                    pshTaskPrgs = TaskSys.Instance.GetCalcTaskPrgs(playerData, 4);
                    break;
                case 1:
                    
                    playerData.coin += 1000;
                    // 任务进度更新
                    pshTaskPrgs = TaskSys.Instance.GetCalcTaskPrgs(playerData, 5);
                    break;
            }

            if (cacheSvc.UpdatePlayerData(playerData.id, playerData) == false)
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                RspBuy rspBuy = new RspBuy
                {
                    type = data.type,
                    diamond = playerData.diamond,
                    coin = playerData.coin,
                    power = playerData.power
                };

                msg.rspBuy = rspBuy;

                // 并包处理
                msg.pshTaskPrgs = pshTaskPrgs;
            }

        }


        pack.session.SendMsg(msg);

    }
}

