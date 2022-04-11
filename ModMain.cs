using Assets.Main.Scenes;
using Assets.Scripts.Unity;
using HarmonyLib;
using MelonLoader;
using System.IO;
using Assets.Scripts.Data.Music;
using static BetterJukebox.ModMain;
using UnityEngine;
using Assets.Scripts.Data;
using NinjaKiwi.Common;
using System.Collections.Generic;
using NLayer;
using Assets.Scripts.Unity.UI_New.InGame;
using System;
[assembly: MelonInfo(typeof(BetterJukebox.ModMain),"Better Jukebox","1.1.0","BowDown097, updated by Silentstorm")]
[assembly: MelonGame("Ninja Kiwi","BloonsTD6")]
namespace BetterJukebox{
    public class ModMain:MelonMod{
        private static MelonLogger.Instance mllog=new MelonLogger.Instance("Better Jukebox");
        public static void Log(object thingtolog,string type="msg"){
            switch(type){
                case"msg":
                    mllog.Msg(thingtolog);
                    break;
                case"warn":
                    mllog.Warning(thingtolog);
                    break;
                 case"error":
                    mllog.Error(thingtolog);
                    break;
            }
        }
        public static List<AudioClip>clips=new List<AudioClip>();
        public override void OnApplicationStart(){
            string modsFolder=MelonHandler.ModsDirectory;
            if(!Directory.Exists(modsFolder+"\\Jukebox")){
                Directory.CreateDirectory(modsFolder+"Jukebox");
            }
            string[]files=Directory.GetFiles(modsFolder+"\\Jukebox");
            if(files.Length>0){
                foreach(string mp3 in files){
                    try{
                        string filename=new FileInfo(mp3).Name;
                        filename=filename.Remove(filename.Length-4);
                        Log(filename);
                        MpegFile mpegFile=new MpegFile(mp3);
                        float[]samples=new float[mpegFile.Length/mpegFile.Channels/2];
                        mpegFile.ReadSamples(samples,0,samples.Length);
                        AudioClip clip=AudioClip.Create(filename,samples.Length/2,mpegFile.Channels,mpegFile.SampleRate,false);
                        clip.SetData(samples,1);
                        clips.Add(clip);
                    }catch(Exception exception){
                        Log(exception.Message);
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(TitleScreen),"OnPlayButtonClicked")]
    public class TitleScreenOnPlayButtonClicked_Patch{
        [HarmonyPostfix]
        public static void Postfix(){
            if(clips.Count>0){
                foreach(AudioClip clip in clips){
                    Game.instance.audioFactory.RegisterAudioClip(clip.name,clip);
                    LocalizationManager.Instance.textTable.Add(clip.name,clip.name);
                    LocalizationManager.Instance.defaultTable.Add(clip.name,clip.name);
                    MusicItem musicItem=new MusicItem();
                    musicItem.name=clip.name;
                    musicItem.freeTrack=true;
                    musicItem.Clip=clip;
                    musicItem.hideFlags=0;
                    musicItem.id=clip.name;
                    musicItem.locKey=clip.name;
                    GameData.Instance.audioJukeBox.musicTrackData.Add(musicItem);
                }
            }
        }
    }
}