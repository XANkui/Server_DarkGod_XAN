
using Protocal;
using System.Collections.Generic;

public class CacheSvc
{
    private static CacheSvc instance = null;
    public static CacheSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CacheSvc();
            }

            return instance;
        }
    }

    private DBMgr dbMgr = null;
    private Dictionary<string, ServerSession> onlineAcctDic = new Dictionary<string, ServerSession>();
    private Dictionary<ServerSession, PlayerData> onlineSessionPdDic = new Dictionary<ServerSession, PlayerData>();

    public void Init()
    {
        dbMgr = DBMgr.Instance;
        Common.Log("CacheSvc Init Done.");
    }

    public bool IsOnlineAcct(string acct) {

        return onlineAcctDic.ContainsKey(acct);
    }

    /// <summary>
    /// 根据账号获取对应的玩家数据
    /// </summary>
    /// <param name="acct"></param>
    /// <param name="pass"></param>
    /// <returns></returns>
    public PlayerData GetPlayerData(string acct, string pass) {

        // 从数据库中提取玩家信息
        PlayerData playerData = dbMgr.QueryPlayerData(acct,pass);

        return playerData;
    }

    /// <summary>
    /// 帐号上线，缓存数据
    /// </summary>
    /// <param name="acct"></param>
    /// <param name="session"></param>
    /// <param name="playerData"></param>
    public void AcctOnline(string acct, ServerSession session, PlayerData playerData) {
        onlineAcctDic.Add(acct,session);
        onlineSessionPdDic.Add(session,playerData);
    }

    /// <summary>
    /// 判断名字是否已存在
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool IsNameExit(string name) {
        return dbMgr.QueryNameData(name);
    }

    public PlayerData GetPlayerDataBySession(ServerSession session) {
        if (onlineSessionPdDic.TryGetValue(session,out PlayerData playerdata))
        {
            return playerdata;
        }

        return null;
    }

    public bool UpdatePlayerData(int id, PlayerData playerData) {
        
        return dbMgr.UpdatePlayerData(id,playerData);
    }

    public void AcctOffline(ServerSession session) {
        foreach (var item in onlineAcctDic)
        {
            if (item.Value == session)
            {
                onlineAcctDic.Remove(item.Key);
                break;
            }
        }

        bool succ = onlineSessionPdDic.Remove(session);
        Common.Log("Offline Result:SessionID = "+session.SessionID+" result = " +succ);
    }

    public List<ServerSession> GetAllOnlineServerSessions() {
        List<ServerSession> lst = new List<ServerSession>();
        foreach (var item in onlineSessionPdDic)
        {
            lst.Add(item.Key);
        }

        return lst;
    }

    public Dictionary<ServerSession, PlayerData> GetOnlineCache() {
        return onlineSessionPdDic;
    }

    public ServerSession GetOnlineServerSession(int ID) {
        ServerSession session = null;

        foreach (var item in onlineSessionPdDic)
        {
            if (item.Value.id == ID)
            {
                session = item.Key;
                break;
            }
        }

        return session;
    }
}

