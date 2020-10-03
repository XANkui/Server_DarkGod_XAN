
using System.Threading;

class ServerStart
{
    static void Main(string[] args)
    {
        ServerRoot.Instance.Init();

        while (true)
        {
            ServerRoot.Instance.Update();
            // 开发的时候节约性能（正式的时候可以去掉）
            Thread.Sleep(20);
        }
    }
}

