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

        public int hp;          // 生命值
        public int ad;          // 物理攻击
        public int ap;          // 法术攻击
        public int addef;       // 物理防御
        public int apdef;       // 法术防御
        public int dodge;       // 闪避概率
        public int pierce;      // 穿透概率
        public int critical;    // 暴击概率

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

    public enum ErrorCode {
        None=0,         // 没有错误

        UpdateDBError,  // 更新数据库出错

        AcctIsOnline,   //客户端已在线上
        WrongPassword,  // 密码错误
        NameIsExist,    // 名字已存在
    }

    public enum CMD {
        None = 0,

        // 登录相关
        ReqLogin = 101,
        RspLogin = 102,

        ReqRename = 103,
        RspRename = 104,
    }

    public class SrvCfg {
        public const string srvIP = "127.0.0.1";
        public const int srvPort = 16777;
    }
}
