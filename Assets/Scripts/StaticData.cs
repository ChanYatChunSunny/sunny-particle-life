using System;
using UnityEngine;

static class StaticData
{
    public const int NumOfTypes = 8;
    public static readonly String ConfigPath = Application.persistentDataPath + "/config.tsv";
    private static bool simRunning = false;
    public static bool SimRunning
    {
        get { return simRunning; }
        set { simRunning = value; }
    }

}
