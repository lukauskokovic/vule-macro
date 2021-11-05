using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

public abstract class Instruction
{
    public string Name;
    public string[] parameters;
    public abstract void Execute();
    public abstract void Parse();
}

/// <summary>
/// Examples
/// SLEEP 10MS(mili secconds)
/// SLEEP 10M(10 minutes)
/// SLEEP 10S(10 seconds)
/// </summary>
public class SleepInstruction : Instruction
{
    public int Value;
    public string Unit;
    public SleepInstruction() { Name = "SLEEP"; }
    public override void Parse()
    {
        if (parameters.Length != 1)
            return;
        
        string timeStr = parameters[0];
        string unitStr = "";
        int time = 0;
        for (int i = 0; i < timeStr.Length; i++)
        {
            char c = timeStr[i];
            int numberValue;
            if (c == 'm' || c == 's' || c == 'M' || c == 'S') unitStr += c;
            else if((numberValue = c - '0') < 10)
            {
                time *= 10;
                time += numberValue;
            }
        }
        Unit = unitStr.ToUpper();
        Value = time;
        Console.WriteLine("SLEEP> Value'{0}' Unit:'{1}'", Value, Unit);
    }

    public override void Execute()
    {
        int time = 1;
        if (Unit == "MS") time = Value;
        else if (Unit == "S") time = Value * 1000;
        else if (Unit == "M") time = Value * 1000 * 60;
        Thread.Sleep(time);
    }
}
/// <summary>
/// Example
/// 
/// CURSOR POS 1920 1080
/// CURSOR MOVE 100 100
/// </summary>
public class CursorInstruction : Instruction
{
    [DllImport("user32.dll")]
    static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SetCursorPos(int x, int y);

    public string Type = null;
    public int ValueX, ValueY;

    string[] ValidTypes = new string[]
    {
        "POS",
        "MOVE"
    };
    public CursorInstruction() { Name = "CURSOR"; }
    public override void Parse()
    {
        if (parameters.Length != 3) return;
        for(int i = 0; i < ValidTypes.Length; i++)
            if(parameters[0].ToLower() == ValidTypes[i].ToLower())
                Type = ValidTypes[i];

        if (!int.TryParse(parameters[1], out ValueX)) return;
        if (!int.TryParse(parameters[2], out ValueY)) return;
        Console.WriteLine("CURSOR>  Type:'{0}' X:'{1}' Y:'{2}'", Type, ValueX, ValueY);
    }

    public override void Execute()
    {
        if (Type == null) return;

        if (Type == "POS") SetCursorPos(ValueX, ValueY);
        else if(Type == "MOVE")
        {
            mouse_event(0x0001, ValueX, ValueY, 0, (UIntPtr)0);
        }
    }
}
/// <summary>
/// Example
/// MOUSE DOWN LEFT
/// MOUSE UP RIGHT
/// </summary>
public class MouseInstruction : Instruction
{
    [DllImport("user32.dll")]
    static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);
    public MouseInstruction() { Name = "MOUSE"; }
    public string Button;
    public string Direction = null;


    private const uint RIGHT_UP = 0x0010,
                       RIGHT_DOWN = 0x0008,
                       LEFT_DOWN = 0x0002,
                       LEFT_UP = 0x0004;

    public override void Parse()
    {
        if (parameters.Length != 2) return;

        if (parameters[0].ToUpper() == "DOWN" || parameters[0].ToUpper() == "UP") Direction = parameters[0].ToUpper();
        else return;
        if (parameters[1].ToUpper() == "LEFT" || parameters[1].ToUpper() == "RIGHT") Button = parameters[1].ToUpper();
        else return;

        Console.WriteLine("MOUSE> Direction:'{0}' Button:'{1}'", Direction, Button);
    }

    public override void Execute()
    {
        mouse_event(Direction == "UP"? Button == "LEFT" ? LEFT_UP : RIGHT_UP:
                                        Button == "LEFT"? LEFT_DOWN : RIGHT_DOWN, 0, 0, 0, (UIntPtr)0);
    }
}