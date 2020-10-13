using System;
using System.Collections.Generic;
using System.Xml;

public class CfgSvc
{
    private static CfgSvc instance = null;
    public static CfgSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CfgSvc();
            }

            return instance;
        }
    }   

    public void Init()
    {
        InitGuideCfg(@"D:\ProgramFile\UnityProjects\DarkGod_XAN\Assets\Resources\ResCfgs\guide.xml");
        InitStrongCfg(@"D:\ProgramFile\UnityProjects\DarkGod_XAN\Assets\Resources\ResCfgs\strong.xml");
        InitTaskRewardCfg(@"D:\ProgramFile\UnityProjects\DarkGod_XAN\Assets\Resources\ResCfgs\taskreward.xml");
        InitMapCfg(@"D:\ProgramFile\UnityProjects\DarkGod_XAN\Assets\Resources\ResCfgs\map.xml");

        Common.Log("CfgSvc Init Done.");
    }

    #region 自动引导配置
    private Dictionary<int, GuideCfg> guideTaskDic = new Dictionary<int, GuideCfg>();
    private void InitGuideCfg(string path)
    {        
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

        for (int i = 0; i < nodLst.Count; i++)
        {
            XmlElement ele = nodLst[i] as XmlElement;

            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

            GuideCfg agc = new GuideCfg
            {
                ID = ID
            };

            foreach (XmlElement e in nodLst[i].ChildNodes)
            {
                switch (e.Name)
                {

                    case "coin":
                        agc.coin = int.Parse(e.InnerText);
                        break;

                    case "exp":
                        agc.exp = int.Parse(e.InnerText);
                        break;


                }
            }

            guideTaskDic.Add(ID, agc);
        }

        Common.Log("Guide Cfg Init Done.");
    }

    public GuideCfg GetGuideCfg(int id)
    {
        GuideCfg agc = null;
        guideTaskDic.TryGetValue(id, out agc);

        return agc;
    }

    #endregion

    #region 强化配置数据
    // 位置（星级，配置）
    private Dictionary<int, Dictionary<int, StrongCfg>> strongDic = new Dictionary<int, Dictionary<int, StrongCfg>>();

    private void InitStrongCfg(string path)
    {

        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

        for (int i = 0; i < nodLst.Count; i++)
        {
            XmlElement ele = nodLst[i] as XmlElement;

            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

            StrongCfg sc = new StrongCfg
            {
                ID = ID
            };

            foreach (XmlElement e in nodLst[i].ChildNodes)
            {
                int val = int.Parse(e.InnerText); ;
                switch (e.Name)
                {
                    case "pos":
                        sc.pos = val;
                        break;
                    case "starlv":
                        sc.starlv = val;
                        break;
                    case "adddef":
                        sc.adddef = val;
                        break;
                    case "addhp":
                        sc.addhp = val;
                        break;
                    case "addhurt":
                        sc.addhurt = val;
                        break;
                    case "minlv":
                        sc.minlv = val;
                        break;
                    case "coin":
                        sc.coin = val;
                        break;
                    case "crystal":
                        sc.crystal = val;
                        break;


                }
            }

            Dictionary<int, StrongCfg> dic = null;
            if (strongDic.TryGetValue(sc.pos, out dic) == true)
            {
                dic.Add(sc.starlv, sc);
            }
            else
            {
                dic = new Dictionary<int, StrongCfg>();
                dic.Add(sc.starlv, sc);
                strongDic.Add(sc.pos, dic);
            }



        }

        Common.Log("Strong Cfg Init Done.");
    }

    public StrongCfg GetStrongCfg(int pos, int starlv)
    {
        StrongCfg sc = null;
        Dictionary<int, StrongCfg> dic = null;

        if (strongDic.TryGetValue(pos, out dic) == true)
        {
            if (dic.ContainsKey(starlv))
            {
                sc = dic[starlv];
            }
        }

        return sc;
    }
    #endregion

    #region 任务奖励配置
    private Dictionary<int, TaskRewardCfg> taskRewardDic = new Dictionary<int, TaskRewardCfg>();
    private void InitTaskRewardCfg(string path)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

        for (int i = 0; i < nodLst.Count; i++)
        {
            XmlElement ele = nodLst[i] as XmlElement;

            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

            TaskRewardCfg trc = new TaskRewardCfg
            {
                ID = ID
            };

            foreach (XmlElement e in nodLst[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "count":
                        trc.count = int.Parse(e.InnerText);
                        break;

                    case "coin":
                        trc.coin = int.Parse(e.InnerText);
                        break;

                    case "exp":
                        trc.exp = int.Parse(e.InnerText);
                        break;


                }
            }

            taskRewardDic.Add(ID, trc);
        }

        Common.Log("TaskReward Cfg Init Done.");
    }

    public TaskRewardCfg GetTaskRewardCfg(int id)
    {
        TaskRewardCfg trc = null;
        taskRewardDic.TryGetValue(id, out trc);

        return trc;
    }

    #endregion

    #region 任务奖励配置
    private Dictionary<int, MapCfg> mapDic = new Dictionary<int, MapCfg>();
    private void InitMapCfg(string path)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

        for (int i = 0; i < nodLst.Count; i++)
        {
            XmlElement ele = nodLst[i] as XmlElement;

            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

            MapCfg mc = new MapCfg
            {
                ID = ID
            };

            foreach (XmlElement e in nodLst[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "power":
                        mc.power = int.Parse(e.InnerText);
                        break;

                    case "coin":
                        mc.coin = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        mc.exp = int.Parse(e.InnerText);
                        break;
                    case "crystal":
                        mc.crystal = int.Parse(e.InnerText);
                        break;

                }
            }

            mapDic.Add(ID, mc);
        }

        Common.Log("MapCfg Cfg Init Done.");
    }

    public MapCfg GetMapCfg(int id)
    {
        MapCfg mc = null;
        mapDic.TryGetValue(id, out mc);

        return mc;
    }

    #endregion
}



public class MapCfg : BaseData<GuideCfg>
{

    public int power;
    public int coin;
    public int exp;
    public int crystal;
    
}

public class StrongCfg : BaseData<StrongCfg>
{
    public int pos;
    public int starlv;
    public int addhp;
    public int addhurt;
    public int adddef;
    public int minlv;
    public int coin;
    public int crystal;
}

public class GuideCfg : BaseData<GuideCfg>
{

    public int coin;
    public int exp;
}

public class TaskRewardCfg : BaseData<TaskRewardCfg>
{
    public int count;
    public int exp;
    public int coin;
}

public class TaskRewardData : BaseData<TaskRewardCfg>
{

    public int prgs;
    public bool isTakenReward;      // 是否领取奖励

}

public class BaseData<T>
{
    public int ID;
}

