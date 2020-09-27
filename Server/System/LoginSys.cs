using Protocal;

/// <summary>
/// 登陆业务逻辑
/// </summary>
public class LoginSys
{
    private static LoginSys instance = null;
    public static LoginSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LoginSys();
            }

            return instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;

        Common.Log("LoginSys Init Done.");
    }

    public void ReqLogin(MsgPack pack) {
        ReqLogin data = pack.msg.reqLogin;
        // 当前账号是否已经上线
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspLogin,
            
        };
        if (cacheSvc.IsOnlineAcct(data.acct) == true)
        {
            // 已上线，返回错误信息
            msg.err = (int)ErrorCode.AcctIsOnline;
        }
        else {
            // 未上线：1、账号是否存在；存在，检测密码；不存在，则创建默认的账号密码
            PlayerData playerData = cacheSvc.GetPlayerData(data.acct,data.pass);
            if (playerData == null)
            {
                // 存在，密码错误
                msg.err = (int)ErrorCode.WrongPassword;
            }
            else {
                msg.rspLogin = new RspLogin
                {
                    playerData = playerData
                };

                // 缓存账号数据
                cacheSvc.AcctOnline(data.acct, pack.session, playerData);
            }
        }


        // 回应客户端
        pack.session.SendMsg(msg);
    }

    /// <summary>
    /// 创建角色，重命名
    /// </summary>
    /// <param name="pack"></param>
    public void ReqRename(MsgPack pack) {
        ReqRename data = pack.msg.reqRename;
        GameMsg msg = new GameMsg {
            cmd = (int)CMD.RspRename
        };

        if (cacheSvc.IsNameExit(data.name) == true)
        {
            // 名字已存在，返回错误码
            msg.err = (int)ErrorCode.NameIsExist;
        }
        else {
            // 名字未使用，更新缓存，数据库，返回给客户端
            PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);
            playerData.name = data.name;

            if (cacheSvc.UpdatePlayerData(playerData.id, playerData) == false)
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else {
                msg.rspRename = new RspRename { name =data.name };
            }
        }

        pack.session.SendMsg(msg);
       
    }

    public void ClearOfflineData(ServerSession session) {
        cacheSvc.AcctOffline(session);
    }

}

