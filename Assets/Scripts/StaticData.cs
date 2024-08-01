using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class StaticData
{
    private static bool simRunning = false;

    public static bool SimRunning
    {
        get { return simRunning; }
        set { simRunning = value; }
    }

}
