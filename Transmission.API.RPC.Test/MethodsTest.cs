﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Transmission.API.RPC.Entity;
using Transmission.API.RPC.Arguments;

namespace Transmission.API.RPC.Test
{
    [TestClass]
    public class MethodsTest
    {
        const string FILE_PATH = "./Data/ubuntu-10.04.4-server-amd64.iso.torrent";
        const string HOST = "http://192.168.1.50:9091/transmission/rpc";
        const string SESSION_ID = "";

        Client client = new Client(HOST, SESSION_ID);

        #region Torrent Test

        [TestMethod]
        public void AddTorrent()
        {
            if (!File.Exists(FILE_PATH))
                throw new Exception("Torrent file not found");

            var fstream = File.OpenRead(FILE_PATH);
            byte[] filebytes = new byte[fstream.Length];
            fstream.Read(filebytes, 0, Convert.ToInt32(fstream.Length));
            
            string encodedData = Convert.ToBase64String(filebytes, Base64FormattingOptions.InsertLineBreaks);

            var torrent = new NewTorrent
            {
                Metainfo = encodedData,
                Paused = true
            };

            var torrentInfo = client.AddTorrent(torrent);
        }

        [TestMethod]
        public void GetTorrent()
        {
            var allTorrents = client.GetTorrents(TorrentFields.ALL_FIELDS);
            var oneTorrent = client.GetTorrents(TorrentFields.ALL_FIELDS, new int[] { 42 });
        }

        [TestMethod]
        public void RemoveTorrent()
        {
            client.RemoveTorrents(new int[] { 41 });
        }

        #endregion

        #region Session Test

		[TestMethod]
		public void SessionGetTest()
		{
			var info = client.GetSessionInformation();
			Assert.IsNotNull(info);
			Assert.IsNotNull(info.Version);
		}

		[TestMethod]
        public void ChangeSessionTest()
        {
            //Get current session information
            var sessionInformation = client.GetSessionInformation();

			//Save old speed limit up
			var oldSpeedLimit = sessionInformation.SpeedLimitUp;

            //Set new speed limit
			sessionInformation.SpeedLimitUp = 200;

            //Set new session settings
			client.SetSessionSettings(sessionInformation);

            //Get new session information
            var newSessionInformation = client.GetSessionInformation();

			//Check new speed limit
			Assert.AreEqual(newSessionInformation.SpeedLimitUp, 200);
            
			//Restore speed limit
            newSessionInformation.SpeedLimitUp = oldSpeedLimit;

            //Set new session settinhs
            client.SetSessionSettings(newSessionInformation);
        }

        #endregion
    }
}
