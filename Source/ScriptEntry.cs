using System;
using System.IO;
using System.Threading;

class ScriptEntry
{
    public MacroScript Script;
    public string FileName;
    public int KeyBind;
    public int LastKeyState;
    private Thread ExecuteThread;
    public bool Running = false;
    public ScriptEntry(string File)
    {
        Script = new MacroScript();
        Script.ParseFile(File);
        KeyBind = 0xA3;
        FileName = File;
    }

    public void Cancel()
    {
        if (Running)
        {
            Running = false;
            if(ExecuteThread != null)
                ExecuteThread.Abort();
        }
    }
    public void Start()
    {
        if (!Running) {
            Console.WriteLine("Executing " + Path.GetFileName(FileName));
            Running = true;
            ExecuteThread = new Thread(() =>
            {
                try
                {
                    Script.Execute();
                }
                finally
                {
                    Running = false;
                }
            });
            ExecuteThread.Start();
        }
    }
}