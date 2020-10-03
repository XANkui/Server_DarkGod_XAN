
using MySql.Data.MySqlClient;
using Protocal;
/// <summary>
/// 数据库管理
/// </summary>
public class DBMgr
{
    private static DBMgr instance = null;
    public static DBMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DBMgr();
            }

            return instance;
        }
    }

    private MySqlConnection conn = null;

    public void Init()
    {
        conn = new MySqlConnection("server=localhost;User Id = root;password=root123;Database=darkgod_xan;Charset=utf8");
        conn.Open();
        Common.Log("DBMgr Init Done.");

        // Test
        //QueryPlayerData("Test","Test");
    }

    /// <summary>
    /// 通过账号密码从数据库中查找角色数据
    /// </summary>
    /// <param name="acct"></param>
    /// <param name="pass"></param>
    /// <returns></returns>
    public PlayerData QueryPlayerData(string acct, string pass) {
        PlayerData playerData = null;
        MySqlDataReader reader = null;

        // 判断是否是新账号
        bool isNew = true;

        try
        {
            MySqlCommand cmd = new MySqlCommand("select * from account where acct = @acct", conn);
            // 这样防止数据库注入错误
            cmd.Parameters.AddWithValue("acct", acct);
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                isNew = false;
                string _pass = reader.GetString("pass");
                if (pass.Equals(_pass))
                {
                    playerData = new PlayerData
                    {
                        id = reader.GetInt32("id"),
                        name = reader.GetString("name"),
                        lv = reader.GetInt32("level"),
                        exp = reader.GetInt32("exp"),
                        power = reader.GetInt32("power"),
                        coin = reader.GetInt32("coin"),
                        diamond = reader.GetInt32("diamond"),
                        crystal = reader.GetInt32("crystal"),

                        hp = reader.GetInt32("hp"),
                        ad = reader.GetInt32("ad"),
                        ap = reader.GetInt32("ap"),
                        addef = reader.GetInt32("addef"),
                        apdef = reader.GetInt32("apdef"),
                        dodge = reader.GetInt32("dodge"),
                        pierce = reader.GetInt32("pierce"),
                        critical = reader.GetInt32("critical"),
                        guideid = reader.GetInt32("guideid"),
                        time = reader.GetInt64("time"),
                        fuben = reader.GetInt32("fuben")

                        //Todo Add
                    };

                    #region Strong

                    
                    // strong数据形式：1#2#3#4#5#6#
                    string[] strongStrArr = reader.GetString("strong").Split('#');
                    int[] _strongArr = new int[6];
                    for (int i = 0; i < strongStrArr.Length; i++)
                    {
                        if (strongStrArr[i]=="")
                        {
                            continue;
                        }
                        if (int.TryParse(strongStrArr[i], out int starlv))
                        {
                            _strongArr[i] = starlv;
                        }
                        else {
                            Common.Log("Parse Strong Data Error,Data = "+ strongStrArr[i], LogType.Error);
                        }
                    }

                    playerData.strongArr = _strongArr;
                    #endregion


                    #region TaskReward


                    // TaskReward 数据形式：1|0|0#2|0|0#3|0|0#4|0|0#5|0|0#
                    string[] tskRwdStrArr = reader.GetString("task").Split('#');
                    playerData.taskArr = new string[6];
                    for (int i = 0; i < tskRwdStrArr.Length; i++)
                    {
                        if (tskRwdStrArr[i] == "")
                        {
                            continue;
                        }
                        else if (tskRwdStrArr[i].Length>=5)
                        {
                            playerData.taskArr[i] = tskRwdStrArr[i];
                        }
                        else
                        {
                            Common.Log("Parse Strong Data Error,Data = " + tskRwdStrArr[i], LogType.Error);

                            throw new System.Exception("Data Error");
                        }
                    }

                    
                    #endregion
                    // Todo ADD
                }
            }
        }
        catch (System.Exception e)
        {
            Common.Log("Query PlayerData By acct&pass Error:" + e, LogType.Error);
            
        }
        finally {

            if (reader != null)
            {
                reader.Close();
            }

            if (isNew == true)
            {
                // 不存在账号，在创建账号(甚至默认数据)
                playerData = new PlayerData {
                    id = -1,
                    name = "",
                    lv = 1,
                    exp = 0,
                    power = 150,
                    coin = 5000,
                    diamond = 500,
                    crystal = 500,

                    hp = 2000,
                    ad = 275,
                    ap = 265,
                    addef = 67,
                    apdef = 43,
                    dodge = 7,
                    pierce = 5,
                    critical = 2,

                    guideid = 1001,
                    strongArr = new int[6],
                    time = TimerSvc.Instance.GetNowTime(),
                    taskArr = new string[6],
                    fuben = 10001

                   
                    

                    //Todo Add

                };
                // 数据形式：1 | 0 | 0#2|0|0#3|0|0#4|0|0#5|0|0#
                // 初始化任务奖励数据
                for (int i = 0; i < playerData.taskArr.Length; i++)
                {
                    playerData.taskArr[i] = (i + 1) + "|0|0";
                }

                // 保存数据到数据库中
                playerData.id =  InsertNewAcctData(acct,pass,playerData);
            }
        }


        return playerData;
    }

    private int InsertNewAcctData(string acct,string pass, PlayerData playerData) {

        int id = -1;

        try
        {
            MySqlCommand cmd = new MySqlCommand(
                "Insert into account set acct=@acct,pass=@pass,name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond,"+
                "crystal=@crystal,hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical,"+
                "guideid=@guideid,strong=@strong,time=@time,task=@task,fuben=@fuben", conn
                );
            cmd.Parameters.AddWithValue("acct",acct);
            cmd.Parameters.AddWithValue("pass", pass);
            cmd.Parameters.AddWithValue("name", playerData.name);
            cmd.Parameters.AddWithValue("level", playerData.lv);
            cmd.Parameters.AddWithValue("exp", playerData.exp);
            cmd.Parameters.AddWithValue("power", playerData.power);
            cmd.Parameters.AddWithValue("coin", playerData.coin);
            cmd.Parameters.AddWithValue("diamond", playerData.diamond);
            cmd.Parameters.AddWithValue("crystal", playerData.crystal);

            cmd.Parameters.AddWithValue("hp", playerData.hp);
            cmd.Parameters.AddWithValue("ad", playerData.ad);
            cmd.Parameters.AddWithValue("ap", playerData.ap);
            cmd.Parameters.AddWithValue("addef", playerData.addef);
            cmd.Parameters.AddWithValue("apdef", playerData.apdef);
            cmd.Parameters.AddWithValue("dodge", playerData.dodge);
            cmd.Parameters.AddWithValue("pierce", playerData.pierce);
            cmd.Parameters.AddWithValue("critical", playerData.critical);
            cmd.Parameters.AddWithValue("guideid", playerData.guideid);

            string strongArr = "";
            for (int i = 0; i < playerData.strongArr.Length; i++)
            {
                strongArr += playerData.strongArr[i];
                strongArr += "#";
            }
            cmd.Parameters.AddWithValue("strong", strongArr);
            cmd.Parameters.AddWithValue("time", playerData.time);

            // 数据形式：1 | 0 | 0#2|0|0#3|0|0#4|0|0#5|0|0#
            string taskArr = "";
            for (int i = 0; i < playerData.taskArr.Length; i++)
            {
                taskArr += playerData.taskArr[i];
                taskArr += "#";
            }
            cmd.Parameters.AddWithValue("task", taskArr);
            cmd.Parameters.AddWithValue("fuben", playerData.fuben);

            // Todo Add

            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId;
        }
        catch (System.Exception e)
        {

            Common.Log("Insert PlayerData By acct&pass Error:" + e, LogType.Error);

        }

        return id;
    }

    /// <summary>
    /// 查询名字是否重复
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool QueryNameData(string name) {
        bool isExist = false;
        MySqlDataReader reader = null;

        try
        {
            MySqlCommand cmd = new MySqlCommand("select * from account where name = @name", conn);
            cmd.Parameters.AddWithValue("name", name);
            reader = cmd.ExecuteReader();
            if (reader.Read() == true)
            {
                isExist = true;
            }
        }
        catch (System.Exception e)
        {

            Common.Log("Query Name Data Error:" + e, LogType.Error);
        }
        finally {
            if (reader != null)
            {
                reader.Close();
            }
        }

        return isExist;
    }

    public bool UpdatePlayerData(int id, PlayerData playerData) {
        try
        {
            MySqlCommand cmd = new MySqlCommand(
                "update account set name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal," +
                "hp = @hp, ad = @ad, ap = @ap, addef = @addef, apdef = @apdef, dodge = @dodge, pierce = @pierce, critical = @critical," +
                "guideid=@guideid,strong=@strong,time=@time,task=@task,fuben=@fuben" +
                " where id = @id",conn
                );

            cmd.Parameters.AddWithValue("id",id);
            cmd.Parameters.AddWithValue("name", playerData.name);
            cmd.Parameters.AddWithValue("level", playerData.lv);
            cmd.Parameters.AddWithValue("exp", playerData.exp);
            cmd.Parameters.AddWithValue("power", playerData.power);
            cmd.Parameters.AddWithValue("coin", playerData.coin);
            cmd.Parameters.AddWithValue("diamond", playerData.diamond);
            cmd.Parameters.AddWithValue("crystal", playerData.crystal);

            cmd.Parameters.AddWithValue("hp", playerData.hp);
            cmd.Parameters.AddWithValue("ad", playerData.ad);
            cmd.Parameters.AddWithValue("ap", playerData.ap);
            cmd.Parameters.AddWithValue("addef", playerData.addef);
            cmd.Parameters.AddWithValue("apdef", playerData.apdef);
            cmd.Parameters.AddWithValue("dodge", playerData.dodge);
            cmd.Parameters.AddWithValue("pierce", playerData.pierce);
            cmd.Parameters.AddWithValue("critical", playerData.critical);
            cmd.Parameters.AddWithValue("guideid", playerData.guideid);

            string strongArr = "";
            for (int i = 0; i < playerData.strongArr.Length; i++)
            {
                strongArr += playerData.strongArr[i];
                strongArr += "#";
            }
            cmd.Parameters.AddWithValue("strong", strongArr);
            cmd.Parameters.AddWithValue("time", playerData.time);

            // 数据形式：1 | 0 | 0#2|0|0#3|0|0#4|0|0#5|0|0#
            string taskArr = "";
            for (int i = 0; i < playerData.taskArr.Length; i++)
            {
                taskArr += playerData.taskArr[i];
                taskArr += "#";
            }
            cmd.Parameters.AddWithValue("task", taskArr);
            cmd.Parameters.AddWithValue("fuben", playerData.fuben);

            // TOADD

            cmd.ExecuteNonQuery();
        }
        catch (System.Exception e)
        {
            Common.Log("Update PlayerData Error:" + e, LogType.Error);
            return false;
        }

        return true;
    }
}

