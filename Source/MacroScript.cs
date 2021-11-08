using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class MacroScript
{
    public int CurrentLine = 0;
    public List<Instruction> Instructions = new List<Instruction>();

    public void ParseFile(string FilePath)
    {
        if (!File.Exists(FilePath))
        {
            Console.Write("file:{0} does not exist.", FilePath);
            return;
        }
        Console.WriteLine("Parsing file:'{0}'", FilePath);
        string[] Lines = File.ReadAllLines(FilePath);
        for(int i = 0; i < Lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(Lines[i])) continue;

            string[] Split = Lines[i].Split(' ');
            if (Split[0][0] == '#') continue;
            Instruction instruction = null;
            switch (Split[0])
            {
                case "CURSOR":
                    instruction = new CursorInstruction();
                    break;
                case "MOUSE":
                    instruction = new MouseInstruction();
                    break;
                case "SLEEP":
                case "WAIT":
                    instruction = new SleepInstruction();
                    break;
                case "JUMP":
                    instruction = new JumpInstruction(this);
                    break;
                case "KEY":
                    instruction = new KeyInstruction();
                    break;
            }
            if(instruction != null)
            {
                instruction.parameters = Split.Skip(1).ToArray();
                instruction.Parse();
                Instructions.Add(instruction);
            }
        }
    }
    public void Execute()
    {
        for (CurrentLine = 0; CurrentLine < Instructions.Count; CurrentLine++)
            Instructions[CurrentLine].Execute();
    }
    public void SaveToFile(string FilePath)
    {
        List<string> Lines = new List<string>();
        foreach(Instruction instruction in Instructions)
            Lines.Add(instruction.Compile());
        
        try
        {
            File.WriteAllLines(FilePath, Lines.ToArray());
        }catch(Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}