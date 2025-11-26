using System;

namespace MarioRunner
{
    /// <summary>
    /// 程序入口类
    /// </summary>
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MarioRunnerGame())
                game.Run();
        }
    }
}
