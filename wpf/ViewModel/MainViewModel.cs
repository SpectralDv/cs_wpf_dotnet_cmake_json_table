using System.Threading;
using System.IO;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;


using wpf.Models;
namespace wpf.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        //выбранный канал
        private string targetUnicNumber;
        //список имен сигналов на View
        public ObservableCollection<EntityViewModel> ViewNameTable {get;set;} 
        //выбранное имя на View
        public EntityViewModel SelectNameTable {get;set;} 
        public EntityViewModel SelectHandId {get;set;} 
        //список параметров выбранного сигнала
        public ObservableCollection<EntityViewModel> ViewHandId {get;set;}
        //выбранное результат на View
        public EntityViewModel SelectResultChannel {get;set;} 
        //список результатов обработки сигналов
        public ObservableCollection<EntityViewModel> ViewData {get;set;}

        //синхронизатор контекста потока UI
        private SynchronizationContext syncContext;

        public string TextLog {get;set;}

        //делегаты обработчиков комманд//
        public DelegateCommand commandOpenDialog;
        public DelegateCommand commandChangeLanguage;
        public DelegateCommand commandOpenHandId;
        public DelegateCommand commandOpenData;

        //биндинг комманд//
        public ICommand ClickOpenDialog {get{return commandOpenDialog;}}
        public ICommand ClickChangeLanguage {get{return commandChangeLanguage;}}
        //команда вызываемая нажатием на номер канала
        public ICommand OpenHandId{get{return commandOpenHandId;}}
        //команда вызываемая нажатием на результат
        public ICommand OpenData{get{return commandOpenData;}}

        ModelHand mHand = new ModelHand();

        public MainViewModel()
        {
            ViewHandId = new ObservableCollection<EntityViewModel>();
            ViewNameTable = new ObservableCollection<EntityViewModel>();
            ViewData = new ObservableCollection<EntityViewModel>();

            commandOpenDialog = new DelegateCommand((obj) =>{OpenDialogJson();});
            commandChangeLanguage = new DelegateCommand((obj) =>{ChangeLanguage();});
            commandOpenHandId = new DelegateCommand((obj) =>{ShowHandId(SelectNameTable!);});
            commandOpenData = new DelegateCommand((obj) =>{ShowData(SelectNameTable,SelectHandId!);});

            syncContext = SynchronizationContext.Current;
        }


        public void OpenDialogJson()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            folderDialog.Description = LangHelper.GetValue("selectedPath")!;
            folderDialog.ShowNewFolderButton = true;
            
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderDialog.SelectedPath;
                TextLog = selectedPath; 

                string error = mHand.ReadJson(selectedPath);
                if(error != ""){ViewNameTable.Add(new ViewModel(error)); return;}

                //заполнить левую колонку TableName 
                ViewNameTable.Clear();
                //расшифровка бинарников для каждого канала
                foreach (var pair in mHand.dictTable)
                {
                    ViewNameTable.Add(new ViewModel(pair.Key));
                }
            }
        }

        public void ShowHandId(object parameter)
        {
            if(parameter is ViewModel mTable)
            {
                TextLog = mTable.name;
                ViewHandId.Clear();

                if(mHand.dictTable.TryGetValue(mTable.name,out var dict1))
                {
                    foreach (var pair in mHand.dictTable[mTable.name])
                    {
                        ViewHandId.Add(new ViewModel(pair.Key));
                    }
                }
            }
        }

        public void ShowData(object parameter1,object parameter2)
        {
            if(parameter1 is ViewModel mTable1 && parameter2 is ViewModel mTable2)
            {
                TextLog = mTable1.name + " " + mTable2.name;
                ViewData.Clear();

                if(mHand.dictTable.TryGetValue(mTable1.name,out var dict1))
                {
                    if(mHand.dictTable[mTable1.name].TryGetValue(mTable2.name,out var dict2))
                    {
                        foreach (var pair in mHand.dictTable[mTable1.name][mTable2.name])
                        {
                            ViewData.Add(new ViewModel(pair.Value));
                        }
                    }
                }
            }
        }

        private string GetType(int type)
        {
            switch (type)
            {
                case 1:
                    return "EEG";
                case 2:
                    return "EKG";
                case 3:
                    return "EMG";
                default:
                    return "";
            }
        }

        public string tlNameTable {get;set;} = LangHelper.GetValue("tlNameTable")!;
        public string tlHandID {get;set;} = LangHelper.GetValue("tlHandID")!;
        public string tlData {get;set;} = LangHelper.GetValue("tlData")!;
        public string bOpenDialog {get;set;} = LangHelper.GetValue("bOpenDialog")!; 
        public string bChangeLanguage {get;set;} = LangHelper.GetValue("bChangeLanguage")!; 
        public string menuSettings {get;set;} = LangHelper.GetValue("menuSettings")!; 

        public string ChangeLanguage()
        {
            string result = LangHelper.ChangeCultureSystem();
            TextLog = result;
            
            tlNameTable = LangHelper.GetValue("tlNameTable")!;
            tlHandID = LangHelper.GetValue("tlHandID")!;
            tlData = LangHelper.GetValue("tlData")!;
            bOpenDialog = LangHelper.GetValue("bOpenDialog")!;
            bChangeLanguage = LangHelper.GetValue("bChangeLanguage")!;
            menuSettings = LangHelper.GetValue("menuSettings")!;

            return result;
        }
    }
}