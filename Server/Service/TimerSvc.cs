

using System;
using System.Collections.Generic;
/// <summary>
/// 定时服务
/// </summary>
public class TimerSvc
{
    class TaskPack {
        public int tid;
        public Action<int> cb;

        public TaskPack(int tid, Action<int> cb)
        {
            this.tid = tid;
            this.cb = cb;
        }
    }

    private static TimerSvc instance = null;
    public static TimerSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TimerSvc();
            }

            return instance;
        }
    }

    private PETimer pt;
    private Queue<TaskPack> tpQue = new Queue<TaskPack>();
    private static readonly string tpQueLock = "tpQueLock";

    public void Init()
    {
      

        pt = new PETimer(100);

        // 设置日志输出
        pt.SetLog((info) => {
            Common.Log(info);
        });

        pt.SetHandle((Action<int> cb, int tid)=> {
            if (cb != null)
            {
                lock (tpQueLock)
                {
                    tpQue.Enqueue(new TaskPack(tid,cb));
                }
            }
        });

        Common.Log("TimerSvc Init Done.");

    }

    public void Update()
    {
        while (tpQue.Count>0)
        {
            TaskPack tp = null;
            lock (tpQueLock)
            {
                tp = tpQue.Dequeue();
            }

            if (tp !=null)
            {
                tp.cb(tp.tid);
            }
        }
    }

    public int AddTimeTask(Action<int> callback, double delay, PETimeUnit unit = PETimeUnit.Millisecond, int count = 1)
    {
        return pt.AddTimeTask(callback, delay, unit, count);
    }


    public long GetNowTime() {
        return (long)pt.GetMillisecondsTime();
    }
}

