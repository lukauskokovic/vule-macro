using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using static WindowsAPI;
public class ScriptEntry
{
    public WindowsKey ActivateKey, DeactiveKey;
    public string FileName;
    public MacroScript Script;
    private Thread ExecuteThread;
    public bool Running = false;
    public event EventHandler ScriptFinished;
    public ScriptEntry(string File)
    {
        Script = new MacroScript();
        Script.ParseFile(File);
        FileName = File;
        ActivateKey = new WindowsKey(KeyMap["NONE"]);
        DeactiveKey = new WindowsKey(KeyMap["NONE"]);
    }

    public void Cancel()
    {
        if (Running)
        {
            Console.WriteLine("Canceling " + Path.GetFileName(FileName));
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
                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    Script.Execute();
                }
                finally
                {
                    stopwatch.Stop();
                    Running = false;
                    Console.WriteLine("'{0}' finished worked for {1}", Path.GetFileName(FileName), stopwatch.Elapsed);
                    ScriptFinished?.Invoke(this, null);
                }
            });
            ExecuteThread.Start();
        }
    }

    public override string ToString()
    {
        return string.Format("FILE:'{0}' ToggleKey:{1} StopKey:{2}", FileName, ActivateKey.KeyID, DeactiveKey.KeyID);
    }
}

struct ScriptEntryDTO
{
    public int ActivateKey, DectivateKey;
    public string FileName;
    public ScriptEntryDTO(ScriptEntry entry)
    {
        ActivateKey = entry.ActivateKey.KeyID;
        DectivateKey = entry.DeactiveKey.KeyID;
        FileName = entry.FileName;
    }
}

public class WindowsKey
{
    public int KeyID;
    public int LastPressedState;
    public WindowsKey(int key) => KeyID = key;
    public bool Pressed()
    {
        int state = GetKeyState(KeyID);
        if(state < 0 && state != LastPressedState)
        {
            LastPressedState = state;
            return true;
        }

        return false;
    }
}
