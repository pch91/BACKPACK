using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BACKPACK.patch;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BACKPACK
{
    [BepInPlugin("SOGS", "BACKPACK", "0.0.0.1")]
    public class SOGS : BaseUnityPlugin
    {
        public static SOGS Instance;

        private static string loglevel = "INFO";

        public enum Logs
        {
            DEBUG = 1,
            ERROR = 2,
            INFO = 0,
        }

        public static void log(string line, Logs level)
        {
            //Debug.Log((int)Enum.Parse(typeof(Logs), loglevel));

            if ((int)Enum.Parse(typeof(Logs), loglevel) - (int)level >= 0)
            {
                Debug.Log("[" + level + "    :   SOGS] " + line);
            }
        }


        public static Dictionary<string, object> fmainconfig = new Dictionary<string, object>();
        public static Dictionary<string, object> fconfigEvents = new Dictionary<string, object>();
        private Dictionary<string, object> mainconfigs = new Dictionary<string, object>();


        private void Awake()
        {
            try
            {
                log("Start - SOGS", Logs.INFO);
                Instance = this;
                //Harmony.DEBUG = true;
                Handleconfig();
                if (bool.Parse(StaticAttributes.configs["EnabledMod"].ToString()))
                {
                    var harmony = new Harmony("net.pch91.stationeers.SOGS.patch");
                    if (bool.Parse(StaticAttributes.configs["EnabledBP"].ToString()))
                    {
                        log("Load - Backpack System", Logs.INFO);

                        harmony.PatchAll(typeof(BackpackPatch));
                    }
                }
                log("Finish patch", Logs.INFO);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void Handleconfig()
        { 
            mainconfigs.Add("LogEnabled", Config.Bind("0 - General configuration", "Log Level", "info", "Enable or disable logs. values can be debug , info or error"));
            mainconfigs.Add("EnabledMod", Config.Bind("0 - General configuration", "Eneble mod", true, "Enable or disable mod. values can be false or true"));
            mainconfigs.Add("EnabledBP", Config.Bind("0 - General configuration", "Eneble Backpack System", true, "Enable or disable part of mod. values can be false or true"));

            fconfigEvents.Add("beltPossitionBP", Config.Bind("3 - Backpack configuration", "Belt Slot Possition", 0, "The position where the belt can be placed needs to change if you modify the start game or use mods that modify it so that the belts are generated. values from 1 to 10 \n 0 defines the last slot"));

            loglevel = (mainconfigs["LogEnabled"] as ConfigEntry<string>).Value.ToUpper();

            StaticConfig();
        }

        private void StaticConfig()
        {
            StaticAttributes.configs.Add("EnabledMod", (mainconfigs["EnabledMod"] as ConfigEntry<bool>).Value);
            StaticAttributes.configs.Add("EnabledBP", (mainconfigs["EnabledBP"] as ConfigEntry<bool>).Value);

            StaticAttributes.beltPosition = (fconfigEvents["beltPossitionBP"] as ConfigEntry<int>).Value;
        }
    }
}