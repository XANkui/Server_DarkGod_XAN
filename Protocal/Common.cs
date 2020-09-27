

using PENet;
using Protocal;

public enum LogType {
    Log =0,
    Warn =1,
    Error=2,
    Info=3
}

public class Common
{
    public static void Log(string msg="",LogType tp= LogType.Log) {
        LogLevel lv = (LogLevel)tp;
        PETool.LogMsg(msg,lv);
    }

    public static int GetFightByProps(PlayerData playerData) {

        return playerData.lv * 100 + playerData.ad + playerData.ap + playerData.apdef + playerData.addef;
    }

    public static int GetPowerLimit(int lv) {

        //每十级，体力增加150
        return ((lv - 1) / 10) * 150 + 150;
    }

    public static int GetExpUpValByLv(int lv) {
        return 100 * lv * lv;
    }
}

