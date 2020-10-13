using System;
using PENet;

namespace Protocal
{
    [Serializable]
    public class GameMsg : PEMsg
    {
        public ReqLogin reqLogin;
        public RspLogin rspLogin;

        public ReqRename reqRename;
        public RspRename rspRename;

        public ReqGuide reqGuide;
        public RspGuide rspGuide;


        public ReqStrong reqStrong;
        public RspStrong rspStrong;

        public SndChat sndChat;
        public PshChat pshChat;

        public ReqBuy reqBuy;
        public RspBuy rspBuy;

        public PshPower pshPower;

        public ReqTakeTaskReward reqTakeTaskReward;
        public RspTakeTaskReward rspTakeTaskReward;
        public PshTaskPrgs pshTaskPrgs;

        public ReqFBFight reqFBFight;
        public RspFBFight rspFBFight;
        public ReqFBFightEnd reqFBFightEnd;
        public RspFBFightEnd rspFBFightEnd;
    }

    #region 登录相关
    [Serializable]
    public class ReqLogin {
        public string acct;
        public string pass;
    }

    [Serializable]
    public class RspLogin
    {
        public PlayerData playerData;
    }

    [Serializable]
    public class PlayerData
    {
        public int id;
        public string name;
        public int lv;
        public int exp;
        public int power;
        public int coin;
        public int diamond;
        public int crystal;

        public int hp;          // 生命值
        public int ad;          // 物理攻击
        public int ap;          // 法术攻击
        public int addef;       // 物理防御
        public int apdef;       // 法术防御
        public int dodge;       // 闪避概率
        public int pierce;      // 穿透概率
        public int critical;    // 暴击概率


        public int guideid;     // 引导任务ID
        public int[] strongArr; // 装备强化

        public long time;       // 上次下机时间

        public string[] taskArr; // 任务奖励
        public int fuben;       // 副本（临时的，真正的不止是一个int）

        //Todo

    }

    [Serializable]
    public class ReqRename {
        public string name;
    }
    [Serializable]
    public class RspRename {
        public string name;
    }

    #endregion

    #region 引导相关
    [Serializable]
    public class ReqGuide {
        public int guideid;
    }
    [Serializable]
    public class RspGuide {
        public int guideid;
        public int coin;
        public int lv;
        public int exp;
    }

    #endregion

    #region 强化相关
    [Serializable]
    public class ReqStrong {
        public int pos;
    }

    [Serializable]
    public class RspStrong
    {
        public int coin;
        public int crystal;
        public int hp;
        public int ad;
        public int ap;
        public int addef;
        public int apdef;
        public int[] strongArr;     //各个装备部分目前星级的列表（与UI显示对应）
    }
    #endregion

    #region Chat

    [Serializable]
    public class SndChat {
        public string chat;
    }

    [Serializable]
    public class PshChat
    {
        public string name;
        public string chat;
    }

    #endregion

    #region Buy
    [Serializable]
    public class ReqBuy {
        public int type;
        public int cost;
    }

    [Serializable]
    public class RspBuy
    {
        public int type;
        public int diamond;
        public int coin;
        public int power;
    }

    #endregion

    #region Power
    [Serializable]
    public class PshPower {
        public int power;
    }
    #endregion

    #region TaskReward

    [Serializable]
    public class ReqTakeTaskReward {
        public int rid;
    }
    
    [Serializable]
    public class RspTakeTaskReward
    {
        public int coin;
        public int lv;
        public int exp;
        public string[] taskArr;
    }

    [Serializable]
    public class PshTaskPrgs
    {
       
        public string[] taskArr;
    }

    #endregion

    #region Fuben Fight
    [Serializable]
    public class ReqFBFight {
        public int fbId;
    }

    [Serializable]
    public class RspFBFight
    {
        public int fbId;
        public int power;
    }

    [Serializable]
    public class ReqFBFightEnd
    {
        public int fbId;
        public int resthp;
        public int costtime;
        public bool win;
    }

    [Serializable]
    public class RspFBFightEnd
    {
        public int fbId;
        public int resthp;
        public int costtime;
        public bool win;

        //副本奖励
        public int coin;
        public int lv;
        public int exp;
        public int crystal;
        public int fuben;
    }

    #endregion

    public enum ErrorCode {
        None=0,         // 没有错误

        ServerDataError,    // 服务器数据异常（服务端与客户端数据不一致，客户端可能开外挂）
        ClientDataError,    // 客户端数据异常（服务端与客户端数据不一致，客户端可能开外挂）

        UpdateDBError,  // 更新数据库出错

        AcctIsOnline,   //客户端已在线上
        WrongPassword,  // 密码错误
        NameIsExist,    // 名字已存在

        LackLevel,  //等级不够
        LackCoin,   // 金币不够
        LackCrystal,    // 水晶不够
        LackDiamond,    // 钻石不够
        LackPower,    // 体力不够
    }

    public enum CMD {
        None = 0,

        // 登录相关
        ReqLogin = 101,
        RspLogin = 102,

        ReqRename = 103,
        RspRename = 104,

        // 主城相关
        ReqGuide = 201,
        RspGuide = 202,

        // 强化相关
        ReqStrong = 203,
        RspStrong = 204,

        // 聊天
        SndChat =205,
        PshChat =206,

        ReqBuy = 207,
        RspBuy = 208,

        PshPower = 209,

        ReqTakeTaskReward = 210,
        RspTakeTaskReward = 210,
        PshTaskPrgs = 211,

        // 副本战斗
        ReqFBFight =301,
        RspFBFight =302,
        ReqFBFightEnd = 303,
        RspFBFightEnd = 304,

    }

    public class SrvCfg {
        public const string srvIP = "127.0.0.1";
        public const int srvPort = 16777;
    }
}
