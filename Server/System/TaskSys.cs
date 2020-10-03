
using Protocal;
/// <summary>
/// 任务奖励业务系统
/// </summary>
public class TaskSys
{

    private static TaskSys instance = null;
    public static TaskSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TaskSys();
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

      

        Common.Log("TaskSys Init Done.");
    }

    public void ReqTakeTaskReward(MsgPack pack) {
        ReqTakeTaskReward data = pack.msg.reqTakeTaskReward;

        GameMsg msg = new GameMsg {
            cmd = (int)CMD.RspTakeTaskReward
        };

        PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(data.rid);
        TaskRewardData trd = CalcTaskRewardData(playerData, data.rid);

        if (trd.prgs ==trc.count && trd.isTakenReward == false)
        {
            playerData.coin += trc.coin;
            Common.CalcExp(playerData,trc.exp);
            trd.isTakenReward = true;

            // 更新任务进度数据
            CalcTaskArr(playerData,trd);

            if (cacheSvc.UpdatePlayerData(playerData.id,playerData) == true)
            {
                RspTakeTaskReward rspTakeTaskReward = new RspTakeTaskReward {
                    coin = playerData.coin,
                    lv = playerData.lv,
                    exp = playerData.exp,
                    taskArr = playerData.taskArr
                };

                msg.rspTakeTaskReward = rspTakeTaskReward;
                

            }
            else {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
        }
        else
        {
            msg.err = (int)ErrorCode.ClientDataError;
        }

        pack.session.SendMsg(msg);
    }

    public TaskRewardData CalcTaskRewardData(PlayerData playerData, int rid) {
        TaskRewardData trd = null;

        for (int i = 0; i < playerData.taskArr.Length; i++)
        {
            string[] taskInfo = playerData.taskArr[i].Split('|');

            // 1|0|0
            if (int.Parse(taskInfo[0])== rid)
            {
                trd = new TaskRewardData {
                    ID = rid,
                    prgs = int.Parse(taskInfo[1]),
                    isTakenReward = taskInfo[2].Equals("1")
                };

                break;
            }
        }

        return trd;
    }

    public void CalcTaskArr(PlayerData playerData, TaskRewardData trd) {
        string result = trd.ID + "|" + trd.prgs + "|" + (trd.isTakenReward ? 1 : 0);
        int index = -1;

        for (int i = 0; i < playerData.taskArr.Length; i++)
        {
            string[] taskInfo = playerData.taskArr[i].Split('|');

            // 1|0|0
            if (int.Parse(taskInfo[0]) == trd.ID)
            {
                index = i;
                break;
            }
        }

        playerData.taskArr[index] = result;
    }

    public void CalcTaskPrgs(PlayerData playerData,int tid) {
        TaskRewardData trd = CalcTaskRewardData(playerData,tid);
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(tid);

        if (trd.prgs < trc.count)
        {
            trd.prgs += 1;

            // 更新任务进度
            CalcTaskArr(playerData,trd);

            ServerSession session = cacheSvc.GetOnlineServerSession(playerData.id);
            if (session != null)
            {
                session.SendMsg(new GameMsg {
                    cmd = (int)CMD.PshTaskPrgs,
                    pshTaskPrgs = new PshTaskPrgs {
                        taskArr = playerData.taskArr
                    }
                });
            }
              
        }
    }

    public PshTaskPrgs GetCalcTaskPrgs(PlayerData playerData, int tid)
    {
        TaskRewardData trd = CalcTaskRewardData(playerData, tid);
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(tid);

        if (trd.prgs < trc.count)
        {
            trd.prgs += 1;

            // 更新任务进度
            CalcTaskArr(playerData, trd);

            return new PshTaskPrgs
            {
                taskArr = playerData.taskArr
            };

        }
        else {
            return null;
        }
    }
}

