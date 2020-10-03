
using Protocal;
using System.Collections.Generic;
/// <summary>
/// 体力恢复系统
/// </summary>
public class PowerSys
{
    private static PowerSys instance = null;
    public static PowerSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PowerSys();
            }

            return instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private TimerSvc timerSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        timerSvc = TimerSvc.Instance;

     
        TimerSvc.Instance.AddTimeTask(CalPowerAdd,Common.PowerAddSpace,PETimeUnit.Minute,0);

        Common.Log("ChatSys Init Done.");
    }

    private void CalPowerAdd(int tid) {
        // TODo 计算体力的增加
        Common.Log("All Online Player Calc Power Increase…… ……");

        GameMsg msg = new GameMsg {
            cmd = (int)CMD.PshPower
        };

        msg.pshPower = new PshPower();

        // 所有在线玩家获得实时的体力增长推送
        Dictionary<ServerSession, PlayerData> onlineDic = cacheSvc.GetOnlineCache();
        foreach (var item in onlineDic)
        {
            PlayerData pd = item.Value;
            ServerSession session = item.Key;
            int powerMax = Common.GetPowerLimit(pd.lv);
            if (pd.power >= powerMax)
            {
                continue;
            }
            else {
                pd.power += Common.PowerAddCount;
                // 同时更新一下时间（避免意外下线，无法更新下线时间的情况）
                pd.time = timerSvc.GetNowTime();
                if (pd.power > powerMax)
                {
                    pd.power = powerMax;
                }
            }

            if (cacheSvc.UpdatePlayerData(pd.id, pd) == false)
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else {
                msg.pshPower.power = pd.power;
                session.SendMsg(msg);
            }


        }
    }
}

