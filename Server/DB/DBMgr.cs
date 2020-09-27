
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

                        hp = reader.GetInt32("hp"),
                        ad = reader.GetInt32("ad"),
                        ap = reader.GetInt32("ap"),
                        addef = reader.GetInt32("addef"),
                        apdef = reader.GetInt32("apdef"),
                        dodge = reader.GetInt32("dodge"),
                        pierce = reader.GetInt32("pierce"),
                        critical = reader.GetInt32("critical")

                        //Todo Add
                    };
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
                    lv=1,
                    exp=0,
                    power=150,
                    coin=5000,
                    diamond=500,

                    hp = 2000,
                    ad = 275,
                    ap = 265,
                    addef = 67,
                    apdef = 43,
                    dodge = 7,
                    pierce = 5,
                    critical = 2
                    //Todo Add

                };
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
                "hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical", conn
                );
            cmd.Parameters.AddWithValue("acct",acct);
            cmd.Parameters.AddWithValue("pass", pass);
            cmd.Parameters.AddWithValue("name", playerData.name);
            cmd.Parameters.AddWithValue("level", playerData.lv);
            cmd.Parameters.AddWithValue("exp", playerData.exp);
            cmd.Parameters.AddWithValue("power", playerData.power);
            cmd.Parameters.AddWithValue("coin", playerData.coin);
            cmd.Parameters.AddWithValue("diamond", playerData.diamond);

            cmd.Parameters.AddWithValue("hp", playerData.hp);
            cmd.Parameters.AddWithValue("ad", playerData.ad);
            cmd.Parameters.AddWithValue("ap", playerData.ap);
            cmd.Parameters.AddWithValue("addef", playerData.addef);
            cmd.Parameters.AddWithValue("apdef", playerData.apdef);
            cmd.Parameters.AddWithValue("dodge", playerData.dodge);
            cmd.Parameters.AddWithValue("pierce", playerData.pierce);
            cmd.Parameters.AddWithValue("critical", playerData.critical);

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
                "update account set name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond,"+
                "hp = @hp, ad = @ad, ap = @ap, addef = @addef, apdef = @apdef, dodge = @dodge, pierce = @pierce, critical = @critical" +
                " where id = @id",conn
                );

            cmd.Parameters.AddWithValue("id",id);
            cmd.Parameters.AddWithValue("name", playerData.name);
            cmd.Parameters.AddWithValue("level", playerData.lv);
            cmd.Parameters.AddWithValue("exp", playerData.exp);
            cmd.Parameters.AddWithValue("power", playerData.power);
            cmd.Parameters.AddWithValue("coin", playerData.coin);
            cmd.Parameters.AddWithValue("diamond", playerData.diamond);

            cmd.Parameters.AddWithValue("hp", playerData.hp);
            cmd.Parameters.AddWithValue("ad", playerData.ad);
            cmd.Parameters.AddWithValue("ap", playerData.ap);
            cmd.Parameters.AddWithValue("addef", playerData.addef);
            cmd.Parameters.AddWithValue("apdef", playerData.apdef);
            cmd.Parameters.AddWithValue("dodge", playerData.dodge);
            cmd.Parameters.AddWithValue("pierce", playerData.pierce);
            cmd.Parameters.AddWithValue("critical", playerData.critical);
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

