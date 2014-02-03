//-----------------------------------------------------------------------
// <summary>メインウィンドウViewModel</summary>
// <author>MayaTakimoto</author>
// <date>$Date: 2014-01-31 00:00:00 +9:00 $</date>
// <copyright file="$Name: MainViewModel.cs $" >
// Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------
using Climie3.model;
using Livet;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace Climie3.viewmodel
{
    /// <summary>
    /// メインVMクラス
    /// </summary>
    [ProtoContract]
    public class MainViewModel : ViewModel
    {
        // データ保持用リスト
        [ProtoMember(1)]
        private ObservableCollection<ListItemViewModel> listMain;

        // 表示用リスト
        private ObservableCollection<ListItemViewModel> listDsp;

        // 履歴件数
        private string listCount;
        
        // 検索モード
        private string[] searchModeList;

        // クリップボード監視オブジェクト
        private ClipboradWatcherModel cWatch;

        // スレッド
        private Thread thread;

        // 正規表現パターン生成オブジェクト
        private RegexManagerModel regMan;

        /// <summary>
        /// データ保持用リストプロパティ
        /// </summary>
        /// <remarks>取得した値はこちらに保持される</remarks>
        private ObservableCollection<ListItemViewModel> ListMain
        {
            get
            {
                return this.listMain;
            }
            set
            {
                this.listMain = value;

                // 画面に反映させる
                ListDsp = this.listMain;
            }
        }

        /// <summary>
        /// 表示用リストプロパティ
        /// </summary>
        /// <remarks>画面にはこのリストの内容が反映される</remarks>
        public ObservableCollection<ListItemViewModel> ListDsp
        {
            get
            {
                return this.listDsp;
            }
            set
            {
                this.listDsp = value;
                RaisePropertyChanged("ListDsp");
            }
        }

        /// <summary>
        /// 履歴件数プロパティ
        /// </summary>
        public string ListCount
        {
            get
            {
                return this.listCount;
            }
            set
            {
                this.listCount = value; 
                RaisePropertyChanged("ListCount");
            }
        }

        /// <summary>
        /// 検索モードプロパティ
        /// </summary>
        public string[] SearchModeList
        {
            get
            {
                if (searchModeList == null)
                {
                    searchModeList = new string[] { "Text", "Tags" };
                }

                return searchModeList;
            }
        }

        /// <summary>
        /// 現在の検索モード
        /// </summary>
        public string SearchMode { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainViewModel()
        {
            cWatch = new ClipboradWatcherModel();

            // VM破棄時に同時に破棄されるように設定
            CompositeDisposable.Add(cWatch);
            CompositeDisposable.Add(regMan);

            // データ復元
            this.Load();

            ListCount = "Count : " + ListMain.Count;
            this.ThreadStart();
        }

        /// <summary>
        /// クリップボード変更検知時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cWatch_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // クリップボードが空の場合は何もしない
            if (string.IsNullOrEmpty(cWatch.CbText))
            {
                return;
            }

            // 同じテキストデータがリスト内に存在する場合は何もしない
            if (ListMain.Any(item => item.Text.Equals(cWatch.CbText)))
            {
                return;
            }

            // テキストデータをリストに追加
            Application.Current.Dispatcher.Invoke(
                new Action(() =>
                {
                    ListItemViewModel newItem = new ListItemViewModel();
                    newItem.Text = cWatch.CbText;
                    newItem.Tags.Add(cWatch.CbText);

                    ListMain.Add(newItem);
                })
            );

            // 最新の履歴件数を取得
            ListCount = "Count : " + ListMain.Count;
        }

        /// <summary>
        /// クリップボード監視の開始
        /// </summary>
        public void ThreadStart()
        {
            if (thread == null)
            {
                // クリップボード変更検知イベントを登録
                cWatch.PropertyChanged += new PropertyChangedEventHandler(cWatch_PropertyChanged);

                thread = new Thread(new ThreadStart(cWatch.WatchClipboard));
                thread.SetApartmentState(ApartmentState.STA);

                // 監視開始
                cWatch.StopFlg = false;
                thread.Start();
            }
        }

        /// <summary>
        /// クリップボード監視の終了
        /// </summary>
        public void ThreadStop()
        {
            if (thread != null)
            {
                cWatch.StopFlg = true;

                // 監視終了
                thread.Abort();
                thread = null;

                // クリップボード変更検知イベントの登録を解除
                cWatch.PropertyChanged -= cWatch_PropertyChanged;
            }
        }

        /// <summary>
        /// 取得済みデータ保存
        /// </summary>
        public void Save()
        {
            DataSaveLoadModel.Save<ListItemViewModel>(ListMain, "");
        }

        /// <summary>
        /// 取得済みデータ復元
        /// </summary>
        public void Load()
        {
            ListMain = DataSaveLoadModel.Load<ListItemViewModel>("");
        }

        /// <summary>
        /// 検索前処理
        /// </summary>
        public void InitSearch()
        {
            regMan = new RegexManagerModel();
        }

        /// <summary>
        /// 検索処理
        /// </summary>
        /// <param name="pattern"></param>
        public void Search(string pattern)
        {
            string mode = SearchMode;
            
            if (mode == "Text")
            {
                ListDsp = SearchByText(pattern);
            }
            else
            {
                ListDsp = SearchByTags(pattern);
            }
        }

        /// <summary>
        /// 検索後処理
        /// </summary>
        public void EndSearch()
        {
            if (regMan != null)
            {
                regMan.Dispose();
            }
        }

        /// <summary>
        /// アプリケーションの終了
        /// </summary>
        public void Shutdown()
        {
            this.ThreadStop();

            // 取得済みデータの保存
            this.Save();

            Application.Current.Shutdown();
        }

        /// <summary>
        /// テキスト検索
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private ObservableCollection<ListItemViewModel> SearchByText(string pattern)
        {
            ObservableCollection<ListItemViewModel> oc = null;
            Regex regex = null;

            if (string.IsNullOrEmpty(pattern) || "/".Equals(pattern))
            {
                oc = ListMain;
            }
            else
            {
                regex = regMan.GetRegex(pattern);

                if (regex != null)
                {
                    oc = new ObservableCollection<ListItemViewModel>(ListMain.Where((item) =>
                    {
                        return regex.IsMatch(item.Text);
                    }));
                }
            }

            return oc;
        }

        /// <summary>
        /// タグ検索
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private ObservableCollection<ListItemViewModel> SearchByTags(string pattern)
        {
            ObservableCollection<ListItemViewModel> oc = null;
            Regex regex = null;

            if (string.IsNullOrEmpty(pattern) || "/".Equals(pattern))
            {
                oc = ListMain;
            }
            else
            {
                regex = regMan.GetRegex(pattern);

                oc = new ObservableCollection<ListItemViewModel>(ListMain.Where((item) =>
                {
                    if (item.Tags.Any(tag => regex.IsMatch(tag)))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }));
            }

            return oc;
        }
    }
}
