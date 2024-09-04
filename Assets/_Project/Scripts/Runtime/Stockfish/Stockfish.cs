// CREDIT: https://github.com/Oremiro/Stockfish.NET/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class Stockfish
{

    private Process _process;
    private ProcessStartInfo _startInfo;


    private int _depth;

    public Stockfish(int depth)
    {
        _depth = depth;

        _startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(Application.streamingAssetsPath, "stockfish/stockfish-windows-x86-64-avx2.exe"),
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };

        _process = new Process
        {
            StartInfo = _startInfo
        };
    }


    public void Start()
    {
        _process.Start();
    }

    private void Send(string command, int estimatedTime = 100)
    {
        WriteLine(command);
        Wait(estimatedTime);
    }

    private void Go()
    {
        Send($"go depth {_depth}");
    }

    public string GetBestMove()
    {
        Go();
        var tries = 0;
        while (true)
        {
            if (tries > 200)
            {
                throw new Exception("Exceeded maximum tries");
            }

            var data = ReadLineAsList();
            if (data[0] == "bestmove")
            {
                if (data[1] == "(none)")
                {
                    return null;
                }

                return data[1];
            }
        }
    }
    
    private List<string> ReadLineAsList()
    {
        var data = ReadLine();
        return data.Split(' ').ToList();
    }

    public void SetFenPosition(string fen)
    {
        Send($"position fen {fen}");
    }



    private void WriteLine(string command)
    {
        if (_process.StandardInput == null)
        {
            throw new NullReferenceException();
        }

        _process.StandardInput.WriteLine(command);
        _process.StandardInput.Flush();
    }

    private string ReadLine()
    {
        if (_process.StandardInput == null)
        {
            throw new NullReferenceException();
        }

        return _process.StandardOutput.ReadLine();
    }

    private void Wait(int milliseconds)
    {
        _process.WaitForExit(milliseconds);
    }
   
    public void StopStockfish()
    {
        if (_process != null && _process.HasExited)
        {
            _process.Kill();
            _process.Dispose();
        }
    }
}
