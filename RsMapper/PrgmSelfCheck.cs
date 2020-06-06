﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace RsMapper
{
    public class PrgmSelfCheck
    {
        // DEPENDANCIES
        public static string ComponentsJson = "Components.json";
        public static string JsonNet =        "Newtonsoft.Json.dll";
        public static string JsonNetXml =     "Newtonsoft.Json.xml";

        WebClient wc = new WebClient();

        /// <summary>
        /// Check for any missing files that are required to start the program.
        /// </summary>
        public void CheckAll()
        {
            if(File.Exists(ComponentsJson) == false)
            {

                // Check for components settings file.
                if(MessageBox.Show("The file " + ComponentsJson + " is missing. Would you like to redownload it?", "Missing Dependancy", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {

                    // Reinstall the file if the user clicks yes.
                    wc.DownloadFile("https://raw.githubusercontent.com/GreenJamesDev/RsMapper/master/RsMapper/Components.json", AppDomain.CurrentDomain.BaseDirectory + "Components.json");


                } else
                {

                    // If the user chooses not to reinstall the file, exit RsMapper.
                    Application.Exit();
                }

            }



        }
        

    }
}
