using System;
using System.Collections.Generic;
using Unigine;

namespace UnigineApp.data.Scripts.Menu
{
    internal class SaveManger
    {
        public enum DATA { AUTOSAVE, SAVE1, SAVE2 }
        string
            Path = Environment.GetEnvironmentVariable("USERPROFILE"),
            Selector = "RunTimeFiles\\Selector.txt",
            SavePath1 = "CompanyName\\GameName\\AutoSave",
            SavePath2 = "CompanyName\\GameName\\SaveFile1",
            SavePath3 = "CompanyName\\GameName\\SaveFile2",
            LoadWorld = "EmptyWorld.world";

        List<Node> Objects = new List<Node>();
        File File;
        String SelectorString;
        Node ExclusionFile;

        ~SaveManger() { File.DeleteLater(); Objects.Clear(); }
        public SaveManger() { }
        public SaveManger(Node ExclusionFile) {

            Path += "\\Documents\\";

            File = new File();
            this.ExclusionFile = ExclusionFile;

            if (File.Open(Selector, "rb")) { File.Close();GetValueInsideSelector(); }
            else
            {
                File.Open(Selector, "wb");
                File.WriteString("0");
                File.Close();
            }
            World.AutoReloadNodeReferences = true;
        }

        public void Save(DATA Num)
        {
            World.GetNodes(Objects);
            Objects.Remove(ExclusionFile);
            Objects.ForEach(obj => Log.Message("{0}  ID: {1} \n",obj.Name, obj.ID));
            Log.Message("\n Saved. \n\n\n ");

            switch (Num)
            {
                case DATA.AUTOSAVE: World.SaveNodes(Path + SavePath1, Objects.ToArray(), 0); break;
                case DATA.SAVE1:    World.SaveNodes(Path + SavePath2, Objects.ToArray(), 0); break;
                case DATA.SAVE2:    World.SaveNodes(Path + SavePath3, Objects.ToArray(), 0); break;
                default: break;
            }
        }

        public void Load(DATA Num)
        {
            File.Open(Selector, "w");

            switch (Num)
            {
                case DATA.AUTOSAVE: File.WriteString("0"); break;
                case DATA.SAVE1:    File.WriteString("1"); break;
                case DATA.SAVE2:    File.WriteString("2"); break;
                default: break;
            }

            File.Close();
            World.LoadWorld(LoadWorld);
        }

        public bool FileExist(DATA dATA)
        {
            bool isOpen = false;

            switch (dATA)
            {
                case DATA.AUTOSAVE: File.Open(Path + SavePath1, "r"); isOpen = File.IsOpened; break;
                case DATA.SAVE1: File.Open(Path + SavePath2, "r"); isOpen = File.IsOpened; break;
                case DATA.SAVE2: File.Open(Path + SavePath3, "r"); isOpen = File.IsOpened; break;
                default: isOpen = false; break;
            }

            if (isOpen) { File.Close(); }
            return isOpen;
        }

        void GetValueInsideSelector()
        {
            File.Open(Selector, "rb");
            SelectorString = File.ReadString();
            File.Close();
        }

        public void LoadObjectsIntoWorld()
        {
            GetValueInsideSelector();
            if (SelectorString == "0") World.LoadNodes(Path + SavePath1, Objects);
            if (SelectorString == "1") World.LoadNodes(Path + SavePath2, Objects);
            if (SelectorString == "2") World.LoadNodes(Path + SavePath3, Objects);
        }

    }
}
