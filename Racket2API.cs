using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace rackets2listener
{
    static class Rackets2API
    {
        private static uint tasState = 0x1BE0270;
        private static uint tasStop = 0x1BE0274;
        private static uint frameStepMode = 0x1BE0028;
        private static uint frameAdvance = 0x1BE002C;
        private static uint tasRenderMode = 0x1BE0038;
        private static uint tasGCMFlip = 0x1BE003C;

        private static uint savefileLoadAside = 0x1BE0040;
        private static uint savefileSetAside = 0x1BE0044;

        private static uint tasHideHud = 0x1BE0048;

        private static string currentScriptFile = null;

        public static string UploadInputsFile(this Ratchetron api, string scriptFilePath)
        {
            if (!File.Exists(scriptFilePath))
            {
                return $"Script file not found: {scriptFilePath}";
            }

            string scriptDir = Path.GetDirectoryName(scriptFilePath);
            string compiledFileName = Path.GetFileNameWithoutExtension(scriptFilePath) + ".rtas";
            string compiledFilePath = Path.Combine(scriptDir, compiledFileName);

            try
            {
                ScriptSerializer.Compile(scriptFilePath, compiledFilePath);
            }
            catch (Exception e)
            {
                return $"compilation failed: {e.Message}\n";
            }

            Console.Write("Uploading... ");
            try
            {
                api.WriteFile("/dev_hdd0/game/NPEA00386/USRDIR/recording.rtas", compiledFilePath);
                Console.WriteLine("done.");

                api.ReloadSetState(4);
                // Thread.Sleep(200);
            }
            catch (Exception e)
            {
                return $"Upload failed: {e.Message}";
            }

            return null;
        }

        private static void WriteUint(this Ratchetron api, uint addr, uint state)
        {
            api.WriteMemory(api.getCurrentPID(), addr, state);
        }

        public static void PauseUnpauseRackets(this Ratchetron api, bool pause)
        {
            api.WriteUint(frameStepMode, (uint)(pause ? 1 : 0));
        }

        public static void Framestep(this Ratchetron api)
        {
            api.WriteUint(frameAdvance, 1);
        }

        public static void SetAsideMethod(this Ratchetron api)
        {
            api.WriteMemory(api.getCurrentPID(), savefileSetAside, new byte[1] { 1 });
        }

        public static void LoadSetAsideMethod(this Ratchetron api)
        {
            api.WriteMemory(api.getCurrentPID(), savefileLoadAside, new byte[1] { 1 });
        }

        private static void ReloadSetState(this Ratchetron api, uint state)
        {
            api.LoadSetAsideMethod();
            api.WriteUint(tasState, state);
        }

        public static void CancelPlayback(this Ratchetron api)
        {
            api.WriteUint(tasStop, 1);
        }

        public static void RestartPlayback(this Ratchetron api)
        {
            api.ReloadSetState(4);
        }

        public static void SetRenderingMode(this Ratchetron api, bool skipFrames, bool skipRender)
        {
            api.WriteUint(tasRenderMode, (uint)(skipRender ? 1 : 0));
            api.WriteUint(tasGCMFlip, (uint)(skipFrames ? 1 : 0));
        }

        public static void SetHudStatus(this Ratchetron api, bool hide)
        {
            api.WriteUint(tasHideHud, (uint)(hide ? 1 : 0));
        }
    }
}
