using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DatingSoundManager
{
    private static string path = "VA/";

    public static AudioClip GetClip(UnitPair pair, string rank, int line) {
        string filePath = path;
        switch (pair) {
            case UnitPair.MINKEEVHALL:
                filePath += "vhallMinkee/";
                break;
            case UnitPair.PHYNNEMINKEE:
                filePath += "minkeePhynne/";
                break;
            case UnitPair.VHALLPHYNNE:
                filePath += "phynneVhall/";
                break;
        }

        filePath += rank + line.ToString();
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        return clip;
    }
}
